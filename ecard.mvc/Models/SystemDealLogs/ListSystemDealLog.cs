using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.SystemDealLogs
{
    public class ListSystemDealLog
    {
        private readonly SystemDealLog _innerObject;

        [NoRender]
        public SystemDealLog InnerObject
        {
            get { return _innerObject; }
        }

        public ListSystemDealLog()
        {
            _innerObject = new SystemDealLog();
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public ListSystemDealLog(SystemDealLog innerObject)
        {
            _innerObject = innerObject;
        }

        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }

        public decimal? AmountIn
        {
            get
            {
                switch (InnerObject.DealType)
                {
                    case SystemDealLogTypes.ShopDealLogCharging:
                        return InnerObject.Amount > 0 ? InnerObject.Amount : (decimal?)null;
                    default:
                        return null;
                }
            }
        }
        public decimal? AmountShouldPay
        {
            get
            {
                if (InnerObject.Amount > 0)
                    return AmountIn > 0 ? (decimal?) null : InnerObject.Amount;
                return null;
            }
        }
        public decimal? AmountOut
        {
            get { return InnerObject.Amount < 0 ? -InnerObject.Amount : (decimal?)null; }
        }

        public string Id
        {
            get { return InnerObject.SerialNo; }
        }

        public string UserName
        {
            get { return InnerObject.UserName; }
        }

        public string HowToDeal { get; set; }
        public decimal SiteAmount
        {
            get { return InnerObject.SiteAmount; }
        }
        public bool? HasReceipt
        {
            get { return InnerObject.Amount > 0 ? InnerObject.HasReceipt : (bool?)null; }
        }

        [NoRender]
        public int SystemDealLogId
        {
            get { return InnerObject.SystemDealLogId; }
        }

        public string DealType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.DealType); }
        }
    }
}