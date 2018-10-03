using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Liquidates;
using Ecard.Mvc.Models.Rollbacks;
using Ecard.Mvc.ViewModels;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class RollbackController : BaseController
    {
        private readonly IUnityContainer _unityContainer;

        public RollbackController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }


        [HttpPost]
        [CheckPermission(Permissions.RollbackApply)]
        public ActionResult Query([Bind(Prefix = "Item")]AddRollback model)
        {
            model.Ready();
            return View("add", new EcardModelItem<AddRollback>(model));
        }
        [CheckPermission(Permissions.RollbackApply)]
        public ActionResult Apply()
        {
            return View("add", new EcardModelItem<AddRollback>(new AddRollback()));
        }
        [HttpPost]
        [CheckPermission(Permissions.RollbackApply)]
        [CheckUserType(typeof(AdminUser))]
        public ActionResult DoApply([Bind(Prefix = "Item")]AddRollback model)
        {
            model.Apply();
            return View("add", new EcardModelItem<AddRollback>(model));
        }
        [CheckPermission(Permissions.Rollback)]
        public ActionResult List(ListRollbacks request)
        {
            request.Query();
            return View("List", request);
        }

        [CheckPermission(Permissions.Rollback)]
        public ActionResult View(RollbackModel request)
        {
            request.Ready();
            return View("view", new EcardModelItem<RollbackModel>(request));
        }

        [HttpPost]
        [CheckPermission(Permissions.Rollback)]
        public ActionResult Delete([Bind(Prefix = "Item")]DeleteRollback model)
        {
            var result = model.Ready();

            IMessageProvider msg = null;
            if (!result.Success)
            {
                var vm = _unityContainer.Resolve<RollbackModel>();
                vm.Id = model.Id;
                vm.Ready();

                msg = new SimpleMessageProvider(MessageType.Error, new[] { result.Message });
                return View("view", new EcardModelItem<RollbackModel>(vm, msg));
            }
            else
            {
                return RedirectToAction("List");
            }
        }
        [HttpPost]
        [CheckPermission(Permissions.Rollback)]
        public ActionResult Done([Bind(Prefix = "Item")]DoneRollback model)
        {
            var result = model.Ready();


            var vm = _unityContainer.Resolve<RollbackModel>();
            vm.Id = model.Id;
            vm.Ready();
            IMessageProvider msg = null;
            if (!result.Success)
            {
                msg = new SimpleMessageProvider(MessageType.Error, new[] { result.Message });
                return View("view", new EcardModelItem<RollbackModel>(vm, msg));
            }
            else
            {
                msg = new SimpleMessageProvider(MessageType.Message, new[] { "Ω·À„≥…π¶" });
                return View("view", new EcardModelItem<RollbackModel>(vm, msg));
            }
        }
    }
}