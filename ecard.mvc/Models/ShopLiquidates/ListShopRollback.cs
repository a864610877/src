using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class ListShopRollback
    {
        private readonly RollbackShopDealLog _rollback;
        private readonly ShopDealLog _shopDealLog;

        public ListShopRollback(RollbackShopDealLog rollback, ShopDealLog shopDealLog)
        {
            _rollback = rollback;
            _shopDealLog = shopDealLog;
        }

        public DateTime SubmitTime
        {
            get { return _rollback.SubmitTime; }
        }

        public string SerialNo
        {
            get { return _shopDealLog.SerialNo; }
        }
        public string SerialServerNo
        {
            get { return _shopDealLog.SerialServerNo; }
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

        public int RollbackId
        {
            get { return _rollback.RollbackShopDealLogId; }
        }
    }
}