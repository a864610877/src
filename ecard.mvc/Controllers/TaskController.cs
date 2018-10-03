using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Commands;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Tasks;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class TaskController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public TaskController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        [CheckPermission(Permissions.TaskRecharging)]
        public ActionResult Recharging(ListTasks request)
        {
            request.CommandType = Task.GetCommandType(typeof(RechargingCommand));
            ViewData["action"] = "Recharging";
            return List(request);
        }
        [CheckPermission(Permissions.TaskLimitAmount)]
        //[DashboardItem]
        public ActionResult LimitAmount(ListTasks request)
        {
            request.CommandType = Task.GetCommandType(typeof(LimitAmountCommand));
            ViewData["action"] = "LimitAmount";
            return List(request);
        }
        //[DashboardItem]
        ActionResult List(ListTasks request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }

        [HttpPost]
        [CheckPermission(Permissions.TaskRecharging)]
        public ActionResult ApproveRecharging(int id, ListTasks request)
        {
            request.Approve<RechargingCommand>(id, LogTypes.ApproveRecharging);
            return Recharging(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.TaskRecharging)]
        public ActionResult RefuseRecharging(int id, ListTasks request)
        {
            request.Reject<RechargingCommand, RefuseRechargeCommand>(id, LogTypes.RefuseRecharging);
            return Recharging(request);
        }
        [HttpPost]
        [CheckPermission(Permissions.TaskLimitAmount)]
        public ActionResult ApproveLimitAmount(int id, ListTasks request)
        {
            request.Approve<LimitAmountCommand>(id, LogTypes.ApproveLimitAmount);
            return LimitAmount(request);
        }
        [HttpPost]
        [CheckPermission(Permissions.TaskLimitAmount)]
        public ActionResult RefuseLimitAmount(int id, ListTasks request)
        {
            request.Reject<LimitAmountCommand, RefuseCommand>(id, LogTypes.RefuseLimitAmount);
            return LimitAmount(request);
        }
    }
}
