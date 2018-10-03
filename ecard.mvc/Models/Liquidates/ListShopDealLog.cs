using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.Liquidates
{
    public class ListShopDealLog
    {
        private readonly ShopDealLog InnerObject;

        public ListShopDealLog(ShopDealLog innerObject)
        {
            InnerObject = innerObject;
        }

        public string SerialServerNo
        {
            get { return InnerObject.SerialServerNo; }
        }
        public string SerialNo
        {
            get { return InnerObject.SerialNo; }
        }
        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }
        public string AccountName
        {
            get { return InnerObject.AccountName; }
        }
        public string DealType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.DealType); }
        }
    }
}