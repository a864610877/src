using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Accounts
{
    public class TransferAccount : ViewModelBase
    {
        public void Ready()
        {
        }

        public string AccountName { get; set; }
        public string AccountNameTo { get; set; }
        public string Password { get; set; }
        public decimal Amount { get; set; }

        [Dependency, NoRender]
        public IPointRebateService PointRebateService { get; set; }
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }  
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }

        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; } 
        public SimpleAjaxResult Save()
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                if (Amount < 0)
                {
                    return new SimpleAjaxResult(Localize("invalidAmount"));
                }
                TransactionHelper.BeginTransaction();
                var account1 = AccountService.GetByName(AccountName);
                if (account1 == null || (account1.State != AccountStates.Normal && account1.State != AccountStates.Invalid))
                {
                    return new SimpleAjaxResult(string.Format(Localize("accountNonFound"), AccountName));
                }
                var account2 = AccountService.GetByName(AccountNameTo);
                if (account2 == null || (account2.State != AccountStates.Normal && account2.State != AccountStates.Invalid))
                {
                    return new SimpleAjaxResult(string.Format(Localize("accountNonFound"), AccountNameTo));
                }

                var accountType = AccountTypeService.GetById(account1.AccountTypeId);
                if (accountType == null || !accountType.IsRecharging)
                {
                    return new SimpleAjaxResult(string.Format(Localize("accountCannotRecharging"), AccountName));
                }
                accountType = AccountTypeService.GetById(account2.AccountTypeId);
                if (accountType == null || !accountType.IsRecharging)
                {
                    return new SimpleAjaxResult(string.Format(Localize("accountCannotRecharging"), AccountNameTo));
                }
                if (Amount == 0)
                {
                    Amount = account1.Amount;
                }

                if (account1.Amount < Amount)
                    return new SimpleAjaxResult(Localize("invalidAmount"));

                var passSvc = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                var password = passSvc.Decrypto(Password);
                if (User.SaltAndHash(password, account1.PasswordSalt) != account1.Password)
                    return new SimpleAjaxResult(Localize("invalidPassword", "√‹¬Î¥ÌŒÛ"));
                account1.Amount -= Amount;
                account2.Amount += Amount;
                AccountService.Update(account1);
                AccountService.Update(account2);

                // transfer in
                DealLog dealLog = new DealLog(serialNo);
                dealLog.Account = account1;
                dealLog.Addin = account1.AccountId;
                dealLog.Amount = Amount;
                dealLog.DealType = DealTypes.TransferOut;
                DealLogService.Create(dealLog);

                // transfer out
                dealLog = new DealLog(serialNo);
                dealLog.Account = account2;
                dealLog.Addin = account2.AccountId;
                dealLog.Amount = -Amount;
                dealLog.DealType = DealTypes.TransferIn;
                DealLogService.Create(dealLog);

                Logger.LogWithSerialNo(LogTypes.AccountTransfer,serialNo, account1.AccountId, account1.Name, account2.Name, Amount);
                var r = new DataAjaxResult();
                if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfTransfer))
                {
                    r.Data1 = MessageFormator.FormatTickForTransfer(HostSite.TicketTemplateOfTransfer, serialNo,
                        account1,  
                        account1.OwnerId.HasValue ? MembershipService.GetUserById(account1.OwnerId.Value) : null,
                        AccountTypeService.GetById(account1.AccountTypeId),
                        account2,
                        account2.OwnerId.HasValue ? MembershipService.GetUserById(account2.OwnerId.Value) : null,
                        AccountTypeService.GetById(account2.AccountTypeId),
                        SecurityHelper.GetCurrentUser().CurrentUser
                        );
                    PrintTicketService.Create(new PrintTicket(LogTypes.AccountTransfer,serialNo, r.Data1.ToString(), account1));
                }
                return TransactionHelper.CommitAndReturn(r);
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.AccountTransfer, ex);

                return new SimpleAjaxResult(Localize("SystemError"));
            }
        }
    }
}