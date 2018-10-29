using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.AdminUsers;
using Ecard.Mvc.Models.Shops;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IShopService _shopService;
        private readonly LogHelper _logger;

        public ShopController(IUnityContainer unityContainer, IShopService shopService, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _shopService = shopService;
            _logger = logger;
        }

        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Create()
        {
            var createShop = _unityContainer.Resolve<CreateShop>();
            createShop.Ready();
            var model = new EcardModelItem<CreateShop>(createShop);
            return View(model);
        } 

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Create([Bind(Prefix = "Item")]CreateShop shop)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(shop))
            {
                this.ModelState.Clear();

                msg = shop.Create();

                shop = _unityContainer.Resolve<CreateShop>();
            }
            shop.Ready();
            return View(new EcardModelItem<CreateShop>(shop, msg));
        }
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditShop>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditShop>(model));
        }
        [CheckPermission(Permissions.Shop)]
        public ActionResult View(int id)
        {
            var model = _unityContainer.Resolve<ViewShop>();
            model.Read(id);
            return View(new EcardModelItem<ViewShop>(model));
        }
         
        public ActionResult Names(string name)
        {
            return Json(this._shopService.QueryByName(name ?? "").ToList(0, 20, "Name")
                .Select(x => new AutoCompletedItem { Text = string.Format("{0} ({1})", x.DisplayName, x.Name), Value = x.Name }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")]EditShop userModel)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(userModel))
            {
                this.ModelState.Clear();
                msg = userModel.Save();
                return RedirectToAction("List");
            }
            userModel.Ready();
            return View(new EcardModelItem<EditShop>(userModel, msg));
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Deletes(string strIds, ListShops request)
        {
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Delete(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "删除成功" + successCount + "个商户,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Suspend(int id, ListShops request)
        {
            return Json(request.Suspend(id));
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Suspends(string strIds, ListShops request)
        {
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Suspend(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "停用成功" + successCount + "个商户,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Resume(int id, ListShops request)
        {
            return Json(request.Resume(id));
        }
        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Resumes(string strIds, ListShops request)
        {
            ResultMsg result = new ResultMsg();
            int successCount = 0;
            int errorCount = 0;
            if (!string.IsNullOrEmpty(strIds))
            {
                string[] sId = strIds.Split(',');
                foreach (var id in sId)
                {
                    int intId = 0;
                    if (int.TryParse(id, out intId))
                    {
                        result = request.Resume(intId);
                        if (result.Code == 1)
                        {
                            successCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
            }
            result.CodeText = "启用成功" + successCount + "个商户,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.ShopEdit)]
        public ActionResult Delete(int id, ListShops request)
        {
            return Json(request.Delete(id));
        }

        [CheckPermission(Permissions.Shop)]
        public ActionResult Export(ListShops request)
        {
            _logger.LogWithSerialNo(LogTypes.ShopExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
        [CheckPermission(Permissions.Shop)]
        [DashboardItem]
        public virtual ActionResult List(ListShops request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            return View("List", request);
        }
        [CheckPermission(Permissions.Shop)]
        [HttpPost]
        public  ActionResult ListPost(ShopRequest request)
        {
            var createRole = _unityContainer.Resolve<ListShops>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }

        public ActionResult SerachPos(int id)
        {
            var createRole = _unityContainer.Resolve<ListShops>();
           var listpost= createRole.GetListPos(id);
           if (listpost != null && listpost.Count > 0)
           {
               return Json(new { Code = 1, ListPost = listpost });
           }
           else
           {
               return Json(new { Code = 0});
           } 
        }
    }

    public class AutoCompletedItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
