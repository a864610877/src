using System.Collections.Generic;
using System.Data;

namespace Ecard.Services
{
    public class SqlReportService : IReportService
    {
        private readonly DatabaseInstance _databaseInstance;

        public SqlReportService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public DataTable GetReport(string sql, IDictionary<string, object> parameters, int start, int pageSize, string orderBy)
        { 
            var table = _databaseInstance.Table(sql, parameters, start, pageSize, orderBy);

            table.Columns.Remove("rownum");
            return table;
        }

        public int GetCount(string sql, IDictionary<string, object> parameters)
        {
            return (int)_databaseInstance.Count(sql, parameters);
        }
    }
}