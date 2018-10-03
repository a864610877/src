using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PointPolicies;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PointPolicyController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;

        public PointPolicyController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Create()
        {
            var createPointPolicy = _unityContainer.Resolve<CreatePointPolicy>();
            createPointPolicy.Ready();
            var model = new EcardModelItem<CreatePointPolicy>(createPointPolicy);
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreatePointPolicy model)
        {
            IMessageProvider msg = null; 
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreatePointPolicy>(); 
            }
            model.Ready();
            return View(new EcardModelItem<CreatePointPolicy>(model, msg));
        }

        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditPointPolicy>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditPointPolicy>(model));
        }


        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditPointPolicy model)
        {
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditPointPolicy>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Deletes(ListPointPolicies request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Delete(int id, ListPointPolicies request)
        {
            request.Delete(id);
            return List(request);
        }

        
        [CheckPermission(Permissions.PointPolicy)]
        public virtual ActionResult List(ListPointPolicies request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Suspend(int id, ListPointPolicies request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Suspends(ListPointPolicies request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Resume(int id, ListPointPolicies request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PointPolicyEdit)]
        public ActionResult Resumes(ListPointPolicies request)
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
