using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Orderss
{
    public class ListOrderss: EcardModelListRequest<ListOrders>
    {
        public ListOrderss()
        {
            OrderBy = "id desc";
        }
        private Bounded _type;
        public Bounded type
        {
            get
            {
                if (_type == null)
                {
                    _type = Bounded.Create<Orders>("type", OrderTypes.all);
                }
                return _type;
            }
            set { _type = value; }
        }

        public string mobile { get; set; }
        public string orderNo { get; set; }
        public string shopName { get; set; }

        private DataRange _data;
        public DataRange Data
        {
            get
            {
                if (_data == null)
                    _data = new DataRange();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" });
        }
        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [NoRender, Dependency]
        public IOrdersService OrdersService { get; set; }

        public void Query(out string pageHtml)
        {
            pageHtml = string.Empty;
            var request = new OrdersRequest();
            if ((request.type == OrderTypes.all))
            {
                request.type = null;
            }
            request.Bdate = Data.Start;
            request.Edate = Data.End;
            request.orderState = OrderStates.paid;
            if (request.pageIndex == null || request.pageIndex <= 0)
            {
                request.pageIndex = 1;
            }
            if (request.pageSize == null || request.pageSize <= 0)
            {
                request.pageSize = 10;
            }
            var query = OrdersService.Query(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListOrders(u));
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
            }
        }

        public List<ListOrders> AjaxGet(OrdersRequest request, out string pageHtml)
        {
            List<ListOrders> data = null;
            pageHtml = string.Empty;
            if ((request.type == OrderTypes.all))
            {
                request.type = null;
            }
            request.orderState = OrderStates.paid;
            if (request.pageIndex == null || request.pageIndex <= 0)
            {
                request.pageIndex = 1;
            }
            if (request.pageSize == null || request.pageSize <= 0)
            {
                request.pageSize = 10;
            }
            var query = OrdersService.Query(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListOrders(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);

            }
            return data;
        }
    }
}
