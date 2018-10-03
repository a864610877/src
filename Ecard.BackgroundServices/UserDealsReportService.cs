using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class AccountDealsReportService : DailyReportService, IBackgroundService
    {

        public AccountDealsReportService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        }

        protected override void OnExecute(DateTime date)
        {
            var start = date.Date;
            var end = date.Date.AddDays(1);

            // 1, pay, 4 doneprepay, 3, cancel, 6, canceldoneprepay
            var sql =
                string.Format(
                    @"select 0 AccountDealsReportId, AccountId,
                                               isnull((select sum(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and  t1.dealtype = 1 and t1.submittime >= @start and t1.submittime < @end), 0.0) as PayAmount, 
                                               isnull((select count(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and  t1.dealtype = 1 and t1.submittime >= @start and t1.submittime < @end), 0.0) as PayCount, 
                                               isnull((select sum(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and  t1.dealtype = 4 and t1.submittime >= @start and t1.submittime < @end), 0.0) as DonePrepayAmount, 
                                               isnull((select count(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and  t1.dealtype = 4 and t1.submittime >= @start and t1.submittime < @end), 0.0) as DonePrepayCount, 
	                                           isnull((select sum(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and t1.dealtype = 2 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelAmount,
	                                           isnull((select count(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and t1.dealtype = 2 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelCount,
	                                           isnull((select sum(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and t1.dealtype = 6 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelDonePrepayAmount,
	                                           isnull((select count(t1.amount) from deallogs t1 where t1.AccountId = t.AccountId and t1.state <> {0} and t1.dealtype = 6 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelDonePrepayCount,
	                                           isnull((select count(*) from deallogs t1 where t1.AccountId = t.AccountId and t1.state = {0} and t1.submittime >= @start and t1.submittime < @end), 0) as UnPayCount,
                                               accountName from deallogs t
                                               where (state <> {0}) and submittime >= @start and submittime < @end
                                               group by accountName, AccountId",
                    DealLogStates.Normal_);

            var parameters = new
            {
                start = start,
                end = end,
            };
            var reports = DatabaseInstance.Query<AccountDeal>(sql, parameters).ToList();
            reports.ForEach(x => x.SubmitDate = start);
            DatabaseInstance.ExecuteNonQuery("delete ReportAccountDeals where submitdate = @submitdate", new { submitdate = start });
            foreach (var report in reports)
            {
                DatabaseInstance.Insert(report, "ReportAccountDeals");
            }
        } 
    }
}