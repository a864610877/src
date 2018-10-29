using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PointPolicies;
using Ecard.Mvc.Models.PointRebateLogs;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class PointRebateLogController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public PointRebateLogController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        } 

        [CheckPermission(Permissions.PointRebateLog)]
        public virtual ActionResult List(ListPointRebateLogs request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        //[HttpPost]
        //[CheckPermission(Permissions.PointRebateLogSuspend)]
        //public ActionResult Suspend(int id, ListPointRebateLogs request)
        //{
        //    request.Suspend(id);
        //    return List(request);
        //}

        //[HttpPost]
        //[CheckPermission(Permissions.PointRebateLogSuspend)]
        //public ActionResult Suspends(ListPointRebateLogs request)
        //{
        //    var ids = request.CheckItems.GetCheckedIds();
        //    foreach (var id in ids)
        //    {
        //        request.Suspend(id);
        //    }

        //    return List(request);
        //}

        //[HttpPost]
        //[CheckPermission(Permissions.PointRebateLogResume)]
        //public ActionResult Resume(int id, ListPointRebateLogs request)
        //{
        //    request.Resume(id);
        //    return List(request);
        //}

        //[HttpPost]
        //[CheckPermission(Permissions.PointRebateLogResume)]
        //public ActionResult Resumes(ListPointRebateLogs request)
        //{
        //    var ids = request.CheckItems.GetCheckedIds();
        //    foreach (var id in ids)
        //    {
        //        request.Resume(id);
        //    }

        //    return List(request);
        //}

    }
}