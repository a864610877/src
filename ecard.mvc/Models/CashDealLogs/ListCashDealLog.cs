using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.CashDealLogs
{
    public class ListCashDealLog
    {
        private readonly CashDealLog _innerObject;

        [NoRender]
        public CashDealLog InnerObject
        {
            get { return _innerObject; }
        }

        public ListCashDealLog()
        {
            _innerObject = new CashDealLog();
        }

        public ListCashDealLog(CashDealLog innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int CashDealLogId
        {
            get { return InnerObject.CashDealLogId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
        public string DealType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.DealType); }
        }

        public string OwnerName { get; set; }
        public string UserName { get; set; }
        public decimal? AmountIn
        {
            get { return InnerObject.Amount > 0 ? InnerObject.Amount : (decimal?) null; }
        }
        public decimal? AmountOut
        {
            get { return InnerObject.Amount < 0 ? -InnerObject.Amount : (decimal?) null; }
        }
        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }
    }
}