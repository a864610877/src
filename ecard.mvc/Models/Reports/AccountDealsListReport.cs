using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Reports
{
    public class AccountDealsListReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        private Bounded _shopBounded;
        [NoRender, Dependency]
        public IShopService ShopService { get; set; }
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

        protected override void OnReady()
        {
            int shopId = this.Shop;

            var q = (ShopService.Query(new ShopRequest() { State = ShopStates.Normal }).ToList().Select(x => new IdNamePair { Key = x.ShopId, Name = x.FormatedName })).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            this.Shop.Bind(q, true);

            SetParameter("ShopId", shopId == Globals.All ? null : (int?)shopId);
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
            SetParameter("accountShopId", (int)this.AccountShop == Globals.All ? null : (int?)this.AccountShop);
            q = (from x in ShopService.Query(new ShopRequest() { State = ShopStates.Normal, IsBuildIn = false })
                 select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            AccountShop.Bind(q, true);
        }
        private Bounded _accountShopBounded;

        public Bounded AccountShop
        {
            get
            {
                if (_accountShopBounded == null)
                {
                    _accountShopBounded = Bounded.CreateEmpty("AccountShopId", Globals.All);
                }
                return _accountShopBounded;
            }
            set { _accountShopBounded = value; }
        }

        public AccountDealsListReport()
        {
            OrderBy = "SerialNo";
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("AccountDealsListExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}