using System;
using System.Collections.Generic;
using Ecard.Mvc.ViewModels;
using System.ComponentModel.DataAnnotations; 
namespace Ecard.Mvc.Models.Reports
{
    public class ShopDealsReport : EcardModelReportRequest
    {

        public DateTime? DayMin { get; set; }
        public DateTime? DayMax { get; set; }
        [UIHint("Text")]
        public string ShopName { get; set; }
        [UIHint("Text")]
        public string ShopDisplayName { get; set; }
        protected override void OnReady()
        {
            SetParameter("DayMin", DayMin);
            SetParameter("DayMax", DayMax);
            SetParameter("ShopName", string.IsNullOrEmpty(ShopName) ? null : ShopName);
            SetParameter("ShopDisplayName", string.IsNullOrEmpty(ShopDisplayName) ? null : ShopDisplayName);
        }

        public ShopDealsReport()
        {
            OrderBy = "ShopName";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("ShopDealsExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}