using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity; 
namespace Ecard.Mvc.Models.Accounts
{
    public class ResumeAccount : ViewModelBase
    {
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }
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
                if (account != null && account.State == AccountStates.Invalid)
                {
                    account.State = AccountStates.Normal;
                    AccountService.Update(account);

                    Logger.LogWithSerialNo(LogTypes.AccountResume, serialNo, Id, account.Name);
                    DataAjaxResult r = new DataAjaxResult(); 
                    if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountResume))
                    {
                        var owner = account.OwnerId.HasValue ? MembershipService.GetUserById(account.OwnerId.Value) : null;
                        if (owner != null && owner.IsMobileAvailable)
                        {
                            var accountType = AccountTypeService.GetById(account.AccountTypeId);
                            if (accountType != null && accountType.IsSmsResume)
                            {
                                var msg = MessageFormator.Format(HostSite.MessageTemplateOfAccountResume, owner);
                                msg = MessageFormator.Format(msg, account);
                                SmsHelper.Send(owner.Mobile, msg);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfResumeAccount))
                    {
                        r.Data1 = MessageFormator.FormatTickForResumeAccount(
                            HostSite.TicketTemplateOfResumeAccount, 
                            serialNo,
                            HostSite,
                            account,
                            account.OwnerId.HasValue ? MembershipService.GetUserById(account.OwnerId.Value) : null,
                            AccountTypeService.GetById(account.AccountTypeId),
                            SecurityHelper.GetCurrentUser().CurrentUser);
                        PrintTicketService.Create(new PrintTicket(LogTypes.AccountResume, serialNo, r.Data1.ToString(), account));
                    }
                    return TransactionHelper.CommitAndReturn(r);
                }
                return new SimpleAjaxResult(Localize("accountNoExisting"));
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.AccountResume, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }

        public void Ready()
        {
        }
    }
}