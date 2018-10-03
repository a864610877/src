using System;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class AddShopLiquidates
    {
        public List<int> Ids { get; set; }

        public SimpleAjaxResult Ready(int? shopId = null)
        {
            if (shopId == null)
            {
                var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
                shopId = user.ShopId;
            }

            int shopid = shopId.Value;
            using (var tran = TransactionHelper.BeginTransaction())
            {
                try
                {
                    DoLiquidate(shopid, Ids);
                }
                catch (Exception ex)
                {
                    return new SimpleAjaxResult(ex.Message);
                }
                tran.Commit();
            }
            return new SimpleAjaxResult();
        }

        public void DoLiquidate(int shopid, List<int> ids)
        {
            var liquidate = this.LiquidateService.ReadyLiquidate(shopid, ids.ToArray());
            if (liquidate.Count == 0)
            {
                throw new Exception("没有可以清算的交易");
            }
            liquidate.DealIds = string.Join(",", ids.ToArray());
            liquidate.ShopId = shopid;
            liquidate.State = LiquidateStates.Processing;

            this.LiquidateService.Insert(liquidate);
            if (liquidate.Count != ShopDealLogService.UpdateLiquidateId(ids, liquidate.LiquidateId, 0, shopid))
            {
                throw new Exception("清算数据冲突");
            }
            var dealLogIds = ShopDealLogService.GetAddins(ids.ToArray());
            if (liquidate.Count != dealLogIds.Count)
            {
                throw new Exception("清算数据冲突");
            }
            if (liquidate.Count != DealLogService.UpdateLiquidateId(dealLogIds, liquidate.LiquidateId, 0, shopid))
            {
                throw new Exception("清算数据冲突");
            }
        }

        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
    }
}