using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class WindowTicketing
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 门票编码
        /// </summary>

        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int admissionTicketId { get; set; }

        public int shopId { get; set; }
        /// <summary>
        /// 门票名称
        /// </summary>
        public string ticketName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal discount { get; set; }
        /// <summary>
        /// 支付方式 1 现金 2微信支付 3支付宝 4 其他
        /// </summary>
        [Bounded(typeof(WindowTicketingPayType))]
        public int payType { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 宝宝姓名
        /// </summary>
        public string babyName { get; set; }
        /// <summary>
        /// 宝宝性别
        /// </summary>
        [Bounded(typeof(Genders))]
        public int babySex { get; set; }
        /// <summary>
        /// 宝宝出生年月
        /// </summary>
        public DateTime? babyBirthDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime createTime { get; set; }

    }

    public class WindowTicketingPayType
    {

        public const int All = -10001;
        /// <summary>
        ///现金
        /// </summary>
        public const int cash = 1;
        /// <summary>
        /// 微信
        /// </summary>
        public const int WeChat = 2;
        /// <summary>
        /// 支付宝
        /// </summary>
        public const int Alipay = 3;
        /// <summary>
        /// 其他
        /// </summary>
        public const int Other = 4;
    }
}
