using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlTicketOffService : ITicketOffService
    {

        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "TicketOff";
        public SqlTicketOffService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Insert(TicketOff item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public DataTables<TicketOffs> Query(TicketOffRequest request)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@type",request.type),
                                      new SqlParameter("@Bdate",request.Bdate),
                                      new SqlParameter("@Edate",request.Edate),
                                      new SqlParameter("@mobile",request.mobile),
                                      new SqlParameter("@shopDisplayName",request.shopDisplayName),
                                      new SqlParameter("@shopName",request.shopName),
                                      new SqlParameter("@pageIndex",request.pageIndex),
                                      new SqlParameter("@pageSize",request.pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getTicketOff", param);
            return _databaseInstance.GetTables<TicketOffs>(sp);
        }
    }
}
