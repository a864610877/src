using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IPosKeyService
    {
        void Insert(PosKey item);
        void Update(PosKey item);
        void Delete(PosKey item);
        PosKey GetPosKey(string ShopName, string PosName);
    }

    public class SqlPosKeyService : IPosKeyService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "PosKey";

        public SqlPosKeyService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Insert(PosKey item)
        {
            _databaseInstance.Insert(item, TableName);
        }

        public void Update(PosKey item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(PosKey item)
        {
            throw new NotImplementedException();
        }

        public PosKey GetPosKey(string ShopName, string PosName)
        {
            string sql = "select * from PosKey where ShopName=@ShopName and PosName=@PosName";
            return new QueryObject<PosKey>(_databaseInstance, sql, new { ShopName = ShopName, PosName = PosName }).FirstOrDefault();

        }
    }
}
