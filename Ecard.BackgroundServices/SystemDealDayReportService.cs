using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class SystemDealLogDayReportService : DailyReportService, IBackgroundService
    {

        public SystemDealLogDayReportService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        } 
        protected override void OnExecute(DateTime date)
        {
            DateTime start = date.Date;
            DateTime end = date.Date.AddDays(1);

            // 1, pay, 4 doneprepay, 3, cancel, 6, canceldoneprepay
            string sql =
                string.Format(
                    @"select 0 as SystemDealLogDayId, @start as SubmitDate, sum(amount) as SumAmount, avg(amount) as AvgAmount, count(*) as [Count], DealType from systemdeallogs where state = 1 and submittime >= @start and submittime < @end group by dealtype ");

            List<SystemDealLogDay> reports = DatabaseInstance.Query<SystemDealLogDay>(sql, new { start, end }).ToList();
            DatabaseInstance.ExecuteNonQuery(
                "delete ReportSystemDealLogDay where submitdate = @start", new { start });
            foreach (SystemDealLogDay report in reports)
            {
                DatabaseInstance.Insert(report, "ReportSystemDealLogDay");
            }
        }
    }
}