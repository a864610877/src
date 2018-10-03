using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers
{
    public class TestController:BaseController
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Welcome to ASP.NET MVC!";
            //DashboardHome home = _container.Resolve<DashboardHome>();
            //home.Ready();
            return View();
        }
    }
}
