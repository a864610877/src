using System;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.ShopDealLogs
{
    public class ListShopDealLog
    {
        private readonly ShopDealLog _innerObject;

        [NoRender]
        public ShopDealLog InnerObject
        {
            get { return _innerObject; }
        }

        public ListShopDealLog()
        {
            _innerObject = new ShopDealLog();
        }

        public ListShopDealLog(ShopDealLog adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int ShopDealLogId
        {
            get { return InnerObject.ShopDealLogId; }
        }

        public string SerialServerNo
        {
            get { return InnerObject.SerialServerNo; }
        }  
        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
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
            set { InnerObject.SourcePosName = value; }
        }

        public LinkObject ShopDisplayName
        {
            get { return new LinkObject(InnerObject.ShopDisplayName, InnerObject.ShopId, "Shop", "View"); }
        }

        public string AmountIn
        {
            get { return InnerObject.Amount >= 0 ? "" : (-InnerObject.Amount).ToString(); }
        }
        public string AmountOut
        {
            get { return InnerObject.Amount <= 0 ? "" : (InnerObject.Amount).ToString(); }
        }
        public decimal ShopRechargingAmount
        {
            get { return InnerObject.ShopRechargingAmount; }
        }

        public decimal ShopAmount
        {
            get { return InnerObject.ShopAmount; }
        }
        public bool? IsLiquidate
        {
            get { return this.DealLog == null ? (bool?) null : (DealLog.LiquidateDealLogId != 0); }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
        [NoRender]
        public Shop TempShop { get; set; }
        [NoRender]
        public DealLog DealLog { get; set; }
    }
}