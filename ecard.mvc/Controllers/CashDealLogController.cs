using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.CashDealLogs;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class CashDealLogController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public CashDealLogController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }  
        [CheckPermission(Permissions.CashDealLog)]
        public virtual ActionResult List(ListCashDealLogs request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.CashDealLogEdit)]
        public ActionResult Suspend(int id, ListCashDealLogs request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.CashDealLogEdit)]
        public ActionResult Suspends(ListCashDealLogs request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }
        [CheckPermission(Permissions.CashDealLogEdit)]
        public ActionResult Create()
        {
            var createCashDealLog = _unityContainer.Resolve<CreateCashDealLog>();
            var model = new EcardModelItem<CreateCashDealLog>(createCashDealLog);
            createCashDealLog.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.CashDealLogEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateCashDealLog model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateCashDealLog>();
            }
            model.Ready();
            return View(new EcardModelItem<CreateCashDealLog>(model, msg));
        }

        [CheckPermission(Permissions.CashDealLogDone)]
        public ActionResult Done(DateTime submitDate, int ownerId)
        {
            var createCashDealLog = _unityContainer.Resolve<DoneCashDealLog>();
            createCashDealLog.SubmitDate = submitDate;
            createCashDealLog.OwnerId = ownerId;
            var model = new EcardModelItem<DoneCashDealLog>(createCashDealLog);
            
            createCashDealLog.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.CashDealLogDone)]
        public ActionResult Done([Bind(Prefix = "Item")] DoneCashDealLog model, DateTime submitDate, int ownerId)
        {

            model.SubmitDate = submitDate;
            model.OwnerId = ownerId;
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.Done();
            }
            model.Ready();
            
            return View(new EcardModelItem<DoneCashDealLog>(model, msg));
        }

    }
     
}
