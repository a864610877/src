using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.PrePays
{
    public class ListPrePay
    {
        private readonly PrePay _innerObject;

        [NoRender]
        public PrePay InnerObject
        {
            get { return _innerObject; }
        }

        public ListPrePay()
        {
            _innerObject = new PrePay();
        }

        public ListPrePay(PrePay adminUser)
        {
            _innerObject = adminUser;
        }

        public LinkObject PrePayId
        {
            get { return new LinkObject(InnerObject.PrePayId.ToString(), InnerObject.PrePayId, "PrePay", "View"); }
        }
        public string AccountName
        {
            get { return InnerObject.AccountName; }
        }

        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }
        public decimal? ActualAmount
        {
            get { return InnerObject.State == PrePayStates.Complted ? InnerObject.ActualAmount : (decimal?) null; }
        }
        public string ShopName
        {
            get { return InnerObject.ShopName; }
        }

        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }

        public DateTime UpdateTime
        {
            get { return InnerObject.UpdateTime; }
        }
        public DateTime? DoneTime
        {
            get { return InnerObject.State == PrePayStates.Complted ? InnerObject.UpdateTime : (DateTime?)null; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}