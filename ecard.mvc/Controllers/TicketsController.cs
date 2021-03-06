﻿using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class TicketsController: Controller
    {
        private readonly IUnityContainer _unityContainer;

        public TicketsController(IUnityContainer unityContaine)
        {
            this._unityContainer = unityContaine;
        }
        [ActionFilters.CheckPermission(Permissions.BuyTicketList)]
        public ActionResult List(Models.Ticketss.ListTicketss request)
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
        public ActionResult ListPost(TicketsRequest request)
        {
            var createRole = _unityContainer.Resolve<Models.Ticketss.ListTicketss>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
        [ActionFilters.CheckPermission(Permissions.BuyTicketListReport)]
        public ActionResult Export(Models.Ticketss.ListTicketss request)
        {
            //_logger.LogWithSerialNo(LogTypes.AccountExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
    }
}
