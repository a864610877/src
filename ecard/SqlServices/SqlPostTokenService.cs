using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlPostTokenService : IPostTokenService
    {

        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "PostToken";
        public SqlPostTokenService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public PostToken GetByPosName(string posName)
        {
            return new QueryObject<PostToken>(_databaseInstance, string.Format("select * from  {0} where posName = @posName", TableName), new { posName = posName }).FirstOrDefault();
        }

        public PostToken GetByToken(string token)
        {
            return new QueryObject<PostToken>(_databaseInstance, string.Format("select * from  {0} where token = @token", TableName), new { token = token }).FirstOrDefault();
        }

        public void Insert(PostToken item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public void Update(PostToken item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }


    }
}
