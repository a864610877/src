using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PointRebates;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PointRebateController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper Logger;

        public PointRebateController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            Logger = logger;
        }

        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Create()
        {
            var createPointRebate = _unityContainer.Resolve<CreatePointRebate>();
            var model = new EcardModelItem<CreatePointRebate>(createPointRebate);
            createPointRebate.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreatePointRebate model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreatePointRebate>();
            }
            model.Ready();
            return View(new EcardModelItem<CreatePointRebate>(model, msg));
        }

        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditPointRebate>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditPointRebate>(model));
        }

        public ActionResult GetRebate(int id)
        {
            try
            {
                var model = _unityContainer.Resolve<EditPointRebate>();
                model.Read(id);

                return Json(model.InnerObject, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.RebateGet, ex);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditPointRebate model)
        {
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditPointRebate>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Deletes(ListPointRebates request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Delete(int id, ListPointRebates request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.PointRebate)]
        public virtual ActionResult List(ListPointRebates request)
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
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Suspend(int id, ListPointRebates request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Suspends(ListPointRebates request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Resume(int id, ListPointRebates request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointRebateEdit)]
        public ActionResult Resumes(ListPointRebates request)
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
