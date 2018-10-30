using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class Tickets
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 门票id
        /// </summary>
        public int AdmissionTicketId { get; set; }
        /// <summary>
        /// 门票代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime ExpiredDate { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 大人数量
        /// </summary>
        public int adultNum { get; set; }
        /// <summary>
        /// 小孩数量
        /// </summary>
        public int childNum { get; set; }
        /// <summary>
        /// 1 未使用 2已使用 3已过期
        /// </summary>
        [Bounded(typeof(TicketsState))]
        public int State { get; set; }
        /// <summary>
        /// 使用范围 为空不限制使用，否则绑定商户号,指定门店消费
        /// </summary>
        public string useScope { get; set; }
        /// <summary>
        /// 使用日期
        /// </summary>
        public DateTime? userTime { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime BuyTime { get; set; }
    }

    public class Ticketss: Tickets
    {
        public string TicketName { get; set; }

        public string Introduce { get; set; }

        public string Mobile { get; set; }

        public string UserDisplayName { get; set; }

        public string ShopName { get; set; }
    }

    public class TicketsState
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
        public const int BeOverdue = 3;

        /// <summary>
        /// 全部 100000
        /// </summary>
        public const int All = 100000;
    }
}
