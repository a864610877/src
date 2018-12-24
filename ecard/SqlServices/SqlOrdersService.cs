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
    public class SqlOrdersService : IOrdersService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "fz_Orders";
        public SqlOrdersService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Create(Orders item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public Orders GetById(int id)
        {
            return _databaseInstance.GetById<Orders>(TableName, id);
        }

        public void Update(Orders item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public Orders GetByOrderNo(string orderNo)
        {
            string sql = "select * from fz_Orders where orderNo=@orderNo";
            return new QueryObject<Orders>(_databaseInstance, sql, new { orderNo = orderNo }).FirstOrDefault();
        }

        public DataTables<Ordersss> Query(OrdersRequest request)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@userId",request.userId),
                                      new SqlParameter("@mobile",request.mobile),
                                      new SqlParameter("@orderNo",request.orderNo),
                                      new SqlParameter("@orderState",request.orderState),
                                      new SqlParameter("@type",request.type),
                                      new SqlParameter("@useScope",request.useScope),
                                      new SqlParameter("@Bdate",request.Bdate),
                                      new SqlParameter("@Edate",request.Edate),
                                      new SqlParameter("@pageIndex",request.pageIndex),
                                      new SqlParameter("@pageSize",request.pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getOrders", param);
            return _databaseInstance.GetTables<Ordersss>(sp);
        }
    }
}
