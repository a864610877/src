using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.PosEndPointes;
using Ecard.Mvc.Models.Shops;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;


namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class PosEndPointController : Controller
    {
        private readonly IPosEndPointService _posEndPointService;
        private readonly IShopService _shopService;
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;

        public PosEndPointController(IPosEndPointService posEndPointService, IShopService shopService, IUnityContainer unityContainer, LogHelper logger)
        {
            _posEndPointService = posEndPointService;
            _shopService = shopService;
            _unityContainer = unityContainer;
            _logger = logger;
        }

        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Create(int? shopId)
        {
            var model = _unityContainer.Resolve<CreatePosEndPoint>();
            model.Ready(shopId);
            return View(new EcardModelItem<CreatePosEndPoint>(model));
        }
         
        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Create([Bind(Prefix = "Item")] CreatePosEndPoint model)
        {
            IMessageProvider msg = null;
            var shopId = model.Shop;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();
                msg = model.Save();
                model = _unityContainer.Resolve<CreatePosEndPoint>();
                model.Shop = shopId;
            }
            model.Ready(shopId);
            return View(new EcardModelItem<CreatePosEndPoint>(model, msg));
        }

        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Edit(int id)
        {
            var editPos = _unityContainer.Resolve<EditPosEndPoint>();
            editPos.Read(id);
            return View(new EcardModelItem<EditPosEndPoint>(editPos));
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditPosEndPoint userModel)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(userModel))
            {
                this.ModelState.Clear();
                msg = userModel.Save();
                return RedirectToAction("List");
            }
            return View(new EcardModelItem<EditPosEndPoint>(userModel, msg));
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Deletes(string strIds, ListPosEndPoints request)
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
            result.CodeText = "删除成功" + successCount + "个终端,失败" + errorCount + "个";
            return Json(result);
        }


        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Delete(int id, ListPosEndPoints request)
        {
            return Json(request.Delete(id));
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Suspends(string strIds, ListPosEndPoints request)
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
            result.CodeText = "停用成功" + successCount + "个终端,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Resume(int id, ListPosEndPoints request)
        {
            return Json(request.Resume(id));
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Resumes(string strIds, ListPosEndPoints request)
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
            result.CodeText = "启用成功" + successCount + "个终端,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.PosEdit)]
        public ActionResult Suspend(int id, ListPosEndPoints request)
        {
            return Json(request.Suspend(id));
        }
        [CheckPermission(Permissions.Pos)]
        public ActionResult Export(ListPosEndPoints request)
        {
            _logger.LogWithSerialNo(LogTypes.PosExport, SerialNoHelper.Create(), 0);
            return List(request);
        }

        [CheckPermission(Permissions.Pos)]
        [DashboardItem]
        public virtual ActionResult List(ListPosEndPoints request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                request.Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            request.Ready();
            return View("List", request);
        }
        [HttpPost]
        public ActionResult PostList(PosEndPointRequest request)
        {
            var createRole = _unityContainer.Resolve<ListPosEndPoints>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
    }
}

