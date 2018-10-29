using System.Collections.Generic;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models.ShopLiquidates;
using Microsoft.Practices.Unity;
using Moonlit.Runtime.Serialization;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class ShopLiquidateController : BaseController
    {
        private readonly LogHelper _logger;
        private readonly IUnityContainer _unityContainer;

        public ShopLiquidateController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult Manage()
        {
            return View("Manage", CreatePageModel());
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult ShopDealLogs(ShopDealLogs model)
        {
            model.Ready();
            return View("ShopDealLogs", model);
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult ShopLiquidates(ShopLiquidates model)
        {
            model.Ready();
            return View("ShopLiquidates", model);
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult ShopRollbacks(ShopRollbacks model)
        {
            model.Ready();
            return View("ShopRollbacks", model);
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult DeleteLiquidate(DeleteShopLiquidate model)
        {
            return Json(model.Ready(), JsonRequestBehavior.AllowGet);
        }
        [CheckUserType(typeof (ShopUser))]
        public ActionResult DeleteRollback(DeleteRollback model)
        {
            return Json(model.Ready(), JsonRequestBehavior.AllowGet);
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult AddLiquidate(AddShopLiquidates model, string ids)
        {
            model.Ids = ids.DeserializeAsJson<List<int>>();
            return Json(model.Ready(), JsonRequestBehavior.AllowGet);
        }

        [CheckUserType(typeof (ShopUser))]
        public ActionResult AddRollback(AddRallbackShopDealLogs model, string ids)
        {
            model.Ids = ids.DeserializeAsJson<List<int>>();
            return Json(model.Ready(), JsonRequestBehavior.AllowGet);
        }
    }
}