using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class ShopLiquidates
    {
        public List<Liquidate> Items { get; set; }
        public void Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            Items = this.LiquidateService.QueryShopLiquidate(user.ShopId, LiquidateStates.Processing).ToList();
        }
        [Dependency]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
    }
}