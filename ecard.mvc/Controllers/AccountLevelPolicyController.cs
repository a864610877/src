using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.AccountLevelPolicies;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class AccountLevelPolicyController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;

        public AccountLevelPolicyController(IUnityContainer unityContainer, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Create()
        {
            var createAccountLevelPolicy = _unityContainer.Resolve<CreateAccountLevelPolicy>();
            createAccountLevelPolicy.Ready();
            var model = new EcardModelItem<CreateAccountLevelPolicy>(createAccountLevelPolicy);
            return View(model);
        }

        [CheckPermission(Permissions.AccountLevel)]
        public ActionResult Export(ListAccountLevelPolicies request)
        {
            _logger.LogWithSerialNo(LogTypes.AccountLevelExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateAccountLevelPolicy model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();
                var accountType = model.AccountType;
                msg = model.Create();

                model = _unityContainer.Resolve<CreateAccountLevelPolicy>();
                model.AccountType = accountType;
            }
            model.Ready();
            return View(new EcardModelItem<CreateAccountLevelPolicy>(model, msg));
        }

        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditAccountLevelPolicy>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditAccountLevelPolicy>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditAccountLevelPolicy item)
        {
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                item.Save();
                return RedirectToAction("List");
            }
            item.Ready();
            return View(new EcardModelItem<EditAccountLevelPolicy>(item));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Deletes(ListAccountLevelPolicies request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Delete(int id, ListAccountLevelPolicies request)
        {
            request.Delete(id);
            return List(request);
        }


        [CheckPermission(Permissions.AccountLevel )]
        public virtual ActionResult List(ListAccountLevelPolicies request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Suspend(int id, ListAccountLevelPolicies request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Suspends(ListAccountLevelPolicies request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Resume(int id, ListAccountLevelPolicies request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLevelEdit)]
        public ActionResult Resumes(ListAccountLevelPolicies request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Resume(id);
            }

            return List(request);
        }

    }
}
