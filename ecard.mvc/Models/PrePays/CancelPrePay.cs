using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity; 

namespace Ecard.Mvc.Models.PrePays
{
    public class CancelPrePay : ViewModelBase
    {
        [Dependency, NoRender]
        public IPrePayService PrePayService { get; set; }

        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [Dependency, NoRender]
        public IAccountDealService AccountDealService { get; set; }
        
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
          
        public int Id { get; set; }

        public SimpleAjaxResult Save()
        { 
            try
            {
                TransactionHelper.BeginTransaction();
                var prePay = PrePayService.GetById(Id);
                if (prePay != null && prePay.State == PrePayStates.Processing)
                { 
                    var account = AccountService.GetById(prePay.AccountId);
                    if (account == null)
                        return new SimpleAjaxResult(Localize("invalidAccount"));

                    var serialNo = SerialNoHelper.Create();
                    var rsp = AccountDealService.CancelPrePay(new CancelPayRequest(prePay.AccountName, "", "", prePay.Amount, serialNo, "",
                                                                             account.AccountToken, prePay.ShopName) { IsForce = true });
                    if (rsp.Code != ResponseCode.Success)
                        return new SimpleAjaxResult(ModelHelper.GetBoundText(rsp, x => x.Code));
                    Logger.LogWithSerialNo(LogTypes.CancelPrePayForce, serialNo, Id, account.Name);
                    DataAjaxResult r = new DataAjaxResult();
                    //if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfSuspendAccount))
                    //{
                    //    r.Data1 = MessageFormator.FormatTickForSuspendAccount(HostSite.TicketTemplateOfSuspendAccount, HostSite,
                    //                                                          prePay, SecurityHelper.GetCurrentUser().CurrentUser);
                    //    PrintTicketService.Create(new PrintTicket(LogTypes.AccountSuspend, r.Data1.ToString(), prePay));
                    //}
                    return TransactionHelper.CommitAndReturn(r);
                }
                return new SimpleAjaxResult(Localize("invalidDeal"));
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.CancelPrePayForce, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }

        public void Ready()
        {
        }


    }
}