using System.Collections.Generic;
using Ecard.Models;
using Moonlit;
using Ecard.Infrastructure;

namespace Ecard.Services
{ 
    public interface IAccountService
    {
        QueryObject<Account> Query(AccountRequest request);
        DataTables<AccountWithOwner> NewQuery(AccountRequest Request);
        QueryObject<Account> QueryAccount(string accountName, string accountToken);
        Account GetByName(string accountName);
        Account GetAccountByName(string accountName);
        QueryObject<Account> QueryForName(AccountWithNameRequest request);
        void Create(Account item);
        Account GetById(int id);
        void Update(Account item);
        void Delete(Account item);
        void DisableMessageOfDeal(int accountTypeId);
        QueryObject<AccountWithOwner> QueryAccountWithOwner(AccountRequest request);
        QueryObject<Account> QueryByNames(string[] accountNames);
        IEnumerable<Account> QueryByOwnerId(AccountUser accountUser);
        QueryObject<Account> GetByIds(int[] ids);
    }


    public interface ITaskService
    {
        QueryObject<Task> Query(TaskRequest request);
        void Create(Task item);
        Task GetById(int id);
        void Update(Task item);
        void Delete(Task item);
    }

    public class TaskRequest
    {
        private string _commandTypeName;
        public int? State { get; set; }

        public string CommandTypeName
        {
            get {
                return _commandTypeName.NullIfEmpty();
            }
            set {
                _commandTypeName = value;
            }
        }

        public int? AccountId { get; set; }
    }
}