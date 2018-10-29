using System.Linq;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Commodities;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class CommodityController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly ICommodityService _commodityService;

        public CommodityController(IUnityContainer unityContainer, ICommodityService commodityService)
        {
            _unityContainer = unityContainer;
            _commodityService = commodityService;
        }

        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Create()
        {
            var createCommodity = _unityContainer.Resolve<CreateCommodity>();
            var model = new EcardModelItem<CreateCommodity>(createCommodity);
            createCommodity.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateCommodity model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateCommodity>();
            }
            model.Ready();
            return View(new EcardModelItem<CreateCommodity>(model, msg));
        }

        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditCommodity>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditCommodity>(model));
        }


        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditCommodity model)
        { 
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditCommodity>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Deletes(ListCommodities request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Delete(int id, ListCommodities request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.Commodity)]
        public virtual ActionResult List(ListCommodities request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query();
            }
            return View("List", request);
        }

        [CheckPermission(Permissions.CommodityEdit)]
        public virtual ActionResult ListFast(string name)
        {
            var items = _commodityService.Query(new CommodityRequest() { NameStartWith = name, State = CommodityStates.Normal }).ToList()
                .OrderBy(x => x.Name).Take(20).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Suspend(int id, ListCommodities request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Suspends(ListCommodities request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Resume(int id, ListCommodities request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.CommodityEdit)]
        public ActionResult Resumes(ListCommodities request)
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