using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class Coupons
    {
        [Key]
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
        /// 名称
        /// </summary>
        public string name { get; set; }
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
        /// 发放数量，0：所有会员在领卷有效期内都可以领取一张；大于0：每个会员限领一张，领完为止
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 有效期 超过此有效期，不可使用
        /// </summary>
        public DateTime? validity { get; set; }
        /// <summary>
        /// 已领数量
        /// </summary>
        public int leadersOfNum { get; set; }
        /// <summary>
        /// 状态 1启用 2停用
        /// </summary>
        [Bounded(typeof(CouponsState))]
        public int state { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string createOp { get; set; }
        public DateTime createTime { get; set; }
    }

    public class Couponss: Coupons
    {
        public string shopName { get; set; }
    }
    public class CouponsType
    {
        /// <summary>
        /// 折扣券 1
        /// </summary>
        public const int DiscountedVolume = 1;
        /// <summary>
        /// 抵扣券 2
        /// </summary>
        public const int OffsetRoll = 2;
        /// <summary>
        /// 满减券 3
        /// </summary>
        public const int FullVolumeReduction = 3;

        public static string GetName(int value)
        {
            switch (value)
            {
                case DiscountedVolume:
                    return "折扣券";
                case OffsetRoll:
                    return "抵扣券";
                case FullVolumeReduction:
                    return "满减券";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 全部 100000
        /// </summary>
       // public const int All = 100000;
    }
    public class CouponsState
    {
        /// <summary>
        /// 正常的 1
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// 暂时停使用的 2
        /// </summary>
        public const int Invalid = 2;

        /// <summary>
        /// 全部 100000
        /// </summary>
        public const int All = 100000;
    }
}
