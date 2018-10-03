using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class ShopRollbacks
    {
        public List<ListShopRollback> Items { get; set; }
        public void Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            var rollbacks = this.RollbackShopDealLogService.Query(user.ShopId, LiquidateStates.Processing).ToList();
            var shopDeals = this.ShopDealLogService.GetByIds(rollbacks.Select(x => x.ShopDealLogId).ToArray());

            Items = new List<ListShopRollback>();
            foreach (var rollback in rollbacks)
            {
                var shopDealLogItem = shopDeals.FirstOrDefault(x => x.ShopDealLogId == rollback.ShopDealLogId);
                Items.Add(new ListShopRollback(rollback, shopDealLogItem));
            }
        }
        [Dependency]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
    }
}