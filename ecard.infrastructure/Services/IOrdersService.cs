using Ecard.Infrastructure;
using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IOrdersService
    {
        Orders GetById(int id);
        Orders GetByOrderNo(string orderNo);

        void Update(Orders item);

        DataTables<Orders> Query(OrdersRequest request);

        void Create(Orders item);
    }

    public class OrdersRequest
    {
        public int? userId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? orderState { get; set; }
        /// <summary>
        /// 手机
        /// </summary>

        public string mobile { get; set; }
        public string orderNo { get; set; }
       
        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }

    }
}
