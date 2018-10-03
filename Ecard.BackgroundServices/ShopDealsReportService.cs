using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class ShopDealsReportService : IBackgroundService
    {
        private readonly DatabaseInstance _instance;

        public ShopDealsReportService(  DatabaseInstance instance)
        {
            this._instance = instance;
        }

        public void Execute()
        {
            _instance.BeginTransaction();
            // state preview month data
            var reportSetting = _instance.Query<ReportSetting>("select * from reportsettings where reportType='shopDeals'", null).FirstOrDefault() ??
                                new ReportSetting() { ReportType = "shopDeals", Value = "2009-1-1" };

            var lastTime = DateTime.Parse(reportSetting.Value);
            while (lastTime.Date < DateTime.Now.Date)
            {
                var start = lastTime.Date;
                var end = lastTime.Date.AddDays(1);

                // 1, pay, 4 doneprepay, 3, cancel, 6, canceldoneprepay
                var sql =
                    string.Format(
                                @"select 0 ShopDealsReportId, ShopId,
                                               isnull((select sum(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and  t1.dealtype = 1 and t1.submittime >= @start and t1.submittime < @end), 0.0) as PayAmount, 
                                               isnull((select count(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and  t1.dealtype = 1 and t1.submittime >= @start and t1.submittime < @end), 0.0) as PayCount, 
                                               isnull((select sum(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and  t1.dealtype = 4 and t1.submittime >= @start and t1.submittime < @end), 0.0) as DonePrepayAmount, 
                                               isnull((select count(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and  t1.dealtype = 4 and t1.submittime >= @start and t1.submittime < @end), 0.0) as DonePrepayCount, 
	                                           isnull((select sum(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = 2 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelAmount,
	                                           isnull((select count(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = 2 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelCount,
	                                           isnull((select sum(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = 6 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelDonePrepayAmount,
	                                           isnull((select count(t1.amount) from deallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = 6 and t1.submittime >= @start and t1.submittime < @end), 0.0) as CancelDonePrepayCount,
	                                           isnull((select count(*) from deallogs t1 where t1.shopId = t.shopId and t1.state = {0} and t1.submittime >= @start and t1.submittime < @end), 0) as UnPayCount,
                                               isnull((select sum(t1.amount) from shopdeallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = {1} and t1.submittime >= @start and t1.submittime < @end), 0.0) as ShopDealLogDoneAmount,
                                               isnull((select sum(t1.amount) from shopdeallogs t1 where t1.shopId = t.shopId and t1.state <> {0} and t1.dealtype = {2} and t1.submittime >= @start and t1.submittime < @end), 0.0) as ShopDealLogChargeAmount,
	                                           name shopName from shops t
                                               group by Name, ShopId",
                        DealLogStates.Normal_, CashDealLogTypes.ShopDealLogDone, CashDealLogTypes.ShopDealLogCharging);

                var parameters = new
                {
                    start = start,
                    end = end,
                };
                var reports = _instance.Query<ShopDeal>(sql, parameters).ToList();
                reports.ForEach(x => x.SubmitDate = start);
                _instance.ExecuteNonQuery("delete ReportShopDeals where submitdate = @submitdate", new { submitdate = start });
                foreach (var report in reports)
                {
                    _instance.Insert(report, "ReportShopDeals");
                }
                lastTime = (DateTime.Now.Date < end) ? DateTime.Now.Date : end;
                reportSetting.Value = lastTime.ToString("yyyy-MM-dd");
                if (reportSetting.ReportSettingId == 0)
                    reportSetting.ReportSettingId = _instance.Insert(reportSetting, "reportsettings");
                else
                    _instance.Update(reportSetting, "reportsettings");
            }
            _instance.Commit();
        }
    }
}