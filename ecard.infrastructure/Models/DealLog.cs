using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Oxite.Model;

namespace Ecard.Models
{
    public class DealLog
    {
        [Key]
        public int DealLogId { get; set; }

        public DateTime SubmitTime { get; set; }

        public int SourceShopId { get; set; }
        public bool IsHidden { get; set; }

        public string SourceShopName { get; set; }

        public int SourcePosId { get; set; }

        public string SourcePosName { get; set; }
        [Bounded(typeof(DealLogStates))]
        public int State { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        /// <summary>
        /// 实际交易金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 原交易金额
        /// </summary>
        public decimal YAmount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountRate { get; set; }

        public string SerialNo { get; set; }

       

        public string SerialServerNo
        {
            get
            {
                return this.DealLogId.ToString().PadLeft(12, '0');
            }
        }
        [Bounded(typeof(DealTypes))]
        public int DealType { get; set; }
        /// <summary>
        /// 清算记录ID，关联第 SystemDealLogs的主键
        /// </summary>
        public int LiquidateDealLogId { get; set; }

        public string SourceShopDisplayName { get; set; }

        public decimal AccountAmount { get; set; }

        public int Addin { get; set; }
        public int Point { get; set; }

        [DebuggerStepThrough]
        public DealLog(string seriaNo, int dealType, decimal amount, int point, PosEndPoint pos, Shop sourceShop, Account account, Shop shopTo, int addin)
            : this()
        {
            SerialNo = seriaNo;
            DealType = dealType;
            Amount = amount;
            Point = point;
            SubmitTime = DateTime.Now;
            Pos = pos;
            SourceShop = sourceShop;
            Account = account;
            Shop = shopTo;
            Addin = addin;
        }
        [DebuggerStepThrough]
        public DealLog(string seriaNo)
            : this()
        {
            SerialNo = seriaNo;
        }
        [DebuggerStepThrough]
        public DealLog()
        {
            State = 1;
            SubmitTime = DateTime.Now;
        }

        public Account Account
        {
            set
            {
                if (value == null) return;
                this.AccountAmount = value.TotalAmount;
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

        public bool IsCancelEnabled
        {
            get { return this.State == DealLogStates.Normal && (this.DealType == DealTypes.Deal); }
        }

        public ShopDealLog ToShopDealLog(Shop shopTo)
        {
            shopTo.Amount += Amount;
            return new ShopDealLog(SerialNo)
                       {
                           AccountId = AccountId,
                           AccountName = AccountName,
                           Addin = this.DealLogId,
                           Amount = Amount,
                           DealType = DealType,
                           LiquidateDealLogId = LiquidateDealLogId,
                           Shop = shopTo,
                           SubmitTime = SubmitTime,
                           SourcePosId = SourcePosId,
                           SourcePosName = SourcePosName,
                           SourceShopId = SourceShopId,
                           SourceShopName = SourceShopName,
                           SourceShopDisplayName = SourceShopDisplayName,
                           State = DealLogStates.Normal,
                       };
        }
    }

    public class MessageTemplateStates : States
    {
    }
}