using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Controllers.WeChat
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            string url = "";
            return Redirect(url);
        }

        public ActionResult Register(string code)
        {
            return View();
        }

       
    }
}
