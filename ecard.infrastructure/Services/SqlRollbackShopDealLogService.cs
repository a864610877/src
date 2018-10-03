using System.Linq;
using Ecard.Models;

namespace Ecard.Services
{
    public class SqlRollbackShopDealLogService : IRollbackShopDealLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "RollbackShopDealLogs";

        public SqlRollbackShopDealLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<RollbackShopDealLog> Query(RollbackShopDealLogRequest request)
        {
            var sql = "select * from " + TableName + " where 1 = 1 ";
            return new QueryObject<RollbackShopDealLog>(_databaseInstance, sql, request);
        }

        public void Create(RollbackShopDealLog item)
        {
            item.RollbackShopDealLogId = _databaseInstance.Insert(item, TableName);
        }

        public RollbackShopDealLog GetById(int id)
        {
            return _databaseInstance.GetById<RollbackShopDealLog>(TableName, id);
        }

        public void Update(RollbackShopDealLog item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(RollbackShopDealLog item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public QueryObject<RollbackShopDealLog> Query(int? shopId, int? state)
        {
            var sql = "select * from " + TableName + " where (@shopId  is null or shopId = @shopId) and (@state is null or state = @state) ";
            return new QueryObject<RollbackShopDealLog>(_databaseInstance, sql, new { shopId = shopId, state = state });
        }

        public RollbackShopDealLog GetByShopDealLogId(int shopDealLogId)
        {
            var sql = "select * from " + TableName + " where shopDealLogId = @shopDealLogId";
            return new QueryObject<RollbackShopDealLog>(_databaseInstance, sql, new { shopDealLogId = shopDealLogId }).FirstOrDefault();
        }
    }
}