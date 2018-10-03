using System;
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
    public class RenewAccount
    {
        [Dependency]
        [NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency]
        [NoRender]
        public ISystemDealLogService SystemDealLogService { get; set; }
        [Dependency]
        [NoRender]
        public LogHelper Logger { get; set; }
        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }
        [Dependency]
        [NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService{ get; set; }
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency]
        [NoRender]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        [NoRender]
        public Site HostSite { get; set; }
        private string _accountName;

        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }
        public AccountServiceResponse Save()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();

            var account = AccountService.GetByName(AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            if (account.State != AccountStates.Normal)
                return new AccountServiceResponse(ResponseCode.AccountStateInvalid);
            AccountUser owner = null;
            if (account.OwnerId.HasValue)
                owner = MembershipService.GetUserById(account.OwnerId.Value) as AccountUser;
            var accountType = AccountTypeService.GetById(account.AccountTypeId);
            bool isRenew = false;
            int renewalMonth = 6;
            if (accountType != null)
            {
                isRenew = accountType.IsRenew;
                renewalMonth = accountType.RenewMonths;
            }
            if (!isRenew)
                return new AccountServiceResponse(ResponseCode.NonRenewal);
            var now = DateTime.Now;
            account.ExpiredDate = (now > account.ExpiredDate ? now : account.ExpiredDate).AddMonths(renewalMonth);
            AccountService.Update(account);
            Logger.LogWithSerialNo(LogTypes.AccountRenew, serialNo, account.AccountId, AccountName, accountType.RenewMonths);
            if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountRenew))
            {
                if (owner != null && owner.IsMobileAvailable)
                {
                    if (accountType.IsSmsRenew)
                    {
                        var msg = MessageFormator.Format(HostSite.MessageTemplateOfAccountRenew, owner);
                        msg = MessageFormator.Format(msg, account);
                        SmsHelper.Send(owner.Mobile, msg);
                    }
                }
            }
            var response = new AccountServiceResponse(ResponseCode.Success, null, ShopService.GetById(account.ShopId), account, owner);
            if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfRenewAccount))
            {
                var msg = MessageFormator.FormatTickForRenewAccount(HostSite.TicketTemplateOfRenewAccount, serialNo, HostSite, account, owner, accountType,
                                                                    SecurityHelper.GetCurrentUser().CurrentUser);
                response.CodeText = msg;
                PrintTicketService.Create(new PrintTicket(LogTypes.AccountRenew, serialNo, msg, account));
            }
            return TransactionHelper.CommitAndReturn(response);
        }
    }
}