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
    public class SqlWindowTicketingService : IWindowTicketingService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "WindowTicketings";
        public SqlWindowTicketingService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Insert(WindowTicketing item)
        {
            _databaseInstance.Insert(item,TableName);
        }

        public DataTables<WindowTicketings> Query(WindowTicketingRequest request)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@admissionTicketId",request.admissionTicketId),
                                      new SqlParameter("@shopId",request.shopId),
                                      new SqlParameter("@Bdate",request.Bdate),
                                      new SqlParameter("@Edate",request.Edate),
                                      new SqlParameter("@mobile",request.mobile),
                                      new SqlParameter("@payType",request.payType),
                                      new SqlParameter("@shopName",request.shopName),
                                      new SqlParameter("@pageIndex",request.pageIndex),
                                      new SqlParameter("@pageSize",request.pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getWindowTicketings", param);
            return _databaseInstance.GetTables<WindowTicketings>(sp);
        }
    }
}
