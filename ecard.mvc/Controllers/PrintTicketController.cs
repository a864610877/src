using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PrintTickets;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PrintTicketController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public PrintTicketController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }  

        [HttpPost]
        [CheckPermission(Permissions.PrintTicketDelete)]
        public ActionResult Deletes(ListPrintTickets request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PrintTicketDelete)]
        public ActionResult Delete(int id, ListPrintTickets request)
        {
            request.Delete(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.PrintTicketDelete)]
        public ActionResult Print(int id, ListPrintTickets request)
        {

            return Json(request.Print(id));
        }


        [CheckPermission(Permissions.PrintTicket)]
        public virtual ActionResult List(ListPrintTickets request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            request.Ready();
            return View("List", request);
        }
    }
}