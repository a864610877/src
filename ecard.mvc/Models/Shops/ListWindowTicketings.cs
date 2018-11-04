using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Shops
{
    public class ListWindowTicketings : EcardModelListRequest<ListWindowTicketing>
    {
        public ListWindowTicketings()
        {
            OrderBy = "id desc";
        }
        private Bounded _admissionTicketBounded;
        public Bounded AdmissionTicket
        {
            get
            {
                if (_admissionTicketBounded == null)
                {
                    _admissionTicketBounded = Bounded.CreateEmpty("admissionTicketId", Globals.All);
                }
                return _admissionTicketBounded;
            }
            set { _admissionTicketBounded = value; }
        }

        private Bounded _payType;
        public Bounded PayType
        {
            get
            {
                if (_payType == null)
                {
                    _payType = Bounded.Create<WindowTicketing>("payType", WindowTicketingPayType.All);
                }
                return _payType;
            }
            set { _payType = value; }
        }
        public string mobile { get; set; }
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
        [NoRender, Dependency]
        public IAdmissionTicketService admissionTicketService { get; set; }
        [Dependency]
        [NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        [NoRender]
        public IWindowTicketingService windowTicketingService { get; set; }
        public void Ready()
        {
            IEnumerable<IdNamePair> query = admissionTicketService.GetNormalALL()
             .ToList().Select(x => new IdNamePair { Key = x.id, Name = x.name });
            AdmissionTicket.Bind(query);
            AdmissionTicket.Items.Insert(0, new IdNamePair() { Key = Globals.All, Name = "全部" });
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("ExportWindowTicketing", null, new { export = "excel" });
        }

        public void Query(out string pageHtml)
        {

            pageHtml = string.Empty;
            var request = new WindowTicketingRequest();
            if ((request.payType == WindowTicketingPayType.All))
            {
                request.payType = null;
            }
            if ((request.admissionTicketId == Globals.All))
            {
                request.admissionTicketId = null;
            }
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            if (user is ShopUser)
            {
                request.shopId = (user as ShopUser).ShopId;
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
            var query = windowTicketingService.Query(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListWindowTicketing(u));
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
            }
        }

        public List<ListWindowTicketing> AjaxGet(WindowTicketingRequest request, out string pageHtml)
        {
            List<ListWindowTicketing> data = null;
            pageHtml = string.Empty;
            if ((request.payType == WindowTicketingPayType.All))
            {
                request.payType = null;
            }
            if ((request.admissionTicketId == Globals.All))
            {
                request.admissionTicketId = null;
            }
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            if (user is ShopUser)
            {
                request.shopId = (user as ShopUser).ShopId;
            }
            if (request.pageIndex == null || request.pageIndex <= 0)
            {
                request.pageIndex = 1;
            }
            if (request.pageSize == null || request.pageSize <= 0)
            {
                request.pageSize = 10;
            }
            var query = windowTicketingService.Query(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListWindowTicketing(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);

            }
            return data;
        }
    }
}
