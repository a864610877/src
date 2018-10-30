using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class Orders
    {
        [Key]
        public int id { get; set; }

        public int userId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 状态 1 等待付款 2 已付款
        /// </summary>
        [Bounded(typeof(OrderStates))]
        public int orderState { get; set; }
        /// <summary>
        /// 类型 1购票 2购卡 3充值卡
        /// </summary>
        [Bounded(typeof(OrderTypes))]
        public int type { get; set; }
        /// <summary>
        /// 优惠卷抵扣金额
        /// </summary>
        public decimal deductible { get; set; }
        /// <summary>
        /// 支付的金额
        /// </summary>
        public decimal payAmount { get; set; }
        /// <summary>
        /// 使用范围 为空不限制使用，否则绑定商户号,指定门店消费抵扣
        /// </summary>
        public string useScope { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? payTime { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime subTime { get; set; }
    }

    public class Ordersss: Orders
    {
         public string mobile { get; set; }
         public string userDisplayName { get; set; }
         public string shopName { get; set; }
    }

    public class OrderStates
    {
        /// <summary>
        /// 请选择
        /// </summary>
        public const int all = 0;
        /// <summary>
        /// 等待付款
        /// </summary>
        public const int awaitPay = 1;
        /// <summary>
        /// 已付款
        /// </summary>
        public const int paid = 2;
        public static string GetName(int status)
        {
            switch (status)
            {
                case all:
                    return "全部";
                case awaitPay:
                    return "等待付款";
                case paid:
                    return "已付款";
                default:
                    return " ";
            }


        }
    }
    public class OrderTypes
    {
        /// <summary>
        /// 请选择
        /// </summary>
        public const int all = 0;
        /// <summary>
        /// 购票
        /// </summary>
        public const int ticket = 1;
        /// <summary>
        /// 购卡
        /// </summary>
        public const int card = 2;
        /// <summary>
        /// 卡充值
        /// </summary>
        public const int cardRecharge = 3;
        public static string GetName(int status)
        {
            switch (status)
            {
                case all:
                    return "全部";
                case ticket:
                    return "购票";
                case card:
                    return "购卡";
                case cardRecharge:
                    return "卡充值";
                default:
                    return " ";
            }


        }
    }
}
