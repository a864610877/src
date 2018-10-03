using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class Liquidate
    {
        [Key]
        public int LiquidateId { get; set; }  
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal DealAmount { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal CancelAmount { get; set; }
        /// <summary>
        /// 所有交易ID
        /// </summary>
        public string DealIds { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Bounded(typeof(LiquidateStates))]
        public int State{ get; set; }

        public int Count { get; set; }

        public Liquidate()
        {
            SubmitTime = DateTime.Now;
        }
    }
}