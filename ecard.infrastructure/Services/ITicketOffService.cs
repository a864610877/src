using Ecard.Infrastructure;
using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface ITicketOffService
    {
        void Insert(TicketOff item);

        DataTables<TicketOffs> Query(TicketOffRequest request);
    }

    public class TicketOffs : TicketOff
    {
        /// <summary>
        /// 门店号
        /// </summary>
         public string shopName { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string shopDisplayName { get; set; }
        /// <summary>
        /// 消费手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 消费者姓名
        /// </summary>
        public string userDisplayName { get; set; }



    }

    public class TicketOffRequest
    {
        
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
        
        /// <summary>
        /// 手机
        /// </summary>

        public string mobile { get; set; }
        /// <summary>
        /// 门店号
        /// </summary>
        public string shopName { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string shopDisplayName { get; set; }

        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }

    }
}
