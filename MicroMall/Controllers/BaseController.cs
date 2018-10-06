using MicroMall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroMall.Controllers
{
    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {          
            if (Request.Cookies[SessionKeys.USERID] == null || Request.Cookies[SessionKeys.USERID].Value.ToString() == "")
            {
                RedirectToAction("Index", "login");
                //return Json(new ResultMessage() { Code = -2, Msg = "/login/Index" });
            }
            base.OnActionExecuting(filterContext);
        }
        //
        // GET: /Base/

        public ActionResult Index()
        {
            return View();
        }

    }
}
