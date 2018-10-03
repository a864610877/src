using System;
using System.Collections.Generic;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class SystemDealLogByUserReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", (Start ?? DateTime.Now).Date);
            SetParameter("End", (End ?? DateTime.Now).AddDays(1).Date);
        }

        public SystemDealLogByUserReport()
        {
            OrderBy = "UserName";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("SystemDealLogByUserExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}