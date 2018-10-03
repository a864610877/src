using System;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class AllSystemSummaryReport : EcardModelReportRequest
    {
        protected override void OnReady()
        {
        }

        public AllSystemSummaryReport()
        {
            OrderBy = "orderNum";
        } 
    }
}