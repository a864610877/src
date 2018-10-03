using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class ChangeSelfPasswordAccount : ViewModelBase
    {
        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }
        
        private string _accountName;

        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
        }

        private string _password;
        [DataType(DataType.Password)]
        public string Password
        {
            get { return _password.TrimSafty(); }
            set { _password = value; }
        }

        private string _passwordConfirm;
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm
        {
            get { return _passwordConfirm.TrimSafty(); }
            set { _passwordConfirm = value; }
        }

        public IMessageProvider Save( )
        {
            Account account = null;


            var currentUser = SecurityHelper.GetCurrentUser();
            AccountUserModel accountUser = currentUser as AccountUserModel;
            if (accountUser != null)
            {
                account = accountUser.Accounts.FirstOrDefault();
            } 
            if (account == null || (account.State != AccountStates.Normal && account.State != AccountStates.Invalid))
            {
                AddError(LogTypes.AccountChangePassword, "accountNoExisting");
                return this;
            }
            account.SetPassword(Password);
            TransactionHelper.BeginTransaction();
            AccountService.Update(account);
            Logger.LogWithSerialNo(LogTypes.AccountChangePassword, SerialNoHelper.Create(), account.AccountId, AccountName);
            AddMessage("success");
            return TransactionHelper.CommitAndReturn(this);
        }

        public void Ready()
        {
            var currentUser = SecurityHelper.GetCurrentUser();
            var accountUser = currentUser as AccountUserModel;
            if (accountUser != null)
            {
                this._accountName = accountUser.Accounts.First().Name;
            } 
        }
    }
}