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

namespace Ecard.Mvc.Models.Liquidates
{
    public class ListLiquidates : EcardModelListRequest<ListLiquidate>
    {
        public ListLiquidates()
        {
            OrderBy = "LiquidateId";
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
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }

        public void Query()
        {
            int? shopId = null;
            if (this.Shop != Globals.All)
                shopId = Shop;
            var query = this.LiquidateService.QueryShopLiquidate(shopId, LiquidateStates.Processing);
            // fill condition
            List = query.ToList(this, x => new ListLiquidate(x));
            var shopIds = List.Select(x => x.InnerObject.ShopId).ToArray();
            var shops = ShopService.Query(new ShopRequest() { State = ShopStates.Normal }).ToList();
            Shop.Bind(shops.Select(x => new IdNamePair { Key = x.ShopId, Name = string.Format("{0} - {1}", x.Name, x.DisplayName) }).ToArray(), true);
            List.Merge(shops, (x, y) => x.InnerObject.ShopId == y.ShopId,
                       (x, y) =>
                       x.ShopName = (y.FirstOrDefault() == null ? "" : (y.FirstOrDefault().DisplayName)));

        }

    }
}
