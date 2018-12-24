using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Shops
{
    public class ListHandRingPrints : EcardModelListRequest<ListHandRingPrint>
    {
        public ListHandRingPrints()
        {
            OrderBy = "id desc";
        }

        private Bounded _ticketType;
        public Bounded TicketType
        {
            get
            {
                if (_ticketType == null)
                {
                    _ticketType = Bounded.Create<HandRingPrint>("ticketType", HandRingPrintTicketType.all);
                }
                return _ticketType;
            }
            set { _ticketType = value; }
        }
        //private Bounded _state;
        //public Bounded State
        //{
        //    get
        //    {
        //        if (_state == null)
        //        {
        //            _state = Bounded.Create<HandRingPrint>("state", HandRingPrintState.bot);
        //        }
        //        return _state;
        //    }
        //    set { _state = value; }
        //}
        public string Code { get; set; }
        public string BabyName { get; set; }
        public string Mobile { get; set; }
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
        public IHandRingPrintService handRingPrintService { get; set; }
        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListHandRingPrint item)
        {
            yield return new ActionMethodDescriptor("HandRingPrint", null, new { id = item.Id });
            
        }
        public void Query(out string pageHtml)
        {
            pageHtml = string.Empty;
            var request = new HandRingPrintRequest();
            request.pageIndex = 1;
            request.pageSize = 10;
            request.state = HandRingPrintState.bot;
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            if (user is ShopUser)
            {
                request.shopId = (user as ShopUser).ShopId;
            }
            var query = handRingPrintService.GetList(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, x => new ListHandRingPrint(x));
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
            }
        }

        public List<ListHandRingPrint> AjaxGet(HandRingPrintRequest request, out string pageHtml)
        {
            List<ListHandRingPrint> data = null;
            pageHtml = string.Empty;
            if ((request.ticketType == HandRingPrintTicketType.all))
            {
                request.ticketType = null;
            }
            if ((request.state == HandRingPrintState.all))
            {
                request.state = null;
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
            var query = handRingPrintService.GetList(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListHandRingPrint(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
                foreach (var item in data)
                {
                    item.boor += "<a href='#' onclick=OperatorThis('HandRingPrint','/Shop/GetHandRingInfo/" + item.Id + "') class='tablelink'>打印手环 </a> ";
                }
            }
            return data;
        }

        public ResultMsg<List<PritModel>> GetInfo(int id)
        {
            var item = handRingPrintService.GetById(id);
            if (item == null)
            {
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "手环不存在" };
            }
            List<PritModel> dataPrint = new List<PritModel>();
            for (int j = 0; j < item.adultNum + item.childNum; j++)
            {
                PritModel model = new PritModel();
                model.effectiveTime = DateTime.Now.AddHours(3).ToString("MM月dd日hh时mm分");
                model.einlass = DateTime.Now.ToString("MM月dd日hh时mm分");
                model.name = item.userName;
                model.mobile = item.mobile ;
                model.people = item.adultNum + item.childNum;
                dataPrint.Add(model);
            }
            return new ResultMsg<List<PritModel>>() { Code = 0, CodeText = "", data = dataPrint };
        }
    }
}
