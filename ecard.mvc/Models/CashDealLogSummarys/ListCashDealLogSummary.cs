using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.CashDealLogSummarys
{
    public class ListCashDealLogSummary
    {
        private readonly CashDealLogSummary _innerObject;

        [NoRender]
        public CashDealLogSummary InnerObject
        {
            get { return _innerObject; }
        }

        public DateTime SubmitDate
        {
            get { return InnerObject.SubmitDate; }
        }
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }
        public ListCashDealLogSummary()
        {
            _innerObject = new CashDealLogSummary();
        }

        public ListCashDealLogSummary(CashDealLogSummary innerObject)
        {
            _innerObject = innerObject;
        }
         
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public string OwnerName { get; set; }
    }
}