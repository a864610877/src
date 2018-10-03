using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.GoodandOrder;
using Microsoft.Practices.Unity;
using Ecard.Mvc.ViewModels;
using Ecard.Services;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class GoodController : BaseController
    {
        private readonly IUnityContainer _unityContainer;
        public GoodController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Create()
        {
            var createGood = _unityContainer.Resolve<CreateGood>();
            var model = new EcardModelItem<CreateGood>(createGood);
            createGood.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateGood model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateGood>();
            }
            model.Ready();
            return View(new EcardModelItem<CreateGood>(model, msg));
        }

        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditGood>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditGood>(model));
        }

        [CheckPermission(Permissions.Good)]
        public ActionResult View(int id)
        {
            return View();
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditGood model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                msg = model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditGood>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Deletes(ListGoods request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Delete(int id,ListGoods request)
        {
            request.Delete(id);
            return List(request);
        }

        [CheckPermission(Permissions.Good)]
        public virtual ActionResult List(ListGoods request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request); ;
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Suspend(int id, ListGoods request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Suspends(ListGoods request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Resume(int id, ListGoods request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.GoodEdit)]
        public ActionResult Resumes(ListGoods request)
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
