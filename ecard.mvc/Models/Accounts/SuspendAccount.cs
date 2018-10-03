using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Accounts
{
    public class SuspendAccount : ViewModelBase
    {
        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; } 
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
         
        public int Id { get; set; }

        public SimpleAjaxResult Save()
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                TransactionHelper.BeginTransaction();
                var account = AccountService.GetById(Id);
                if (account != null && account.State == AccountStates.Normal)
                {
                    account.State = AccountStates.Invalid;
                    AccountService.Update(account);

                    Logger.LogWithSerialNo(LogTypes.AccountSuspend, serialNo, Id, account.Name); 
                    if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountSuspend))
                    {
                        var owner = account.OwnerId.HasValue ? MembershipService.GetUserById(account.OwnerId.Value) : null;
                        if (owner != null && owner.IsMobileAvailable)
                        {
                            var accountType = AccountTypeService.GetById(account.AccountTypeId);
                            if (accountType != null && accountType.IsSmsSuspend)
                            {
                                var msg = MessageFormator.Format(HostSite.MessageTemplateOfAccountSuspend, owner);
                                msg = MessageFormator.Format(msg, account);
                                SmsHelper.Send(owner.Mobile, msg);
                            }
                        }
                    }
                    DataAjaxResult r = new DataAjaxResult();
                    if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfSuspendAccount))
                    {
                        r.Data1 = MessageFormator.FormatTickForSuspendAccount(HostSite.TicketTemplateOfSuspendAccount, serialNo,
                            HostSite,
                            account,
                            account.OwnerId.HasValue ? MembershipService.GetUserById(account.OwnerId.Value) : null,
                            AccountTypeService.GetById(account.AccountTypeId), SecurityHelper.GetCurrentUser().CurrentUser);
                        PrintTicketService.Create(new PrintTicket(LogTypes.AccountSuspend, serialNo, r.Data1.ToString(), account));
                    }
                    return TransactionHelper.CommitAndReturn(r);
                }
                return new SimpleAjaxResult(Localize("accountNoExisting"));
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.AccountSuspend, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }

        public void Ready()
        {
        }
    }
}