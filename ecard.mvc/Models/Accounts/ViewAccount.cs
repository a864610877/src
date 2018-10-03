using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Mvc.Models.Accounts
{
    public class ViewAccount : ViewModelBase, ICommandProvider
    {
        private Account _innerObject;
        private AccountUser _owner;

        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }

        public string Name
        {
            get { return _innerObject.Name; }
        }

        public string OwnerName
        {
            get { return _owner == null ? "" : _owner.Name; }
        }

        public string OwnerDisplayName
        {
            get { return _owner == null ? "" : _owner.DisplayName; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(_innerObject, x => x.State); }
        }
        private string _password;
        [DataType(DataType.Password)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _passwordConfirm;
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm
        {
            get { return _passwordConfirm; }
            set { _passwordConfirm = value; }
        }
        protected void SetInnerObject(Account shop, AccountUser owner)
        {
            if (owner != null && owner.SignOnTime == null)
            {
                owner.Name = "";
            }
            _owner = owner;
            _innerObject = shop;
        }

        public void Read(int id)
        {
            Account account = AccountService.GetById(id);
            if (account != null)
            {
                AccountUser owner = (account.OwnerId.HasValue ?
                    MembershipService.GetUserById(account.OwnerId.Value) : null) as AccountUser;
                SetInnerObject(account, owner);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            yield return new ActionMethodDescriptor("ChangeAccountPassword", "Account", new { id = this._innerObject.AccountId });
            //if (this._innerObject.State == AccountStates.Normal)
            //    yield return new ActionMethodDescriptor("Suspend", "Account", new { id = this._innerObject.AccountId });
            //if (this._innerObject.State == AccountStates.Invalid)
            //    yield return new ActionMethodDescriptor("Resume", "Account", new { id = this._innerObject.AccountId });
           // yield return new ActionMethodDescriptor("List", "Account", null);
        }
    }
}