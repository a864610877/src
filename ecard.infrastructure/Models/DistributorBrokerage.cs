using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    [Serializable]
    public class DistributorBrokerage
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 结算开始时间 
        /// </summary>
        public DateTime Bdate { set; get; }
        /// <summary>
        /// 结算结束时间
        /// </summary>
        public DateTime Edate { set; get; }
        /// <summary>
        /// 会员卡归属的经销商Id
        /// </summary>
        public int DistributorId { set; get; }
        /// <summary>
        /// 结算的经销商Id
        /// </summary>
        public int settlementDistributorId { set; get; }
        /// <summary>
        /// 会员卡号
        /// </summary>
        public int AccountId { set; get; }
        /// <summary>
        /// 会员卡消费金额
        /// </summary>
        public decimal consume { set; get; }
        /// <summary>
        /// 经销商提成比例
        /// </summary>
        public decimal Rate { set; get; }
        /// <summary>
        /// 经销商的提成金额
        /// </summary>
        public decimal brokerage { set; get; }
        /// <summary>
        /// 是否已结算，由经销商确认。
        /// </summary>
        public bool status { set; get; }

    }
}
