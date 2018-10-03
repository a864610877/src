using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Accounts
{

    public class AccountModelBase : ViewModelBase
    {
        private Account _innerObject;

        public AccountModelBase()
        {
            _innerObject = new Account();
        }

        public AccountModelBase(Account shop)
        {
            _innerObject = shop;
        }
        [NoRender]
        public Account InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(Account item)
        {
            _innerObject = item;
        }


        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }
    }
}