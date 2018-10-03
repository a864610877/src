using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Liquidates;
using Ecard.Mvc.ViewModels;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class LiquidateController : BaseController
    {
        private readonly IUnityContainer _unityContainer;

        public LiquidateController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        [CheckPermission(Permissions.Liquidate)]
        public ActionResult List(ListLiquidates request)
        {
            request.Query();
            return View("List", request);
        }

        [CheckPermission(Permissions.Liquidate)]
        public ActionResult View(LiquidateModel model)
        {
            model.Ready();
            return View("view", new EcardModelItem<LiquidateModel>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.Liquidate)]
        public ActionResult Delete(DeleteLiquidate model)
        {
            SimpleAjaxResult result = model.Ready();

            IMessageProvider msg = null;
            if (!result.Success)
            {
                var vm = _unityContainer.Resolve<LiquidateModel>();
                vm.Id = model.Id;
                vm.Ready();

                msg = new SimpleMessageProvider(MessageType.Error, new[] { result.Message });
                return View("view", new EcardModelItem<LiquidateModel>(vm, msg));
            }
            else
            {
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [CheckPermission(Permissions.Liquidate)]
        public ActionResult Done([Bind(Prefix = "Item")] DoneLiquidate model)
        {
            SimpleAjaxResult result = model.Ready();


            var vm = _unityContainer.Resolve<LiquidateModel>();
            vm.Id = model.LiquidateId;
            vm.Ready();
            IMessageProvider msg = null;
            if (!result.Success)
            {
                msg = new SimpleMessageProvider(MessageType.Error, new[] { result.Message });
                return View("view", new EcardModelItem<LiquidateModel>(vm, msg));
            }
            else
            {
                msg = new SimpleMessageProvider(MessageType.Message, new[] { "结算成功" });
                return View("view", new EcardModelItem<LiquidateModel>(vm, msg));
            }
        }
    }
}