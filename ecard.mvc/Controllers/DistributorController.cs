using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Distributors;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;

namespace Ecard.Mvc.Controllers
{
    /// <summary>
    /// 经销商
    /// </summary>
    [Authorize]
    public class DistributorController : Controller
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDistributorService _distributorService;
        private readonly LogHelper _logger;
        [Dependency]
        [NoRender]
        public SecurityHelper SecurityHelper { get; set; }

        public DistributorController(IUnityContainer unityContainer, IDistributorService distributorService, LogHelper logger)
        {
            _unityContainer = unityContainer;
            _distributorService = distributorService;
            _logger = logger;
        }

        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Create()
        {
            var createDistributor = _unityContainer.Resolve<CreateDistributor>();
            createDistributor.Ready();
            var model = new EcardModelItem<CreateDistributor>(createDistributor);
            return View(model);
        } 

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Create([Bind(Prefix = "Item")]CreateDistributor distributor)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(distributor))
            {
                this.ModelState.Clear();
                if (distributor.AccountLevelPolicyRates.Rate < 100m&&distributor.AccountLevelPolicyRates.Rate>=0)
                {
                    msg = distributor.Create();
                    //var user = this.SecurityHelper.GetCurrentUser();
                    //if (user is AdminUserModel)
                    //{
                    //    //系统总部设置总经销商
                    //    msg = distributor.Create();
                    //}
                    //else if (user is DistributorUserModel)
                    //{
                    //    var u = _distributorService.GetByUserId(user.CurrentUser.UserId);
                    //    var list = _distributorService.GetAccountLevelPolicyRates(u.DistributorId);
                    //    decimal r = list.FirstOrDefault().Rate * 100;
                    //    if (r > distributor.AccountLevelPolicyRates.Rate)
                    //    {
                    //        msg = distributor.Create();
                    //        //return RedirectToAction("List");
                    //    }
                    //    new Exception("提成比例大于上级经销商提成比例");
                    //}
                }
                else { new Exception("提成比例只能0-100之间"); }
                //msg = distributor.Create();
                distributor = _unityContainer.Resolve<CreateDistributor>();
            }
            distributor.Ready();
            return View(new EcardModelItem<CreateDistributor>(distributor, msg));
        }
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditDistributor>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<EditDistributor>(model));
        }
        [CheckPermission(Permissions.Distributor)]
        public ActionResult View(int id)
        {
            var model = _unityContainer.Resolve<ViewDistributor>();
            model.Read(id);
            return View(new EcardModelItem<ViewDistributor>(model));
        }
         
        //public ActionResult Names(string name)
        //{
        //    return Json(this._distributorService.QueryByName(name ?? "").ToList(0, 20, "Name")
        //                    .Select(x => new AutoCompletedItem { Text = string.Format("{0} ({1})", x.DisplayName, x.Name), Value = x.Name }), JsonRequestBehavior.AllowGet);
        //}
        //验证
        [HttpGet]
        [OutputCache(Location=System.Web.UI.OutputCacheLocation.None)]
        public JsonResult CheckPolicyRate()
        {
            decimal PolicyRate = decimal.Parse(Request["Item.PolicyRate"]);
            bool result = false;
            //string result = "数值只能在0.00-99.99之间，且不能大于上级的提成比例";
            //只能低于上级经销商的提成比例，上级经销商就是当前登陆的用户。、、
            if (PolicyRate < 100m&&PolicyRate>0)
            {
                result = true;
                //var user = this.SecurityHelper.GetCurrentUser();
                //if (user is AdminUserModel)
                //{
                //    //系统总部设置总经销商
                //    result = true;

                //}
                //else if (user is DistributorUserModel)
                //{
                //    var u = _distributorService.GetByUserId(user.CurrentUser.UserId);
                //    //u.DistributorId
                //    var list = _distributorService.GetAccountLevelPolicyRates(u.DistributorId);
                //    decimal r = list.FirstOrDefault().Rate*100;
                //    if (r > PolicyRate)
                //    {
                //        result = true;
                //    }
                //}
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")]EditDistributor userModel)
        {
            IMessageProvider msg = null;

           
            if (ModelState.IsValid(userModel))
            {
                this.ModelState.Clear();
                //----------------
                if (userModel.AccountLevelPolicyRates.Rate < 100m&&userModel.AccountLevelPolicyRates.Rate>0)
                {
                    msg = userModel.Save();
                    //var user = this.SecurityHelper.GetCurrentUser();
                    //if (user is AdminUserModel)
                    //{
                    //    //系统总部设置总经销商
                    //    msg = userModel.Save();
                    //}
                    //else if (user is DistributorUserModel)
                    //{
                    //    var u = _distributorService.GetByUserId(user.CurrentUser.UserId);
                    //    var list = _distributorService.GetAccountLevelPolicyRates(u.DistributorId);
                    //    decimal r = list.FirstOrDefault().Rate * 100;
                    //    if (r > userModel.AccountLevelPolicyRates.Rate)
                    //    {
                    //        msg = userModel.Save();
                    //        //return RedirectToAction("List");
                    //    }
                    //    new Exception("提成比例大于上级经销商提成比例");
                    //}
                }
                else { new Exception("提成比例只能0-100之间"); }
                //-------------
                
                //msg = userModel.Save();
                //return RedirectToAction("List");
            }
            userModel.Ready();
            return View(new EcardModelItem<EditDistributor>(userModel, msg));
        }

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Deletes(string strIds, ListDistributors request)
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
            result.CodeText = "删除成功" + successCount + "个经销商,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Suspend(int id, ListDistributors request)
        {
            return Json(request.Suspend(id));
        }

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Suspends(string strIds, ListDistributors request)
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
            result.CodeText = "停用成功" + successCount + "个经销商,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Resume(int id, ListDistributors request)
        {
            return Json(request.Resume(id));
        }
        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Resumes(string strIds, ListDistributors request)
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
            result.CodeText = "启用成功" + successCount + "个经销商,失败" + errorCount + "个";
            return Json(result);
        }

        [HttpPost]
        [CheckPermission(Permissions.DistributorEdit)]
        public ActionResult Delete(int id, ListDistributors request)
        {
            return Json(request.Delete(id));
        }

        [CheckPermission(Permissions.Distributor)]
        public ActionResult Export(ListDistributors request)
        {
            _logger.LogWithSerialNo(LogTypes.DistributorExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
        [CheckPermission(Permissions.Distributor)]
        public virtual ActionResult List(ListDistributors request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                request.Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            return View("List", request);
        }
        [HttpPost]
        public ActionResult ListPost(UserRequest request)
        {
            var createRole = _unityContainer.Resolve<ListDistributors>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
        [HttpPost]
        [CheckPermission(Permissions.SystemMessagePanelAccount)]
        public ActionResult SendSmsMessage(ListDistributors request)
        {
            return List(request);
        }
    }
}