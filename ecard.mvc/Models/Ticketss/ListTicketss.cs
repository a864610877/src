using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Ticketss
{
    public class ListTicketss: EcardModelListRequest<ListTickets>
    {
        public ListTicketss()
        {
            OrderBy = "Id desc";
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Tickets>("State", TicketsState.All);
                }
                return _state;
            }
            set { _state = value; }
        }
        public string ticketName { get; set; }
        public string orderNo { get; set; }
        public string mobile { get; set; }
        public string code { get; set; }
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
        public ITicketsService TicketsService { get; set; }

        public void Query(out string pageHtml)
        {
            pageHtml = string.Empty;
            var request = new TicketsRequest();
            if ((request.state == TicketsState.All))
            {
                request.state = null;
            }
            request.Bdate = Data.Start;
            request.Edate = Data.End;
            if (request.pageIndex == null || request.pageIndex <= 0)
            {
                request.pageIndex = 1;
            }
            if (request.pageSize == null || request.pageSize <= 0)
            {
                request.pageSize = 10;
            }
            var query = TicketsService.GetList(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListTickets(u));
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
            }
        }

        public List<ListTickets> AjaxGet(TicketsRequest request, out string pageHtml)
        {
            List<ListTickets> data = null;
            pageHtml = string.Empty;
            if ((request.state == TicketsState.All))
            {
                request.state = null;
            }
            if (request.pageIndex == null || request.pageIndex <= 0)
            {
                request.pageIndex = 1;
            }
            if (request.pageSize == null || request.pageSize <= 0)
            {
                request.pageSize = 10;
            }
            var query = TicketsService.GetList(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListTickets(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);

            }
            return data;
        }
    }
}
