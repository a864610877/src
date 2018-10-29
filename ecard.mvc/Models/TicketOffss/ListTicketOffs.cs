using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.TicketOffss
{
    public class ListTicketOffs : EcardModelListRequest<ListTicketOff>
    {
        public ListTicketOffs()
        {
            OrderBy = "Id desc";
        }

        private Bounded _offType;
        public Bounded OffType
        {
            get
            {
                if (_offType == null)
                {
                    _offType = Bounded.Create<TicketOff>("offType", OffTypes.ALL);
                }
                return _offType;
            }
            set { _offType = value; }
        }

        public string mobile { get; set; }
        public string shopDisplayName { get; set; }
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
        public ITicketOffService TicketOffService { get; set; }
        public void Query(out string pageHtml)
        {

            pageHtml = string.Empty;
            var request = new TicketOffRequest();
            if ((request.type == OffTypes.ALL))
            {
                request.type = null;
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
            var query = TicketOffService.Query(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListTicketOff(u));
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
            }
        }

        public List<ListTicketOff> AjaxGet(TicketOffRequest request, out string pageHtml)
        {
            List<ListTicketOff> data = null;
            pageHtml = string.Empty;
            if ((request.type == OffTypes.ALL))
            {
                request.type = null;
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
            var query = TicketOffService.Query(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListTicketOff(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
                
            }
            return data;
        }
    }
}
