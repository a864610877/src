using Ecard.Infrastructure;
using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IWindowTicketingService
    {
        void Insert(WindowTicketing item);
        DataTables<WindowTicketings> Query(WindowTicketingRequest request);
    }

    public class WindowTicketingRequest
    {
        public string mobile { get; set; }
        /// <summary>
        /// 门店号
        /// </summary>
        public string shopName { get; set; }

        public int? shopId { get; set; }

        public int? payType { get; set; }

        public int? admissionTicketId { get; set; }

        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class WindowTicketings : WindowTicketing
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string shopName { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string shopDisplayName { get; set; }
    }
}
