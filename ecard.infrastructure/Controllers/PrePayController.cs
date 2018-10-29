using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models.PrePays;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PrePayController : Controller
    {
        private readonly IPrePayService _prePayService;
        private readonly SecurityHelper _securityHelper;
        private readonly IUnityContainer _unityContainer;

        public PrePayController(IPrePayService prePayService, SecurityHelper securityHelper, IUnityContainer unityContainer)
        {
            _prePayService = prePayService;
            _securityHelper = securityHelper;
            _unityContainer = unityContainer;
        }


        [CheckPermission(Permissions.AccountPrePayList)]
        public virtual ActionResult List(ListPrePays request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var prePayRequest = new PrePayRequest();
                if (request.State != Globals.All)
                    prePayRequest.State = request.State;

                var currentUser = _securityHelper.GetCurrentUser();
                var shopUser = currentUser as ShopUserModel;
                if (shopUser != null)
                    prePayRequest.ShopId = shopUser.ShopId;

                var accountUser = currentUser as AccountUserModel;
                if (accountUser != null)
                {
                    var accountId = accountUser.Accounts.Select(x => x.AccountId).First();
                    prePayRequest.AccountId = accountId;
                }

                var query = this._prePayService.Query(prePayRequest);
                request.List = query.ToList(request, x => new ListPrePay(x));
            }
            return View("List", request);
        }
        [CheckPermission(Permissions.AccountPrePayDone)]
        [CheckUserType(typeof(AdminUser))]
        [HttpPost]
        public virtual ActionResult Done(DonePrePay model)
        {
            return Json(model.Save());
        }
        [CheckPermission(Permissions.AccountPrePayCancel)]
        [CheckUserType(typeof(AdminUser))]
        [HttpPost]
        public virtual ActionResult Cancel(CancelPrePay model)
        {
            return Json(model.Save());
        }
        [CheckPermission(Permissions.AccountPrePayList)]
        public ActionResult View(int id)
        {
            var model = _unityContainer.Resolve<ViewPrePay>();
            model.Read(id);
            return View(new EcardModelItem<ViewPrePay>(model));
        }
    }
}
