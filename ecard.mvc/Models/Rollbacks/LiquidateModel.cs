using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Rollbacks
{
    public class RollbackModel : ICommandProvider
    {
        [Hidden]
        public int Id { get; set; }

        public string SerialServerNo { get; set; }
        public decimal Amount { get; set; }
        public string AccountName { get; set; }
        public string PosName { get; set; }
        public string ShopName { get; set; }
        public string ShopDisplayName { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime DealSubmitTime { get; set; }
        public string State { get; set; }
        public string DealType { get; set; }

        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency, NoRender]
        public IRollbackShopDealLogService RollbackService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency, NoRender]
        public Site HostSite { get; set; }
        [NoRender]
        public RollbackShopDealLog Rollback { get; set; }

        private bool _hasError;
        public void Ready()
        {
            _hasError = true;

            Rollback = RollbackService.GetById(Id);
            State = ModelHelper.GetBoundText(Rollback, x => x.State);
            var currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
            var shop = ShopService.GetById(Rollback.ShopId);

            var shopDealLog = ShopDealLogService.GetById(Rollback.ShopDealLogId);
            this.SubmitTime = Rollback.SubmitTime;
            this.DealSubmitTime = shopDealLog.SubmitTime;
            this.Amount = shopDealLog.Amount;
            this.AccountName = shopDealLog.AccountName;
            this.PosName = shopDealLog.SourcePosName;
            this.ShopName = shop.Name;
            this.ShopDisplayName = shop.DisplayName;

            SerialServerNo = shopDealLog.SerialServerNo;
            DealType = ModelHelper.GetBoundText(shopDealLog, x => DealType);
            _hasError = false;
        }

        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            if (!_hasError)
            {
                if (this.Rollback.State == RollbackShopDealLogState.Processing)
                {
                    yield return new ActionMethodDescriptor("done", null, new { id = this.Id });
                    yield return new ActionMethodDescriptor("delete", null, new { id = this.Id });
                }
            }
        }
    }
}