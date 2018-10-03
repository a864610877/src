using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class CashDealLog : IKeySetter, IRecordVersion
    {
        public CashDealLog(decimal amount, int userId, int ownerId, int dealType)
        {
            SubmitTime = DateTime.Now;
            SubmitDate = SubmitTime.Date;
            Amount = amount;
            UserId = userId;
            OwnerId = ownerId;
            DealType = dealType;
            State = CashDealLogStates.Normal;
        }

        public CashDealLog()
        {
            SubmitTime = DateTime.Now;
            SubmitDate = SubmitTime.Date;
            State = CashDealLogStates.Normal;
        }
        [Key]
        public int CashDealLogId { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime SubmitDate { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 放贷人
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 债务人
        /// </summary>
        public int OwnerId { get; set; }
        [Bounded(typeof(CashDealLogStates))]
        public int State { get; set; }

        int IKeySetter.Id
        {
            get { return CashDealLogId; }
            set { CashDealLogId = value; }
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        [Bounded(typeof(CashDealLogTypes))]
        public int DealType { get; set; }

        public int RecordVersion { get; set; }
    }
}