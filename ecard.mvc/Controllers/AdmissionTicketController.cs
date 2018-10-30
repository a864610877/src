using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.AdmissionTickets;
using Ecard.Mvc.ViewModels;
using Ecard.Requests;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class AdmissionTicketController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public AdmissionTicketController(IUnityContainer unityContaine)
        {
            this._unityContainer = unityContaine;
        }
       [CheckPermission(Permissions.AdmissionTicketEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<AdmissionTicketEdit>();
            model.Read(id);
            return View(new EcardModelItem<AdmissionTicketEdit>(model));
        }

       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketEdit)]
       public ActionResult Edit([Bind(Prefix = "Item")] AdmissionTicketEdit model)
       {
           IMessageProvider msg = null;
           if (ModelState.IsValid(model))
           {
               this.ModelState.Clear();
               msg = model.Save();
           }
           return View(new EcardModelItem<AdmissionTicketEdit>(model, msg));
       }
       [CheckPermission(Permissions.AdmissionTicketList)]
       public ActionResult List(ListAdmissionTickets request)
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
       public ActionResult ListPost(AdmissionTicketRequest request)
       {
           var createRole = _unityContainer.Resolve<ListAdmissionTickets>();
           string pageHtml = string.Empty;
           var datas = createRole.AjaxGet(request, out pageHtml);
           return Json(new { tables = datas, html = pageHtml });
       }
        [CheckPermission(Permissions.AdmissionTicketCreate)]
       public ActionResult Create()
       {
           var createAccount = _unityContainer.Resolve<AdmissionTicketCreate>();
           var model = new EcardModelItem<AdmissionTicketCreate>(createAccount);
           return View(model);
       }

       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketCreate)]
       public ActionResult Create([Bind(Prefix = "Item")] AdmissionTicketCreate model)
       {
           IMessageProvider msg = null;
           if (ModelState.IsValid)
           {
               this.ModelState.Clear();
               msg = model.Save();
           }
           return View(new EcardModelItem<AdmissionTicketCreate>(model, msg));
       }

       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketSuspend)]
       public ActionResult Suspend(int id)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
           return Json(request.Suspends(id));
       }
       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketSuspend)]
       public ActionResult Suspends(string strIds)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
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
                       result = request.Suspends(intId);
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
           result.CodeText = "停售成功" + successCount + "个门票,失败" + errorCount + "个";
           return Json(result);
       }

       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketResume)]
       public ActionResult Resume(int id)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
           return Json(request.Resumes(id));
       }
       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketResume)]
       public ActionResult Resumes(string strIds)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
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
                       result = request.Resumes(intId);
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
           result.CodeText = "启用成功" + successCount + "个门票,失败" + errorCount + "个";
           return Json(result);
       }

       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketDelete)]
       public ActionResult Delete(int id)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
           return Json(request.Deletes(id));
       }
       [HttpPost]
       [CheckPermission(Permissions.AdmissionTicketDelete)]
       public ActionResult Deletes(string strIds)
       {
           var request = _unityContainer.Resolve<ListAdmissionTickets>();
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
                       result = request.Deletes(intId);
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
           result.CodeText = "删除成功" + successCount + "个门票,失败" + errorCount + "个";
           return Json(result);
       }
    }


}
