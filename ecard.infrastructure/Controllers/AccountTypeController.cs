using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.AccountTypes;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class AccountTypeController : Controller
    {
        private readonly IUnityContainer _unityContainer; 

        public AccountTypeController(IUnityContainer unityContainer )
        {
            _unityContainer = unityContainer; 
        }

        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Create()
        {
            var createAccountType = _unityContainer.Resolve<CreateAccountType>();
            var model = new EcardModelItem<CreateAccountType>(createAccountType);
            return View(model);
        }
        [CheckPermission(Permissions.AccountType)]
        public ActionResult GetAccountTypeById(int id)
        {
            var accountTypeById = _unityContainer.Resolve<GetAccountTypeById>();
            accountTypeById.Read(id);
            return PartialView(new EcardModelItem<GetAccountTypeById>(accountTypeById));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateAccountType model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();

                msg = model.Create();
                model = _unityContainer.Resolve<CreateAccountType>();
            }
            return View(new EcardModelItem<CreateAccountType>(model, msg));
        }

        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditAccountType>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditAccountType>(model));
        }


        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditAccountType model)
        {
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();
                model.Save();
                return RedirectToAction("List");
            }
            model.Ready();
            return View(new EcardModelItem<EditAccountType>(model));
        } 
        [CheckPermission(Permissions.AccountType)]
        [DashboardItem]
        public virtual ActionResult List(ListAccountTypes request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                request.Query();
            }
            return View("List", request);
        }


        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Suspend(int id, ListAccountTypes request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Suspends(ListAccountTypes request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Resume(int id, ListAccountTypes request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountTypeEdit)]
        public ActionResult Resumes(ListAccountTypes request)
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