using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.CashDealLogSummarys;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class CashDealLogSummaryController : Controller
    {
        private readonly IUnityContainer _unityContainer;

        public CashDealLogSummaryController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
         
        [CheckPermission(Permissions.CashDealLogSummary)]
        public virtual ActionResult List(ListCashDealLogSummarys request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }  
    }
}