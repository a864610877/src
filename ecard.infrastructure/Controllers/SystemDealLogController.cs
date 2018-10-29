using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models.SystemDealLogs;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class SystemDealLogController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;

        public SystemDealLogController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckPermission(Permissions.SystemDealLogExport)]
        public virtual ActionResult Export(ListSystemDealLogs request)
        {
            _logger.LogWithSerialNo(LogTypes.SystemDealLogExport, SerialNoHelper.Create(), 0);
            return List(request);
        }

        [CheckPermission(Permissions.SystemDealLog)]
        public virtual ActionResult List(ListSystemDealLogs request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            request.Ready();
            return View("List", request);
        }

        [CheckPermission(Permissions.SystemDealLogCloseRecharging)]
        [HttpPost]
        public virtual ActionResult CloseRecharging(CloseRecharging request)
        {
            return Json(request.Save(), JsonRequestBehavior.AllowGet);
        }


        [CheckPermission(Permissions.SystemDealLogOpenReceipt)]
        public virtual ActionResult OpenReceipt(int id, ListSystemDealLogs request)
        {
            return Json(request.OpenReceipt(id), JsonRequestBehavior.AllowGet);

        }



    }
}
