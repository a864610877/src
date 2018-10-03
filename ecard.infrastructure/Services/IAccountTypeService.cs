using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Moonlit;

namespace Ecard.Services
{
    public interface IAccountTypeService
    {
        QueryObject<AccountType> Query(AccountTypeRequest request);
        void Create(AccountType item);
        AccountType GetById(int id);
        void Update(AccountType item);
        void Delete(AccountType item);
    }

    public class AccountTypeRequest
    {
        public int? State { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName.NullIfEmpty(); }
            set { _displayName = value; }
        } 
    }
}
