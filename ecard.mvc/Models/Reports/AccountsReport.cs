using System.Collections.Generic;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class AccountsReport : EcardModelReportRequest
    {
        protected override void OnReady()
        {
        }

        public AccountsReport()
        {
            OrderBy = "AccountType";
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("AccountsExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}