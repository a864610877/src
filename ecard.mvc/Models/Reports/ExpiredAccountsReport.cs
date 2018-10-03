using System;
using System.Collections.Generic;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class ExpiredAccountsReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", Start);
        }

        public ExpiredAccountsReport()
        {
            OrderBy = "AccountName";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("ExpiredAccountsExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}