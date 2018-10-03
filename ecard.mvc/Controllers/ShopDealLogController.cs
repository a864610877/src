using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.ShopDealLogs;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class ShopDealLogController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;

        public ShopDealLogController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckPermission(Permissions.User)]
        public ActionResult Export(ListShopDealLogs request)
        {
            _logger.LogWithSerialNo(LogTypes.LiquidateExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
        [CheckPermission(Permissions.ShopDealLog)]
        public virtual ActionResult List(ListShopDealLogs request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            request.Ready();
            return View("List", request);
        }
         

        [CheckPermission(Permissions.ShopDealLogPrint)]
        public ActionResult Print(ShopDealLogPrinter model)
        {
            return Json(model.Print(), JsonRequestBehavior.AllowGet);
        }
    }
}
