using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PointGifts;
using Ecard.Mvc.Models.PointRebates;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PointGiftController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper Logger;

        public PointGiftController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            Logger = logger;
        }

        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Create()
        {
            var createPointGift = _unityContainer.Resolve<CreatePointGift>();
            var model = new EcardModelItem<CreatePointGift>(createPointGift);
            createPointGift.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreatePointGift model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreatePointGift>();
            }
            model.Ready();
            return View(new EcardModelItem<CreatePointGift>(model, msg));
        }

        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditPointGift>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditPointGift>(model));
        }

        public ActionResult GetGift(int id)
        {
            try
            {
                var model = _unityContainer.Resolve<EditPointGift>();
                model.Read(id);

                return Json(model.InnerObject, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.PointGiftGet, ex);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditPointGift model)
        {
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditPointGift>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Deletes(ListPointGifts request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Delete(int id, ListPointGifts request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.PointGift)]
        public virtual ActionResult List(ListPointGifts request)
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
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Suspend(int id, ListPointGifts request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Suspends(ListPointGifts request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Resume(int id, ListPointGifts request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointGiftEdit)]
        public ActionResult Resumes(ListPointGifts request)
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