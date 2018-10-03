using System;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealOnlines
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class DealDetail
    {
        public void Init(AccountServiceResponse rsp, int dealType)
        {
            Code = rsp.Code;
            Error = ModelHelper.GetBoundText(this, x => x.Code);
            if (Code == ResponseCode.Success)
            {
                var dealLog = DealLogService.GetById(Convert.ToInt32(rsp.SerialServerNo));
                ShopName = dealLog.ShopName;
                PosName = dealLog.SourcePosName;
                AccountName = dealLog.AccountName;
                Amount = dealLog.Amount;
                SerialNo = dealLog.SerialNo;
                ServerSerialNo = dealLog.SerialServerNo;

                if (Code == ResponseCode.Success)
                {
                    var account = AccountService.GetByName(AccountName);
                    var shop = ShopService.GetByName(ShopName);
                    var pos = PosService.GetById(dealLog.SourcePosId);
                    var currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
                    switch (dealType)
                    {
                        case DealTypes.Deal:

                            if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfDeal))
                            {
                                this.Ticket = MessageFormator.FormatTickForDeal(HostSite.TicketTemplateOfDeal, HostSite, dealLog, account, shop, pos, currentUser);
                            }
                            break;
                        case DealTypes.CancelDeal:
                            if (!string.IsNullOrWhiteSpace(HostSite.TicketTemplateOfCancelDeal))
                            {
                                this.Ticket = MessageFormator.FormatTickForDeal(HostSite.TicketTemplateOfCancelDeal, HostSite, dealLog, account, shop, pos, currentUser);
                            }
                            break;
                    }
                }
            }
        }

        public string Ticket { get; set; }

        public string ShopName { get; set; }
        public string ServerSerialNo { get; set; }
        public string AccountName { get; set; }
        public string PosName { get; set; }
        public decimal Amount { get; set; }
        public string SerialNo { get; set; }

        public string Error { get; set; }
        [Bounded(typeof(ResponseCode))]
        public int Code { get; set; }

        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public IPosEndPointService PosService { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        public Site HostSite { get; set; }
    }
}