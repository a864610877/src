using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Services;
using Ecard.Models;

namespace Ecard.SqlServices
{
    public class SqlRechargingLogService : IRechargingLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "RechargingLog";
        public SqlRechargingLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Create(Models.RechargingLog item)
        {
            item.RechargingLogId = _databaseInstance.Insert(item, TableName);
        }
        public QueryObject<RechargingLog> Query(string serialNoAll, int pageIndex, int pageSize)
        {
            var sql = @"SELECT * FROM (   
   SELECT ROW_NUMBER() OVER(order by RechargingLogId) AS RowNum,*
      FROM RechargingLog  tt   where (@serialNoAll is null or serialNoAll=@serialNoAll) ) t WHERE  t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize";
            return new QueryObject<RechargingLog>(_databaseInstance, sql, new { serialNoAll = serialNoAll, pageIndex = pageIndex, pageSize = pageSize });
        }

    }
}
