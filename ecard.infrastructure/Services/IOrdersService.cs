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

        DataTables<Ordersss> Query(OrdersRequest request);

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

        /// <summary>
        /// 使用范围 为空不限制使用，否则绑定商户号,指定门店消费抵扣
        /// </summary>
        public string useScope { get; set; }

        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }

    }
}
