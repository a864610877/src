using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.DealWays;
using Ecard.Mvc.ViewModels;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class DealWayController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public DealWayController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }


        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Create()
        {
            var createDealWay = _unityContainer.Resolve<CreateDealWay>();
            var model = new EcardModelItem<CreateDealWay>(createDealWay);
            createDealWay.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateDealWay model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateDealWay>();
            }
            model.Ready();
            return View(new EcardModelItem<CreateDealWay>(model, msg));
        }

        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditDealWay>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditDealWay>(model));
        }

        [CheckPermission(Permissions.DealWay)]
        public ActionResult View(int id)
        {
            var model = _unityContainer.Resolve<ViewDealWay>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<ViewDealWay>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditDealWay model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                msg = model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditDealWay>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Deletes(ListDealWays request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Delete(int id, ListDealWays request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.DealWay)]
        public virtual ActionResult List(ListDealWays request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Suspend(int id, ListDealWays request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Suspends(ListDealWays request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Resume(int id, ListDealWays request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.DealWayEdit)]
        public ActionResult Resumes(ListDealWays request)
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