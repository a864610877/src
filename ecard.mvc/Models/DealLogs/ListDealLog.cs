using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.DealLogs
{
    public class ListDealLog
    {
        private readonly DealLog _innerObject;

        [NoRender]
        public DealLog InnerObject
        {
            get { return _innerObject; }
        }

        public ListDealLog()
        {
            _innerObject = new DealLog();
        }

        public ListDealLog(DealLog adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int DealLogId
        {
            get { return InnerObject.DealLogId; }
        }

        //public bool Liquidated
        //{
        //    get { return InnerObject.LiquidateDealLogId != 0; }
        //}
        public string SerialNo
        {
            get { return InnerObject.SerialNo; }
            set {  _innerObject.SerialNo = value; }
        } 

       
        public string AccountName
        {
            get { return InnerObject.AccountName; }
        }

        public string DealType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.DealType); }
        }
        public string PosName
        {
            get { return InnerObject.SourcePosName; }
        }

        public LinkObject ShopDisplayName
        {
            get { return new LinkObject(InnerObject.ShopDisplayName, InnerObject.ShopId, "Shop", "View"); }
        }


        //public string AmountIn
        //{
        //    get { return InnerObject.Amount > 0 ? "" : (-InnerObject.Amount).ToString(); }
        //}
        //public string AmountOut
        //{
        //    get { return InnerObject.Amount <= 0 ? "" : (InnerObject.Amount).ToString(); }
        //}

        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }

        public int Point
        {
            get { return InnerObject.Point; }
            set { InnerObject.Point = value; }
        }


        public decimal AccountAmount
        {
            get { return InnerObject.AccountAmount; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        internal Shop TempShop { get; set; }
        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }
    }
}