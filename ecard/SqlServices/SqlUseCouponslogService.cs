using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlUseCouponslogService : IUseCouponslogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "UseCouponslog";
        public SqlUseCouponslogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Create(UseCouponslog item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }
    }
}
