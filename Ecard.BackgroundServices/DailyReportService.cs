using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard.BackgroundServices
{
    public abstract class DailyReportService : IBackgroundService
    {
        private readonly DatabaseInstance _databaseInstance;

        protected DailyReportService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public DatabaseInstance DatabaseInstance
        {
            get { return _databaseInstance; }
        }

        public void Execute()
        {
            var reportSetting = DatabaseInstance.Query<ReportSetting>("select * from reportsettings where reportType=@ReportType",
                                                                       new { ReportType = this.GetType().FullName }).FirstOrDefault();
            if (reportSetting == null)
            {
                reportSetting = new ReportSetting { ReportType = this.GetType().FullName, Value = "2011-6-1", IsEnabled = true };
                reportSetting.ReportSettingId = DatabaseInstance.Insert(reportSetting, "reportsettings");
            }
            if (!reportSetting.IsEnabled)
                return;

            DateTime lastTime = DateTime.Parse(reportSetting.Value).Date;
            while (lastTime.Date < DateTime.Now.Date)
            {
                using (var tran = DatabaseInstance.BeginTransaction())
                {
                    OnExecute(lastTime);
                    reportSetting.Value = lastTime.ToString("yyyy-MM-dd");
                    DatabaseInstance.Update(reportSetting, "reportsettings");
                    tran.Commit();
                }
                lastTime = lastTime.AddDays(1);
            }
        }

        protected abstract void OnExecute(DateTime date);
    }
}