using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Moonlit.Data;

namespace Ecard.Services
{
    public interface IReportService
    {
        DataTable GetReport(string sql, IDictionary<string, object> parameters, int start, int pageSize, string orderBy);
        int GetCount(string sql, IDictionary<string, object> parameters);
    }
}
