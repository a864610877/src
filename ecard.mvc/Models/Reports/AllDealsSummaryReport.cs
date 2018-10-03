using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class AllDealsSummaryReport : EcardModelReportRequest
    {
        protected override void OnReady()
        {
        }

        public AllDealsSummaryReport()
        {
            OrderBy = "orderNum";
        } 
    }
}