using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;
using Ecard.Infrastructure;
using System.Data.SqlClient;

namespace Ecard.Services
{
    public class SqlAccountService : IAccountService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Accounts";
        public SqlAccountService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Account> Query(AccountRequest request)
        {
           // _databaseInstance.GetTables<>()
            return new QueryObject<Account>(_databaseInstance, "account.query", request);
        }
        public DataTables<AccountWithOwner> NewQuery(AccountRequest Request)
        {
            SqlParameter[] param = { 
                                       new SqlParameter("@Name",Request.Name),
                                       new SqlParameter("@ShopId",Request.ShopId),
                                       new SqlParameter("@AccountToken",Request.AccountToken),
                                       new SqlParameter("@Ids",Request.Ids),
                                       new SqlParameter("@AccountTypeId",Request.AccountTypeId),
                                       new SqlParameter("@pageIndex",Request.PageIndex),
                                       new SqlParameter("@pageSize",Request.PageSize),
                                       new SqlParameter("@IsMobileAvailable",Request.IsMobileAvailable),
                                       new SqlParameter("@NameWith",Request.NameWith),
                                       new SqlParameter("@Content",Request.Content),
                                       new SqlParameter("@State",Request.State),
                                       new SqlParameter("@MobileWith",Request.Mobile)

                                   };
            StoreProcedure sp = new StoreProcedure("P_getAccounts", param);
            return _databaseInstance.GetTables<AccountWithOwner>(sp);
        }
        public QueryObject<Account> QueryAccount(string accountName, string accountToken)
        {
            return new QueryObject<Account>(_databaseInstance, "account.QueryAccount", new { accountName = accountName, accountToken = accountToken });
        }

        public Account GetByName(string accountName)
        {
            return new QueryObject<Account>(_databaseInstance, string.Format("select * from  {0} where Name = @accountName", TableName), new { accountName = accountName }).FirstOrDefault();
        }
        public Account GetAccountByName(string accountName)
        {
            return new QueryObject<Account>(_databaseInstance, string.Format("select * from  {0} where Name = @accountName and State=1", TableName), new { accountName = accountName }).FirstOrDefault();
        }
        public QueryObject<AccountWithOwner> QueryAccountWithOwner(AccountRequest request)
        {
            return new QueryObject<AccountWithOwner>(_databaseInstance, "account.queryAccountWithOwner", request);
        }

        public QueryObject<Account> QueryByNames(string[] accountNames)
        {
            return new QueryObject<Account>(_databaseInstance, "account.queryByNames", new { names = accountNames });
        }

        public IEnumerable<Account> QueryByOwnerId(AccountUser accountUser)
        {
            return new QueryObject<Account>(_databaseInstance, "account.QueryByOwnerId", new { ownerId = accountUser.UserId });
        }

        public QueryObject<Account> GetByIds(int[] ids)
        { 
            return new QueryObject<Account>(_databaseInstance, "select * from accounts where accountid in (@ids)", new { ids = ids });
        }

        public QueryObject<Account> QueryForName(AccountWithNameRequest request)
        {
            return new QueryObject<Account>(_databaseInstance, "account.QueryForName", request);
        }

        public void Create(Account item)
        {
            item.AccountId = _databaseInstance.Insert(item, TableName);
        }

        public Account GetById(int id)
        {
            return _databaseInstance.GetById<Account>(TableName, id);
        }

        public void Update(Account item)
        {
            _databaseInstance.Update(item, "Accounts");
        }

        public void Delete(Account item)
        {
            _databaseInstance.Delete(item, TableName);
        }



        public void DisableMessageOfDeal(int accountTypeId)
        {
            _databaseInstance.ExecuteNonQuery("account.DisableMessageOfDeal", new { accountTypeId = accountTypeId });
        }

    }
}