using System.Linq;
using System.Transactions;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class ChangeNameAccount
    {
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [Dependency, NoRender]
        public ISystemDealLogService SystemDealLogService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency, NoRender]
        public Site HostSite { get; set; }

        [Dependency, NoRender]
        public LogHelper Logger { get; set; }

        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService{ get; set; }
        [Dependency, NoRender]
        public TransactionHelper TransactionHelper { get; set; }

        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        private string _accountName;

        public string OldAccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }

        private string _newAccountName;

        public string AccountName
        {
            get { return _newAccountName.TrimSafty(); }
            set { _newAccountName = value; }
        }

        public AccountServiceResponse Save()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            var newAccount = AccountService.GetByName(AccountName);
            if (newAccount == null || newAccount.State != AccountStates.Ready)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);

            var oldAccount = AccountService.GetByName(OldAccountName);
            if (oldAccount == null ||
                (oldAccount.State != AccountStates.Normal && oldAccount.State != AccountStates.Invalid))
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            oldAccount.Remark1 ="Ô­¿¨ºÅ£º"+newAccount.Name;
            oldAccount.Name = newAccount.Name;
            oldAccount.AccountToken = newAccount.AccountToken;
            oldAccount.Password = newAccount.Password;
            oldAccount.PasswordSalt = newAccount.PasswordSalt;
            var owner = (AccountUser)(oldAccount.OwnerId.HasValue ? MembershipService.GetUserById(oldAccount.OwnerId.Value) : null);

            Logger.LogWithSerialNo(LogTypes.AccountChangeName, serialNo, oldAccount.AccountId, OldAccountName, AccountName);
            var fee = HostSite.ChangeCardFee;
            if (fee.HasValue)
            {
                SystemDealLogService.Create(new SystemDealLog(serialNo, SecurityHelper.GetCurrentUser().CurrentUser)
                                                   {
                                                       Addin = oldAccount.AccountId.ToString(),
                                                       Amount = fee.Value,
                                                       DealType = SystemDealLogTypes.ChangeCard
                                                   });
                oldAccount.ChargingAmount += fee.Value;
                CashDealLogService.Create(new CashDealLog(fee.Value, 0, SecurityHelper.GetCurrentUser().CurrentUser.UserId, CashDealLogTypes.ChangeCard));
            }
            AccountService.Delete(newAccount);
            
            AccountService.Update(oldAccount);
            var response = new AccountServiceResponse(ResponseCode.Success, null, ShopService.GetById(oldAccount.ShopId), oldAccount, owner);
            var accountType = AccountTypeService.GetById(oldAccount.AccountTypeId);

            if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountChangeName))
            {
                if (owner != null && owner.IsMobileAvailable)
                {
                    
                    var msg = MessageFormator.Format(HostSite.MessageTemplateOfAccountChangeName, owner);
                    SmsHelper.Send(owner.Mobile, msg);
                }
            }

            if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfChangeAccountName))
            {
                var msg = MessageFormator.FormatTickForChangeAccountName(
                    HostSite.TicketTemplateOfChangeAccountName,
                    HostSite,
                    serialNo,
                    OldAccountName,
                    oldAccount,
                    owner,
                    accountType,
                    SecurityHelper.GetCurrentUser().CurrentUser);
                PrintTicketService.Create(new PrintTicket(LogTypes.AccountChangeName, serialNo, msg, oldAccount));
                response.CodeText = msg;
            }

            return TransactionHelper.CommitAndReturn(response);
        }
    }
}