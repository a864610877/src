using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlAccountTypeService : IAccountTypeService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "AccountTypes";
        public SqlAccountTypeService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<AccountType> Query(AccountTypeRequest request)
        {
            string sql = @"select * from accounttypes where 
(@displayName is null or displayName = @displayName)
and (@state is null or state = @state) ";
            return new QueryObject<AccountType>(_databaseInstance, sql, request);
        }

        public void Create(AccountType item)
        {
            item.AccountTypeId = _databaseInstance.Insert(item, TableName);
        }

        public AccountType GetById(int id)
        {
            return _databaseInstance.GetById<AccountType>(TableName, id);
        }

        public void Update(AccountType item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(AccountType item)
        {
            _databaseInstance.Delete(item, TableName);
        }

    }
}