using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Reports
{
    public class RechargingListReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
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
        protected override void OnReady()
        {
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
            SetParameter("accountShopId",  (int) this.AccountShop == Globals.All ? null : (int?) this.AccountShop);
            var q = (from x in ShopService.Query(new ShopRequest() {State = ShopStates.Normal, IsBuildIn = false})
                    select new IdNamePair {Key = x.ShopId, Name = x.FormatedName}).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            AccountShop.Bind(q, true);
        }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        public RechargingListReport()
        {
            OrderBy = "SystemDealLogId";
        } 
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("RechargingListExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}