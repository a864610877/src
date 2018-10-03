using System.Collections.Generic;
using Ecard.Models;
using System.Linq;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class ShopDealLogs
    {
        public List<ShopDealLog> Items { get; set; }

        public void Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            var items = this.ShopDealLogService.QueryUnLiquidateDeals(user.ShopId).ToList(0, 500, "SubmitTime").ToList();
            var rollbacks = RollbackShopDealLogService.Query(user.ShopId, null).ToList();
            Items = items.Where(s => !rollbacks.Any(x => x.ShopDealLogId == s.ShopDealLogId)).ToList();
        }
        [Dependency]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
    }
}