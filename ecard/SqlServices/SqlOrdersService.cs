using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
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
    }
}
