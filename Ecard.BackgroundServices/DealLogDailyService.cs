using System;
using System.Data;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard.BackgroundServices
{
    public class DealLogDailyService : DailyReportService, IBackgroundService
    {

        public DealLogDailyService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        } 
        protected override void OnExecute(DateTime date)
        {
            DatabaseInstance.ExecuteNonQuery("CreateDealLogDaily", new { @start = date.Date }, CommandType.StoredProcedure);
        }
    }
}