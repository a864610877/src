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
    public class AccountDealsReport : EcardModelReportRequest
    {
        public DateTime? DayMin { get; set; }
        public DateTime? DayMax { get; set; }
        public string AccountName { get; set; }
        protected override void OnReady()
        {
            SetParameter("DayMin", DayMin);
            SetParameter("DayMax", DayMax);
            SetParameter("AccountName", string.IsNullOrEmpty(AccountName) ? null : AccountName);
            SetParameter("accountShopId", (int)this.AccountShop == Globals.All ? null : (int?)this.AccountShop);
            var q = (from x in ShopService.Query(new ShopRequest() { State = ShopStates.Normal, IsBuildIn = false })
                    select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            AccountShop.Bind(q, true);
        }

        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
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
        public AccountDealsReport()
        {
            OrderBy = "AccountName";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("AccountDealsExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}