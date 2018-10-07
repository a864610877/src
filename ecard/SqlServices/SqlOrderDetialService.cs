using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlOrderDetialService : IOrderDetialService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "fz_OrderDetial";
        public SqlOrderDetialService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Create(OrderDetial item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public OrderDetial GetById(int id)
        {
            return _databaseInstance.GetById<OrderDetial>(TableName, id);
        }

        public void Update(OrderDetial item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public List<OrderDetial> GetOrderNo(string orderNo)
        {
            string sql = "select * from fz_OrderDetial where orderNo=@orderNo";
            return new QueryObject<OrderDetial>(_databaseInstance, sql, new { orderNo = orderNo }).ToList();
        }
    }
}
