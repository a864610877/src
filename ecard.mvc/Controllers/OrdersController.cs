using Ecard.Mvc.Models.Orderss;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class OrdersController: Controller
    {
        private readonly IUnityContainer _unityContainer;

        public OrdersController(IUnityContainer unityContaine)
        {
            this._unityContainer = unityContaine;
        }
        [ActionFilters.CheckPermission(Permissions.OrdersList)]
        public ActionResult List(ListOrderss request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            return View("List", request);
        }
        [HttpPost]
        public ActionResult ListPost(OrdersRequest request)
        {
            var createRole = _unityContainer.Resolve<ListOrderss>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
        [ActionFilters.CheckPermission(Permissions.OrdersListReport)]
        public ActionResult Export(ListOrderss request)
        {
            //_logger.LogWithSerialNo(LogTypes.AccountExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
    }
}
