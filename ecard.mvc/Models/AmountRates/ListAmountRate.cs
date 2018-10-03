using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.AmountRates
{
    public class ListAmountRate
    {
        private readonly AmountRate _innerObject;

        [NoRender]
        public AmountRate InnerObject
        {
            get { return _innerObject; }
        }

        public ListAmountRate()
        {
            _innerObject = new AmountRate();
        }

        public ListAmountRate(AmountRate innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int AmountRateId
        {
            get { return InnerObject.AmountRateId; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        public int AccountLevel
        {
            get { return InnerObject.AccountLevel; }
        }

        public int Days
        {
            get { return InnerObject.Days; }
        }

        public decimal? Rate
        {
            get { return InnerObject.Rate.HasValue ? InnerObject.Rate.Value * 100m : (decimal?)null; }
        }
        public decimal? Amount
        {
            get { return InnerObject.Amount; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}