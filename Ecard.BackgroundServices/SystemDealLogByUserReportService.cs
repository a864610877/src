using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class SystemDealLogByUserReportService : DailyReportService, IBackgroundService
    {

        public SystemDealLogByUserReportService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        } 
        protected override void OnExecute(DateTime date)
        {
            DateTime start = date.Date;
            DateTime end = date.Date.AddDays(1);

            string sql =
                string.Format(
                    @"select 0 as SystemDealLogByUserId, userId, @start as SubmitDate, sum(amount) as SumAmount, avg(amount) as AvgAmount, count(*) as [Count], DealType from systemdeallogs where state = 1 and submittime >= @start and submittime < @end group by userid, dealtype ");

            List<SystemDealLogByUser> reports = DatabaseInstance.Query<SystemDealLogByUser>(sql, new { start, end }).ToList();
            DatabaseInstance.ExecuteNonQuery(
                "delete ReportSystemDealLogByUser where submitdate = @start", new { start });
            foreach (SystemDealLogByUser report in reports)
            {
                DatabaseInstance.Insert(report, "ReportSystemDealLogByUser");
            }
        } 
    }
}