using System;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class CreateAccountReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; } 
        protected override void OnReady()
        {
            SetParameter("LogType", LogTypes.AccountCreate);
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
        }

        public CreateAccountReport()
        {
            OrderBy = "AccoutTypeName";
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("CreateAccountExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}