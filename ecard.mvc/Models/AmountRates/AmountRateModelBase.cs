using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AmountRates
{
    public class AmountRateModelBase : ViewModelBase
    {
        private AmountRate _innerObject;

        public AmountRateModelBase()
        {
            _innerObject = new AmountRate();
        }
         

        [NoRender]
        public AmountRate InnerObject
        {
            get { return _innerObject; }
        }

        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }

        protected void SetInnerObject(AmountRate item)
        {
            _innerObject = item;
        }

        private Bounded _accountLevelBounded;

        public Bounded AccountLevel
        {
            get
            {
                if (_accountLevelBounded == null)
                {
                    _accountLevelBounded = Bounded.CreateEmpty("AccountLevelId", InnerObject.AccountLevel);
                }
                return _accountLevelBounded;
            }
            set { _accountLevelBounded = value; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }

        public decimal? Rate
        {
            get { return InnerObject.Rate.HasValue ? InnerObject.Rate * 100m : null; }
            set
            {
                if (value != null)
                    InnerObject.Rate = (value / 100m);
                else
                    InnerObject.Rate = null;
            }
        }
        public decimal? Amount
        {
            get { return InnerObject.Amount; }
            set
            {
                InnerObject.Amount = value;
            }
        }
        [Range(1, int.MaxValue)]
        public int Days
        {
            get { return InnerObject.Days; }
            set
            {
                InnerObject.Days = value;
            }
        }

        [Dependency]
        [NoRender]
        public IAmountRateService AmountRateService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelService { get; set; }


    }
}