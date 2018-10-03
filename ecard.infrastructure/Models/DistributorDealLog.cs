using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Ecard.Models
{
    public class DistributorDealLog
    {
        public decimal GetPayAmount(Site site, Distributor distributor)
        {
            decimal dealAmount = 0;
            decimal cancelAmount = 0;
            if (DealType == DealTypes.CancelDonePrePay || DealType == DealTypes.CancelDeal)
                cancelAmount = -Amount;
            else
                dealAmount = Amount;

            //return (dealAmount - cancelAmount) -
            //       ((distributor.DistributorDealLogChargeRate ?? site.DistributorDealLogChargeRate) * dealAmount).ToRound(2);
            return 0;
        }
        [Key]
        public int DistributorDealLogId { get; set; }
        public DateTime SubmitTime { get; set; }
        public int SourceDistributorId { get; set; }
        public string SourceDistributorName { get; set; }
        public int SourcePosId { get; set; }
        public string SourcePosName { get; set; }
        /// <summary>
        /// 商户手续费支出
        /// </summary>
        public decimal DistributorRechargingAmount { get; set; }

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

        public string SourceDistributorDisplayName { get; set; }

        public decimal DistributorAmount { get; set; }

        public int Addin { get; set; }

        [DebuggerStepThrough]
        public DistributorDealLog(string seriaNo, int dealType, decimal amount, PosEndPoint pos, Distributor sourceDistributor, Account account, Distributor distributorTo, int addin)
            : this()
        {
            SerialNo = seriaNo;
            DealType = dealType;
            Amount = amount;
            SubmitTime = DateTime.Now;
            Pos = pos;
            SourceDistributor = sourceDistributor;
            Account = account;
            Distributor = distributorTo;
            Addin = addin;
        }
        [DebuggerStepThrough]
        public DistributorDealLog(string seriaNo)
            : this()
        {
            SerialNo = seriaNo;
        }
        [DebuggerStepThrough]
        public DistributorDealLog()
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

        public Distributor SourceDistributor
        {
            set
            {
                if (value == null) return;
                this.SourceDistributorId = value.DistributorId;
                //this.SourceDistributorName = value.Name;
                //this.SourceDistributorDisplayName = value.DisplayName;
            }
        }
        public Distributor Distributor
        {
            set
            {
                if (value == null) return;
                this.DistributorId = value.DistributorId;
                //this.DistributorName = value.Name;
                //this.DistributorDisplayName = value.DisplayName;
                //this.DistributorAmount = value.Amount;
                //DistributorRechargingAmount = value.RechargingAmount;
            }
        }

        public string DistributorDisplayName { get; set; }

        public string DistributorName { get; set; }

        public int DistributorId { get; set; }

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