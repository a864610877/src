using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.Rollbacks
{
    public class ListRollback
    {
        private readonly RollbackShopDealLog _rollback;
        private ShopDealLog _shopDealLog;
        private Shop _shop;

        public ListRollback(RollbackShopDealLog rollback)
        {
            _rollback = rollback;
        }

        public DateTime SubmitTime
        {
            get { return _rollback.SubmitTime; }
        }

        public string SerialNo
        {
            get { return _shopDealLog.SerialNo; }
        }
        public object SerialServerNo
        {
            get { return new LinkObject(_shopDealLog.SerialServerNo, "Rollback", "View", new { id = _rollback.RollbackShopDealLogId }); }
        }
        public string ShopDisplayName
        {
            get { return _shop.DisplayName; }
        }
        public string ShopName
        {
            get { return _shop.Name; }
        }
        public DateTime DealSubmitTime
        {
            get { return _shopDealLog.SubmitTime; }
        }
        public string DealType
        {
            get { return ModelHelper.GetBoundText(_shopDealLog, x => x.DealType); }
        }

        public string AccountName
        {
            get { return _shopDealLog.AccountName; }
        }

        public decimal Amount
        {
            get { return _shopDealLog.Amount; }
        }

        [NoRender]
        public int RollbackId
        {
            get { return _rollback.RollbackShopDealLogId; }
        }
        [NoRender]
        public ShopDealLog ShopDealLog
        {
            get { return _shopDealLog; }
            set { _shopDealLog = value; }
        }
        [NoRender]
        public Shop Shop
        {
            get { return _shop; }
            set { _shop = value; }
        }

        [NoRender]
        public int ShopDealLogId
        {
            get { return this._rollback.ShopDealLogId; }
        }
    }
}