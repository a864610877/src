using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class SystemDealLogDayReport : EcardModelReportRequest
    {
        [DataType(DataType.Date)]
        public DateTime? Start { get; set; }
        [DataType(DataType.Date)]
        public DateTime? End { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", Start);
            var end = (End == null ? DateTime.Now.Date.AddDays(1) : End.Value.AddDays(1));
            SetParameter("End", end > DateTime.Now.Date.AddDays(1) ? DateTime.Now.Date.AddDays(1) : end);
        }


        public SystemDealLogDayReport()
        {
            OrderBy = "OrderNum";
            Start = DateTime.Now.Date;
            End = DateTime.Now.Date;
            this.PageSize = 100;
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("SystemDealLogDayExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}