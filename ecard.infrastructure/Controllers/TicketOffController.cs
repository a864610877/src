using Ecard.Mvc.Models.TicketOffss;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    public class TicketOffController: Controller
    {
        private readonly IUnityContainer _unityContainer;

        public TicketOffController(IUnityContainer unityContaine)
        {
            this._unityContainer = unityContaine;
        }
        [ActionFilters.CheckPermission(Permissions.TicketOffList)]
        public ActionResult List(ListTicketOffs request)
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
        public ActionResult ListPost(TicketOffRequest request)
        {
            var createRole = _unityContainer.Resolve<ListTicketOffs>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }

        [ActionFilters.CheckPermission(Permissions.TicketOffReport)]
        public ActionResult Export(ListTicketOffs request)
        {
            //_logger.LogWithSerialNo(LogTypes.AccountExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
    }
}
