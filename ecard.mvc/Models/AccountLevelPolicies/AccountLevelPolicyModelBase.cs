using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.AccountTypes;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.AccountLevelPolicies
{
    public class AccountLevelPolicyModelBase : ViewModelBase
    {
        private AccountLevelPolicy _innerObject;

        public AccountLevelPolicyModelBase()
        {
            _innerObject = new AccountLevelPolicy();
        }

        public AccountLevelPolicyModelBase(AccountLevelPolicy shop)
        {
            _innerObject = shop;
        }

        [NoRender]
        public AccountLevelPolicy InnerObject
        {
            get { return _innerObject; }
        }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(40)]
        public string DisplayName
        {
            get { return _innerObject.DisplayName; }
            set { _innerObject.DisplayName = value.TrimSafty(); }
        }
        protected void OnSave(AccountLevelPolicy level)
        {
            level.DiscountRate = InnerObject.DiscountRate;
        }

        public decimal TotalPointStart
        {
            get { return InnerObject.TotalPointStart; }
            set { InnerObject.TotalPointStart = value; }
        }
        public decimal DiscountRate
        {
            get { return InnerObject.DiscountRate * 100; }
            set { InnerObject.DiscountRate = value / 100; }
        }
        protected void SetInnerObject(AccountLevelPolicy item)
        {
            _innerObject = item;
        }
    }
}