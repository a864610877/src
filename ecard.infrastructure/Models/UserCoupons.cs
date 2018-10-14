using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class UserCoupons
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 优惠卷id
        /// </summary>
        public int couponsId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 1未使用 2已使用 3已过期
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime receiveTime { get; set; }
        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? useTime { get; set; }
    }

    public class UserCouponsState
    {
        /// <summary>
        /// 未使用
        /// </summary>
        public const int NotUse = 1;
        /// <summary>
        /// 已使用
        /// </summary>
        public const int Used = 2;
        /// <summary>
        /// 已过期
        /// </summary>
        public const int Expired = 3;
    }

    public class UserCouponss : Coupons
    {
        public int id { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 优惠卷类型 1折扣卷 2抵扣卷 3满减卷
        /// </summary>
        [Bounded(typeof(CouponsType))]
        public int couponsType { get; set; }
        
        /// <summary>
        /// 使用范围 为空不限制使用，否则绑定商户号
        /// </summary>
        public string useScope { get; set; }
        /// <summary>
        /// 折扣 类型为1使用
        /// </summary>
        public decimal discount { get; set; }
        /// <summary>
        /// 抵扣金额 类型为2使用
        /// </summary>
        public decimal deductibleAmount { get; set; }
        /// <summary>
        /// 满减金额 满金额，当支付金额达到满金额时可减 满金额
        /// </summary>
        public decimal fullAmount { get; set; }
        /// <summary>
        /// 满减金额 减金额， 当支付金额达到满金额时可减 减金额额
        /// </summary>
        public decimal reduceAmount { get; set; }
        /// <summary>
        /// 有效期 超过此有效期，不可使用
        /// </summary>
        public DateTime? validity { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string shopName { get; set; }

    }
}
