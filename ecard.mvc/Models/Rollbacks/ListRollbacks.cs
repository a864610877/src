using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Collections;

namespace Ecard.Mvc.Models.Rollbacks
{
    public class ListRollbacks : EcardModelListRequest<ListRollback>
    {
        public ListRollbacks()
        {
            OrderBy = "RollbackShopDealLogId";
        }

        private Bounded _shopBounded;

        public Bounded Shop
        {
            get
            {
                if (_shopBounded == null)
                {
                    _shopBounded = Bounded.CreateEmpty("ShopId", Globals.All);
                }
                return _shopBounded;
            }
            set { _shopBounded = value; }
        }
        [Dependency, NoRender]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }

        public void Query()
        {
            int? shopId = null;
            if (this.Shop != Globals.All)
                shopId = Shop;
            List = this.RollbackShopDealLogService.Query(shopId, RollbackShopDealLogState.Processing).ToList(this, x => new ListRollback(x));
            var shopDealLogIds = List.Select(x => x.ShopDealLogId).ToArray();
            var shopDealLogs = ShopDealLogService.GetByIds(shopDealLogIds.ToArray());
            var shops = ShopService.Query(new ShopRequest() { State = ShopStates.Normal }).ToList();

            foreach (var rollback in List)
            {
                var shopDealLog = shopDealLogs.FirstOrDefault(x => x.ShopDealLogId == rollback.ShopDealLogId);
                var shop = shops.FirstOrDefault(x => x.ShopId == shopDealLog.ShopId);
                rollback.Shop = shop;
                rollback.ShopDealLog = shopDealLog;
            }
            Shop.Bind(shops.Select(x => new IdNamePair { Key = x.ShopId, Name = string.Format("{0} - {1}", x.Name, x.DisplayName) }).ToArray(), true);
        }
    }
}
