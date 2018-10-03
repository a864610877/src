using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class AdmissionTicket
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 门票名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 大人数量
        /// </summary>
        public int adultNum { get; set; }
        /// <summary>
        /// 小孩数量
        /// </summary>
        public int childNum { get; set; }
        /// <summary>
        /// 增加一位大人需要收取得金额
        /// </summary>
        public decimal addAdultAmount { get; set; }
        /// <summary>
        /// 平时价格
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 周末价格
        /// </summary>
        public decimal weekendAmount { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string introduce { get; set; }
        /// <summary>
        /// 详情
        /// </summary>
        public string details { get; set; }
        [Bounded(typeof(AdmissionTicketState))]
        public int state { get; set; }
        public DateTime crateTime { get; set; }
    }

    public class AdmissionTicketState
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
