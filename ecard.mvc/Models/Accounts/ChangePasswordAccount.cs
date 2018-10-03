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
    public class ChangePasswordAccount : ViewModelBase
    {
        [Dependency, NoRender]
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

        public SimpleAjaxResult Save(int accountId)
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                var passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                string password1, password2;
                passwordService.Decrypto(this.Password, this.PasswordConfirm, out password1, out password2);
                if (password1 != password2)
                {
                    return new SimpleAjaxResult(Localize("passwordNotEquals"));
                }
                Account account = AccountService.GetById(accountId);
                if (account == null || (account.State != AccountStates.Normal && account.State != AccountStates.Invalid))
                {
                    return new SimpleAjaxResult(Localize("accountNoExisting"));
                }
                account.SetPassword(password1);
                TransactionHelper.BeginTransaction();
                AccountService.Update(account);
                Logger.LogWithSerialNo(LogTypes.AccountChangePassword, serialNo, account.AccountId, AccountName);
                AddMessage("success");
                TransactionHelper.CommitAndReturn(this);
                return new SimpleAjaxResult();
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.AccountChangePassword, ex);
                return new SimpleAjaxResult(ex.Message);
            }
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