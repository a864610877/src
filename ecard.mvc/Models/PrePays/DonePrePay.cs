using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity; 

namespace Ecard.Mvc.Models.PrePays
{
    public class DonePrePay : ViewModelBase
    {
        [Dependency, NoRender]
        public IPrePayService PrePayService { get; set; }

        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [Dependency, NoRender]
        public IAccountDealService AccountDealService { get; set; }
         
         
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }

        public decimal Amount { get; set; }
        public int Id { get; set; }

        public SimpleAjaxResult Save()
        {
            if (Amount < 0)
                return new SimpleAjaxResult(Localize("invalidAmount"));

            try
            {
                var serialNo = SerialNoHelper.Create();
                TransactionHelper.BeginTransaction();
                var prePay = PrePayService.GetById(Id);
                if (prePay != null && prePay.State == PrePayStates.Processing)
                {
                    if (Amount == 0)
                        Amount = prePay.Amount;
                    var account = AccountService.GetById(prePay.AccountId);
                    if (account == null)
                        return new SimpleAjaxResult(Localize("invalidAccount"));

                    var rsp = AccountDealService.DonePrePay(new PayRequest(prePay.AccountName, "", prePay.ShopName, Amount, serialNo,
                                                                           account.AccountToken, prePay.ShopName, prePay.ShopName) { IsForce = true });
                    if (rsp.Code != ResponseCode.Success)
                        return new SimpleAjaxResult(ModelHelper.GetBoundText(rsp, x => x.Code));

                    Logger.LogWithSerialNo(LogTypes.DonePrePayForce, serialNo, Id, account.Name);
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
                Logger.Error(LogTypes.DonePrePayForce, ex); 
                return new SimpleAjaxResult(ex.Message);
            }
        }

        public void Ready()
        {
        }


    }
}