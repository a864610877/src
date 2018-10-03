using System.Web.Mvc;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PrintTickets;

namespace Ecard.Mvc.Controllers
{
    public class UnityController : Controller
    {
        public ActionResult Print()
        {
            return Content("");
        }
    }
}