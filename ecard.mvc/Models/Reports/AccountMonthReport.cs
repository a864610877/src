using System;
using System.Collections.Generic;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class AccountMonthReport : EcardModelReportRequest
    {
        public DateTime? DayMin { get; set; }
        public DateTime? DayMax { get; set; }
        public string AccountName { get; set; }
        protected override void OnReady()
        {
            SetParameter("DayMin", DayMin);
            SetParameter("DayMax", DayMax);
            SetParameter("AccountName", string.IsNullOrEmpty(AccountName) ? null : AccountName);
        }

        public AccountMonthReport()
        {
            OrderBy = "Month";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("AccountMonthExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}