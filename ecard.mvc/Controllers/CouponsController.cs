using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Couponss;
using Ecard.Mvc.ViewModels;
using Ecard.Requests;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    public class CouponsController : Controller
    {
         private readonly IUnityContainer _unityContainer;

         public CouponsController(IUnityContainer unityContaine)
         {
             this._unityContainer = unityContaine;
         }
        [CheckPermission(Permissions.CouponsList)]
        public ActionResult List(ListCoupons request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            return View("List", request);
        }
        [HttpPost]
        public ActionResult ListPost(CouponsRequest request)
        {
            var createRole = _unityContainer.Resolve<ListCoupons>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
        [CheckPermission(Permissions.CouponsCreate)]
        public ActionResult Create()
        {
            var createAccount = _unityContainer.Resolve<CouponCreate>();
            createAccount.Ready();
            var model = new EcardModelItem<CouponCreate>(createAccount);
            
            return View(model);
        }

        [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketCreate)]
       public ActionResult Create([Bind(Prefix = "Item")] CouponCreate model)
       {
           IMessageProvider msg = null;
           if (ModelState.IsValid)
           {
               this.ModelState.Clear();
               msg = model.Save();
           }
           return View(new EcardModelItem<CouponCreate>(model, msg));
       }

        [CheckPermission(Permissions.CouponsEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<CouponEdit>();
            model.Read(id);
            return View(new EcardModelItem<CouponEdit>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.CouponsEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] CouponEdit model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();
                msg = model.Save();
            }
            return View(new EcardModelItem<CouponEdit>(model, msg));
        }

        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketSuspend)]
        public ActionResult Suspend(int id)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            return Json(request.Suspend(id));
        }
        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketSuspend)]
        public ActionResult Suspends(string strIds)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Suspend(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "停用成功" + successCount + "个优惠卷,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketResume)]
        public ActionResult Resume(int id)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            return Json(request.Resume(id));
        }
        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketResume)]
        public ActionResult Resumes(string strIds)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Resume(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "启用成功" + successCount + "个优惠卷,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketDelete)]
        public ActionResult Delete(int id)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            return Json(request.Delete(id));
        }
        [HttpPost]
        [CheckPermission(Permissions.AdmissionTicketDelete)]
        public ActionResult Deletes(string strIds)
        {
            var request = _unityContainer.Resolve<ListCoupons>();
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Delete(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "删除成功" + successCount + "个优惠卷,失败" + errorCount + "个";
            return Json(result);
        }
    }
}
