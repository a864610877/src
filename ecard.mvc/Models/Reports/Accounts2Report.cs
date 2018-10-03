using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class Accounts2Report : EcardModelReportRequest
    {
        [DataType(DataType.Date)]
        public DateTime? Start { get; set; }
        [DataType(DataType.Date)]
        public DateTime? End { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
        }

        public Accounts2Report()
        {
            OrderBy = "AccountTypeName";
            Start = DateTime.Now.Date;
            End = DateTime.Now.Date;

            PageSize = 100;
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Accounts2Export", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}