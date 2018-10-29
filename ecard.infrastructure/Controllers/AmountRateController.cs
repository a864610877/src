using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.AmountRates;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class AmountRateController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public AmountRateController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Create()
        {
            var createAmountRate = _unityContainer.Resolve<CreateAmountRate>();
            var model = new EcardModelItem<CreateAmountRate>(createAmountRate);
            createAmountRate.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateAmountRate model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateAmountRate>();
            }
            model.Ready();
            return View(new EcardModelItem<CreateAmountRate>(model, msg));
        }

        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditAmountRate>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditAmountRate>(model));
        }


        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditAmountRate model)
        {
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditAmountRate>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Deletes(ListAmountRates request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Delete(int id, ListAmountRates request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.AmountRate)]
        public virtual ActionResult List(ListAmountRates request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            request.Ready();
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Suspend(int id, ListAmountRates request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Suspends(ListAmountRates request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Resume(int id, ListAmountRates request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AmountRateEdit)]
        public ActionResult Resumes(ListAmountRates request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Resume(id);
            }

            return List(request);
        }

    }
}
