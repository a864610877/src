using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class HandRingPrint
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 门票代码/卡号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 核销门店
        /// </summary>
        public int shopId { get; set; }
        /// <summary>
        /// 类型 1门票 2卡 3窗口售票
        /// </summary>
        [Bounded(typeof(HandRingPrintTicketType))]
        public int ticketType { get; set; }
        /// <summary>
        /// 大人数量
        /// </summary>
        public int adultNum { get; set; }
        /// <summary>
        /// 小孩数量
        /// </summary>
        public int childNum { get; set; }
        /// <summary>
        /// 家长姓名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 宝宝姓名
        /// </summary>
        public string babyName { get; set; }
        /// <summary>
        /// 宝宝性别
        /// </summary>
        public string babySex { get; set; }
        /// <summary>
        /// 宝宝出生年月
        /// </summary>
        public string babyBirthDate { get; set; }
        /// <summary>
        /// 状态 1未打印 2已打印
        /// </summary>
        [Bounded(typeof(HandRingPrintState))]
        public int state { get; set; }
        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime? printTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
    }

    public class HandRingPrintTicketType
    {
        public const int all = -10001;
        /// <summary>
        /// 门票
        /// </summary>
        public const int ticket = 1;
        /// <summary>
        /// 卡
        /// </summary>
        public const int card = 2;
        /// <summary>
        /// 窗口售票
        /// </summary>
        public const int windowTicket = 3;
    }

    public class HandRingPrintState
    {

        public const int all = -10001;
        /// <summary>
        /// 未打印
        /// </summary>
        public const int bot = 1;
        /// <summary>
        /// 已打印
        /// </summary>
        public const int use = 2;
    }
}
