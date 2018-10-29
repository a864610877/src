using System;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models.DealOnlines;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class DealOnlineController : BaseController
    {
        private readonly IUnityContainer _unityContainer;
        private readonly SecurityHelper _securityHelper;
        private readonly IShopService _shopService;

        public DealOnlineController(IUnityContainer unityContainer, SecurityHelper securityHelper, IShopService shopService )
        {
            _unityContainer = unityContainer;
            _securityHelper = securityHelper;
            _shopService = shopService;
        }

        [HttpGet]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult Pay()
        {
            User currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
            if (currentUser is AdminUser)
            {
                return View("PayShopPage", CreatePageModel());
            }
            if (currentUser is ShopUser)
            {
                ViewBag.HasLayout = true;
                var shop = _shopService.GetById(((ShopUser) currentUser).ShopId);
                ViewData["ShpName"] = shop.Name;
                return View("PayAccountPage", CreatePageModel());
            }
            throw new Exception("no implement");
        }
        [HttpGet]
        [CheckPermission(Permissions.AccountCancelPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult CancelPay()
        { 
            return View("CancelPayDealLogPage", CreatePageModel());
        }

        [CheckPermission(Permissions.AccountCancelPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult CancelPayAccountPage(string serverSerialNo)
        {
            ViewData.Add("serverSerialNo", serverSerialNo);
            return View("CancelPayAccountPage", CreatePageModel());
        }

        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult PayAccountPage(string shopName)
        {
            ViewData["shopName"] = shopName;
            var shopUser= _securityHelper.GetCurrentUser().CurrentUser as ShopUser;
            if(shopUser != null)
            {
                ViewData["shopName"] = shopUser.Name;
            }
            ViewBag.HasLayout = false;
            return PartialView("PayAccountPage", CreatePageModel());
        }

        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult PayConfirm(PayConfirm request)
        {
            return PartialView("PayConfirm", request);
        }
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult CancelPayConfirm(CancelPayConfirm request)
        {
            request.Ready();
            return PartialView("CancelPayConfirm", request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountCancelPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult CancelPayDone(CancelPayDone item)
        {
            item.Ready();
            if (item.IsRetry)
                return Json(new SimpleAjaxResult(item.Error));
            var model = _unityContainer.Resolve<DealDetail>();
            model.Init(item.Response, DealTypes.CancelDeal);
            return PartialView("DealDetail", model);
        }
        [HttpGet]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(AdminUser))]
        public ActionResult Shops(Shops model)
        {
            User currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
            model.Ready((AdminUser)currentUser);
            return View("Shops", model);
        }

        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult Shop(PayShop model)
        {
            model.Ready();
            return View("Shop", model);
        }
        [CheckPermission(Permissions.AccountCancelPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult DealLog(DealLogModel model)
        {
            model.Ready();
            return View("DealLog", model);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult PayDone(PayDone item)
        {
            item.Ready();
            if (item.IsRetry)
                return Json(new SimpleAjaxResult(item.Error));
            var model = _unityContainer.Resolve<DealDetail>();
            model.Init(item.Response, DealTypes.Deal);
            return PartialView("DealDetail", model);
        }     

    }
}
