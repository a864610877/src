using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class AddRallbackShopDealLogs
    {
        public List<int> Ids { get; set; }

        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            var shopId = user.ShopId;

            foreach (var id in Ids)
            {
                using (var tran = TransactionHelper.BeginTransaction())
                {
                    var shopDealLog = ShopDealLogService.GetById(id);
                    if (shopDealLog.State == DealLogStates.Normal_)
                        return new SimpleAjaxResult("该交易已冲正");
                    if (shopDealLog.ShopId != shopId)
                        return new SimpleAjaxResult("只能提交自己商户的申请");

                    var rollback = this.RollbackShopDealLogService.GetByShopDealLogId(shopDealLog.ShopDealLogId);
                    if (rollback != null)
                        return new SimpleAjaxResult("该交易取消请求已经存在");
                    rollback = new RollbackShopDealLog
                                   {
                                       ShopDealLogId = shopDealLog.ShopDealLogId,
                                       ShopId = shopDealLog.ShopId,
                                       State = RollbackShopDealLogState.Processing,
                                   };

                    RollbackShopDealLogService.Create(rollback);
                    tran.Commit();
                }
            }
            return new SimpleAjaxResult();
        }
        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
    }
}