using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Ecard.Models
{
    public class ShopDealLog
    {
        public decimal GetPayAmount(Site site, Shop shop)
        {
            decimal dealAmount = 0;
            decimal cancelAmount = 0;
            if (DealType == DealTypes.CancelDonePrePay || DealType == DealTypes.CancelDeal)
                cancelAmount = -Amount;
            else
                dealAmount = Amount;

            return (dealAmount - cancelAmount) -
                   ((shop.ShopDealLogChargeRate ?? site.ShopDealLogChargeRate) * dealAmount).ToRound(2);
        }
        [Key]
        public int ShopDealLogId { get; set; }
        public DateTime SubmitTime { get; set; }
        public int SourceShopId { get; set; }
        public string SourceShopName { get; set; }
        public int SourcePosId { get; set; }
        public string SourcePosName { get; set; }
        /// <summary>
        /// 商户手续费支出
        /// </summary>
        public decimal ShopRechargingAmount { get; set; }

        [Bounded(typeof(DealLogStates))]
        public int State { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public decimal Amount { get; set; }

        public string SerialNo { get; set; }

        [Bounded(typeof(DealTypes))]
        public int DealType { get; set; }
        /// <summary>
        /// 清算记录ID，关联第 SystemDealLogs的主键
        /// </summary>
        public int LiquidateDealLogId { get; set; }

        public string SourceShopDisplayName { get; set; }

        public decimal ShopAmount { get; set; }

        public int Addin { get; set; }

        [DebuggerStepThrough]
        public ShopDealLog(string seriaNo, int dealType, decimal amount, PosEndPoint pos, Shop sourceShop, Account account, Shop shopTo, int addin)
            : this()
        {
            SerialNo = seriaNo;
            DealType = dealType;
            Amount = amount;
            SubmitTime = DateTime.Now;
            Pos = pos;
            SourceShop = sourceShop;
            Account = account;
            Shop = shopTo;
            Addin = addin;
        }
        [DebuggerStepThrough]
        public ShopDealLog(string seriaNo)
            : this()
        {
            SerialNo = seriaNo;
        }
        [DebuggerStepThrough]
        public ShopDealLog()
        {
            State = 1;
            SubmitTime = DateTime.Now;
        }

        public Account Account
        {
            set
            {
                if (value == null) return;
                this.AccountId = value.AccountId;
                this.AccountName = value.Name;
            }
        }

        public Shop SourceShop
        {
            set
            {
                if (value == null) return;
                this.SourceShopId = value.ShopId;
                this.SourceShopName = value.Name;
                this.SourceShopDisplayName = value.DisplayName;
            }
        }
        public Shop Shop
        {
            set
            {
                if (value == null) return;
                this.ShopId = value.ShopId;
                this.ShopName = value.Name;
                this.ShopDisplayName = value.DisplayName;
                this.ShopAmount = value.Amount;
                ShopRechargingAmount = value.RechargingAmount;
            }
        }

        public string ShopDisplayName { get; set; }

        public string ShopName { get; set; }

        public int ShopId { get; set; }

        public PosEndPoint Pos
        {
            set
            {
                if (value == null) return;
                this.SourcePosId = value.PosEndPointId;
                this.SourcePosName = value.Name;
            }
        }

        public string Code { get; set; }


        public string SerialServerNo
        {
            get
            {
                return this.Addin.ToString().PadLeft(12, '0');
            }
        }
    }
}