using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class AccountMonthReportService : DailyReportService, IBackgroundService
    { 
        protected override void OnExecute(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1);

            // 1, pay, 4 doneprepay, 3, cancel, 6, canceldoneprepay
            var sql =
                string.Format(
                    @"select 0 ShopMonthReportId, convert(varchar(50), DatePart(yyyy, submittime)) + '-' + convert(varchar(50), DatePart(mm, submittime)) as month, accountId,
                                               accountName, sum(amount) dealamount from deallogs 
                                               where (state <> {0}) and submittime >= @start and submittime < @end
                                               group by accountName, convert(varchar(50), DatePart(yyyy, submittime)) + '-' + convert(varchar(50), DatePart(mm, submittime)), accountId",
                    DealLogStates.Normal_);

            var parameters = new
            {
                start = start,
                end = end,
            };
            var reports = DatabaseInstance.Query<AccountMonth>(sql, parameters).ToList();
            reports.ForEach(x => x.Month = start.Date.ToString("yyyy-MM"));
            DatabaseInstance.ExecuteNonQuery("delete ReportAccountMonth where month = '" + start.Date.ToString("yyyy-MM") + "'", null);
            foreach (var report in reports)
            {
                DatabaseInstance.Insert(report, "ReportAccountMonth");
            }
        }

        public AccountMonthReportService(DatabaseInstance databaseInstance) : base(databaseInstance)
        {
        }
    }
}