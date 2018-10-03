using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FZ_Web.Controllers
{
    public class ccController : Controller
    {

         private readonly IUnityContainer _unityContainer;
         public ccController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
        //
        // GET: /cc/

        public ActionResult Index()
        {
            return View();
        }

    }
}
