using System;
using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlSystemDealLogService : ISystemDealLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "SystemDealLogs";

        public SqlSystemDealLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<SystemDealLog> Query(SystemDealLogRequest request)
        {
            return new QueryObject<SystemDealLog>(_databaseInstance, "SystemDealLog.query", request);
        }

        public void Create(SystemDealLog item)
        {
            lock (SiteAccount.Locker)
            {
                //if (!_databaseInstance.InTransaction)
                //    throw new Exception("SystemDealLog have to run in transaction");
                //_databaseInstance.ExecuteNonQuery("update siteaccounts set amount = amount + @amount",
                //                                  new { amount = item.Amount });
                //var siteAccount = _databaseInstance.Query<SiteAccount>("select * from siteaccounts", null).FirstOrDefault();
                //if (siteAccount == null)
                //    throw new Exception("siteaccounts table changed, please check it");
                //item.SiteAmount = siteAccount.Amount ;
                item.SystemDealLogId = _databaseInstance.Insert(item, TableName);
            }
        }

        public SystemDealLog GetById(int id)
        {
            return _databaseInstance.GetById<SystemDealLog>(TableName, id);
        }

        public void Update(SystemDealLog item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(SystemDealLog item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}