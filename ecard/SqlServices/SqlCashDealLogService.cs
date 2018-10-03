using Ecard.Models;

namespace Ecard.Services
{
    public class SqlCashDealLogSummaryService : ICashDealLogSummaryService
    {
        private readonly DatabaseInstance _databaseInstance;

        public SqlCashDealLogSummaryService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<CashDealLogSummary> Query(CashDealLogRequest request)
        {
            if (request.SubmitTimeMax != null)
                request.SubmitTimeMax = request.SubmitTimeMax.Value.Date.AddDays(1);
            if (request.SubmitTimeMin != null)
                request.SubmitTimeMin = request.SubmitTimeMin.Value.Date;
            return new QueryObject<CashDealLogSummary>(_databaseInstance, "CashDealLogSummary.query", request);
        }
    }
    public class SqlCashDealLogService : ICashDealLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "CashDealLogs";

        public SqlCashDealLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<CashDealLog> Query(CashDealLogRequest request)
        {
            var sql = @"select * from CashDealLogs where (@submittimeMin is null or submittime >= @submittimeMin) 
                            and (@submittimeMax is null or submittime < @submittimeMax) 
";
            if (request.SubmitTimeMax != null)
                request.SubmitTimeMax = request.SubmitTimeMax.Value.Date.AddDays(1);
            if (request.SubmitTimeMin != null)
                request.SubmitTimeMin = request.SubmitTimeMin.Value.Date;
            return new QueryObject<CashDealLog>(_databaseInstance, sql, request);
        }

        public void Create(CashDealLog item)
        {
            item.CashDealLogId = _databaseInstance.Insert(item, TableName);
        }

        public CashDealLog GetById(int id)
        {
            return _databaseInstance.GetById<CashDealLog>(TableName, id);
        }

        public void Update(CashDealLog item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(CashDealLog item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public decimal GetSummary(int ownerId)
        {
            var sql = @"select sum(amount) from CashDealLogs where ownerId = @ownerId";
            var value = _databaseInstance.ExecuteScalar(sql, new { ownerId = ownerId });
            return System.Convert.ToDecimal(System.Convert.IsDBNull(value) ? 0m : value);
        }
    }
}