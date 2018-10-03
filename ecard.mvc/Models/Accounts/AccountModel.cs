using System;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Accounts
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class AccountModel
    {
        private string _distributorName;
        public string AccountName
        {
            get { return _distributorName.TrimSafty(); }
            set { _distributorName = value; }
        }

        private string _accountToken;

        public string AccountToken
        {
            get { return _accountToken.TrimSafty(); }
            set { _accountToken = value; }
        }
        public Account Account { get; set; }
        public AccountUser Owner { get; set; }
        public void Ready()
        {
            this.Account = AccountService.GetByName(AccountName);
            if (Account != null && !string.Equals(Account.AccountToken, AccountToken, StringComparison.OrdinalIgnoreCase))
                Account = null;

            if (Account != null && Account.OwnerId.HasValue)
                Owner = (AccountUser)MembershipService.GetUserById(Account.OwnerId.Value);
        }
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
    }
}