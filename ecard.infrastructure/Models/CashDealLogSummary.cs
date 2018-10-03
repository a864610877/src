using System;

namespace Ecard.Models
{
    public class CashDealLogSummary
    {
        public DateTime SubmitDate { get; set; }
        public decimal Amount { get; set; } 
        /// <summary>
        /// Õ®ÎñÈË
        /// </summary>
        public int OwnerId { get; set; }
        [Bounded(typeof(CashDealLogSummaryStates))]
        public int State { get; set; }  
    }
}