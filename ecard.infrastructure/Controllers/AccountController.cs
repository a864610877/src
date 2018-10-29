using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.Accounts;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit.Data;
using Ecard.SqlServices;
namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IShopService _shopService;
        private readonly IAccountService _accountService;
        private readonly IUnityContainer _unityContainer;
        private readonly LogHelper _logger;
        private readonly IMembershipService _membershipService;
        private readonly IDealWayService _dealWayService;

        public AccountController(IAccountService accountService,
            IUnityContainer unityContainer,
            LogHelper logger,
            IMembershipService membershipService, IDealWayService dealWayService, IShopService shopService)
        {
            _accountService = accountService;
            _unityContainer = unityContainer;
            _logger = logger;
            _membershipService = membershipService;
            _dealWayService = dealWayService;
            _shopService = shopService;
            ;
        }
        [CheckPermission(Permissions.AccountInit)]
        public ActionResult Init()
        {
            var createAccount = _unityContainer.Resolve<InitAccount>();
            createAccount.Ready();
            var model = new EcardModelItem<InitAccount>(createAccount);
            return View(model);
        }

        //[CheckPermission(Permissions.AccountQuery)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult Account(AccountModel model)
        {
            model.Ready();
            return View("Account", model);
        }

        [CheckPermission(Permissions.Account)]
        public ActionResult View(int id)
        {
            var model = _unityContainer.Resolve<ViewAccount>();
            model.Read(id);
            return View(new EcardModelItem<ViewAccount>(model));
        }
        [CheckPermission(Permissions.AccountRebate)]
        public ActionResult Rebate(int id)
        {
            var model = _unityContainer.Resolve<RebateAccount>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<RebateAccount>(model));
        }
        [CheckPermission(Permissions.AccountTransfer)]
        public ActionResult Transfer()
        {
            var model = _unityContainer.Resolve<TransferAccount>();
            model.Ready();
            return View(new EcardModelItem<TransferAccount>(model));
        }
        [CheckPermission(Permissions.AccountTransfer)]
        [HttpPost]
        public ActionResult Transfer(TransferAccount model)
        {
            return Json(model.Save(), JsonRequestBehavior.AllowGet);
        }
        [CheckPermission(Permissions.AccountGift)]
        public ActionResult Gift()
        {
            var model = _unityContainer.Resolve<GiftAccount>();
            model.Query();
            model.Ready();
            return View(model);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountGift)]
        public ActionResult Gift(GiftAccount model)
        {
            model.Query();
            model.Ready();
            return PartialView("giftlist", model);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountGift)]
        public ActionResult DoGift(DoGiftAccount model)
        {
            return Json(model.Save(), JsonRequestBehavior.AllowGet);
        }
        [CheckPermission(Permissions.AccountRecharge)]
        public ActionResult Recharging()
        {
            var model = _unityContainer.Resolve<RechargingAccount>();
            model.Ready();
            return View(new EcardModelItem<RechargingAccount>(model));
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountRecharge)]
        public ActionResult Recharging(RechargingAccount model)
        {
            try
            {
                AccountServiceResponse rsp = model.DoRecharge();
                rsp.CodeText = string.IsNullOrEmpty(rsp.CodeText) ? ModelHelper.GetBoundText(rsp, x => x.Code) : rsp.CodeText;
                return Json(rsp);
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountRecharge, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountRecharge, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }


        [CheckPermission(Permissions.AreaRecharing)]
        public ActionResult AreaRecharing()
        {

            var model = _unityContainer.Resolve<RechargingAccount>();
            model.Ready();
            return View(new EcardModelItem<RechargingAccount>(model));
        }
        [HttpPost]
        [CheckPermission(Permissions.AreaRecharing)]
        public ActionResult AreaRecharing(RechargingAccount model)
        {
            try
            {
                string pageHtml = string.Empty;
                int tatolCount = 0;
                List<RechargingLog> rsp = model.AreaRecharges(out pageHtml, out tatolCount);

                return Json(new { ModelList = rsp, html = pageHtml, tatolCount = tatolCount });
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountRecharge, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountRecharge, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [HttpPost]
        public ActionResult RecharingLog(RechargingAccount model)
        {
            int _pageIndex = model.pageIndex == null ? 1 : (int)model.pageIndex;
            int _pageSize = model.pageSize == null ? 10 : (int)model.pageSize;
            string tatol = Request["tatolCount"];
            int tatolCount = 0;
            if (int.TryParse(tatol, out tatolCount))
            {

            }

            Database database = new Database("ecard");
            var instance = database.OpenInstance();
            var _rechargingLogService = new SqlRechargingLogService(instance);


            string pageHtml = string.Empty;
            var rsp = _rechargingLogService.Query(model.AccountName, _pageIndex, _pageSize).ToList();
            pageHtml = MvcPage.AjaxPager(_pageIndex, _pageSize, tatolCount);
            return Json(new { ModelList = rsp, html = pageHtml });
        }

        [CheckPermission(Permissions.AccountRenew)]
        public ActionResult Renew()
        {
            var model = _unityContainer.Resolve<RenewAccount>();
            return View(new EcardModelItem<RenewAccount>(model));
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountRenew)]
        public ActionResult Renew(RenewAccount model)
        {
            try
            {
                AccountServiceResponse rsp = model.Save();
                if (string.IsNullOrEmpty(rsp.CodeText))
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountRenew, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountRenew, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }

        [CheckPermission(Permissions.AccountChangeName)]
        public ActionResult ChangeName()
        {
            var model = _unityContainer.Resolve<ChangeNameAccount>();
            return View(new EcardModelItem<ChangeNameAccount>(model));
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountChangeName)]
        public ActionResult ChangeName(ChangeNameAccount model)
        {
            try
            {
                AccountServiceResponse rsp = model.Save();
                if (string.IsNullOrWhiteSpace(rsp.CodeText))
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountChangeName, ex);

                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountChangeName, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountChangePassword)]
        public ActionResult ChangeAccountPassword(ChangePasswordAccount model, int id)
        {
            var status=model.Save(id);
            if(status.Success){
                return Json(new {  CodeText="修改成功", Code=1});
            }
            return Json(new { CodeText = "修改失败", Code = 0});
        }

        [CheckUserType(typeof(AccountUser))]
        [CheckPermission(Permissions.AccountChangePassword)]
        public ActionResult ChangePassword()
        {
            var model = _unityContainer.Resolve<ChangeSelfPasswordAccount>();
            model.Ready();
            return View(new EcardModelItem<ChangeSelfPasswordAccount>(model));
        }

        [HttpPost]
        [CheckUserType(typeof(AccountUser))]
        [CheckPermission(Permissions.AccountChangePassword)]
        public ActionResult ChangePassword([Bind(Prefix = "Item")]ChangeSelfPasswordAccount model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                msg = model.Save();
            }
            model.Ready();
            return View(new EcardModelItem<ChangeSelfPasswordAccount>(model, msg));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountRebate)]
        public ActionResult Rebate([Bind(Prefix = "Item")]RebateAccount model, int id)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid)
            {
                this.ModelState.Clear();
                msg = model.Save();
                model.Read(id);
            }
            model.Ready();
            return View(new EcardModelItem<RebateAccount>(model, msg));
        }
        [CheckPermission(Permissions.AccountOwner)]
        public ActionResult Owner(int id)
        {
            var model = _unityContainer.Resolve<OwnerAccount>();
            model.Read(id);
            model.Ready();
            return View(new EcardModelItem<OwnerAccount>(model));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountOwner)]
        public ActionResult Owner([Bind(Prefix = "Item")] OwnerAccount model)
        {
            IMessageProvider msg = null;
            if (ModelState.IsValid(model))
            {
                this.ModelState.Clear();
                msg = model.Save();
            }
            model.Ready();
            return View(new EcardModelItem<OwnerAccount>(model, msg));
        }
        [CheckPermission(Permissions.AccountOpen)]
        public ActionResult Open()
        {
            var openAccount = _unityContainer.Resolve<OpenAccount>();
            openAccount.Ready();
            var model = new EcardModelItem<OpenAccount>(openAccount);
            return View(model);
        }

        [CheckPermission(Permissions.AccountClose)]
        public ActionResult Close()
        {
            var closeAccount = _unityContainer.Resolve<CloseAccount>();
            var model = new EcardModelItem<CloseAccount>(closeAccount);
            return View(model);
        }

        [CheckPermission(Permissions.AccountQuery)]
        public ActionResult Query(QueryAccount item)
        {
            try
            {
                //var account = _accountService.Query("Owner").FirstOrDefault(x => x.Name == item.AccountName
                //    && x.AccountToken == item.AccountToken
                //    && (x.State == AccountStates.Normal || x.State == AccountStates.Invalid));
                var account = _accountService.QueryAccount(item.AccountName, item.AccountToken).FirstOrDefault();

                if (account == null)
                {
                    var rsp = new AccountServiceResponse(ResponseCode.NonFoundAccount);

                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                    return Json(rsp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    User owner = null;
                    if (account.OwnerId.HasValue)
                        owner = _membershipService.GetUserById(account.OwnerId.Value) as AccountUser;
                    var rsp = new AccountServiceResponse(ResponseCode.Success, null, _shopService.GetById(account.ShopId), account, owner);

                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                    return Json(rsp, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountQuery, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountQuery, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [CheckPermission(Permissions.AccountQueryWithUserInfo)]
        public ActionResult QueryWithUserInfo(QueryAccount item)
        {
            try
            {
                //var account = _accountService.Query("Owner").Where(x => (x.State == AccountStates.Normal || x.State == AccountStates.Invalid)
                //    && (x.Name == item.AccountName || x.Owner.DisplayName == item.AccountName || x.Owner.Mobile == item.AccountName)
                //    ).ToList();
                var account = string.IsNullOrWhiteSpace(item.AccountName) ? new List<Account>() :
                    _accountService.QueryForName(new AccountWithNameRequest()
                                                               {
                                                                   States = new[] { AccountStates.Normal, AccountStates.Invalid },
                                                                   Name = item.AccountName,
                                                                   OwnerDisplayName = item.AccountName,
                                                                   OwnerMobile = item.AccountName
                                                               }).ToList();
                if (account.Count == 0)
                {
                    var rsp = new AccountServiceResponse(ResponseCode.NonFoundAccount);

                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                    return Json(rsp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var users = _membershipService.QueryUsers<AccountUser>(new UserRequest() { UserIds = account.Where(x => x.OwnerId.HasValue).Select(x => x.OwnerId.Value).ToArray() }).ToList();

                    if (account.Count == 1)
                    {
                        var single = account.Single();
                        var rsp = new AccountServiceResponse(ResponseCode.Success, null, _shopService.GetById(single.ShopId), single, users.FirstOrDefault());

                        rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                        return Json(rsp, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = from x in account
                                   let owner = x.OwnerId.HasValue
                                                   ? users.FirstOrDefault(y => y.UserId == x.OwnerId.Value)
                                                   : null

                                   select new
                                   {
                                       AccountName = x.Name,
                                       OwnerMobile = (owner == null ? "" : owner.Mobile) ?? "",
                                       OwnerDisplayName = (owner == null ? "" : owner.DisplayName) ?? "",
                                       OwnerIdentity = (owner == null ? "" : owner.IdentityCard) ?? "",
                                   };

                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountQueryWithUserInfo, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountQueryWithUserInfo, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [CheckPermission(Permissions.AccountQueryWithoutToken)]
        public ActionResult QueryWithoutToken(QueryAccount item)
        {
            try
            {
               // var account = _accountService.GetByName(item.AccountName);
                Account account = null;
                var accountUser = (AccountUser)_membershipService.GetByMobile(item.AccountName);
                if (accountUser != null)
                {
                    account = _accountService.QueryByOwnerId(accountUser).FirstOrDefault();
                    //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
                }
                else
                {
                    account = _accountService.GetByName(item.AccountName);
                }
                if (account == null || (account.State != AccountStates.Normal && account.State != AccountStates.Invalid))
                {
                    var rsp = new AccountServiceResponse(ResponseCode.NonFoundAccount);

                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                    return Json(rsp, JsonRequestBehavior.AllowGet);
                }
                var user = account.OwnerId.HasValue ? _membershipService.GetUserById(account.OwnerId.Value) : null;
                {

                    var rsp = new AccountServiceResponse(ResponseCode.Success, null, _shopService.GetById(account.ShopId), account, user);
                    
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                    rsp.CodeText = "查询成功";
                    return Json(rsp, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountQueryWithoutToken, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountQueryWithoutToken, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountOpen)]
        public ActionResult Open(OpenAccount model)
        {
            try
            {
                AccountServiceResponse rsp = model.Save();
                if (string.IsNullOrEmpty(rsp.CodeText))
                {
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                }
                return Json(rsp);
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountOpen, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountOpen, ex);

                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }

        //[HttpPost]
        //[CheckPermission(Permissions.AccountOpens)]
        //public ActionResult Opens(ListAccounts request)
        //{
        //    int count = 0;
        //    var ids = request.CheckItems.GetCheckedIds();
        //    foreach (var id in ids)
        //    {
        //        if (request.Open(id))
        //            count++;
        //    }

        //    request.LogOpenSuccess(count);
        //    return List(request);
        //}
        [HttpPost]
        [CheckPermission(Permissions.AccountClose)]
        public ActionResult Close(CloseAccount model)
        {
            try
            {
                AccountServiceResponse rsp = model.Save();
                if (string.IsNullOrWhiteSpace(rsp.CodeText))
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (ConflictException ex)
            {
                _logger.Error(LogTypes.AccountClose, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.AccountConflict);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
            catch (Exception ex)
            {
                _logger.Error(LogTypes.AccountClose, ex);
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountInit)]
        public ActionResult Init([Bind(Prefix = "Item")] InitAccount model, [CurrentShop]Shop shop)
        { 
            IMessageProvider msg = null;
            //if (model.Distributor == -10001)
            //{
            //    model.LogSetdistributorError();
            //    return View(new EcardModelItem<InitAccount>(model, msg));
            //}

            if (ModelState.IsValid)
            {
                this.ModelState.Clear();

                msg = model.CreateAccounts();
            }
            model.Ready();
            return View(new EcardModelItem<InitAccount>(model, msg));
        }

        [CheckPermission(Permissions.AccountCreate)]
        public ActionResult Create( string strIds, ListAccounts request)
        {
            ResultMsg result = new ResultMsg();
          strIds=  System.Web.HttpContext.Current.Request["data"];
            if (!string.IsNullOrEmpty(strIds))
            {
                int[] ids;
                string[] sId = strIds.Split(',');
               ids= Array.ConvertAll<string, int>(sId, s => int.Parse(s));
               var items = _accountService.Query(new AccountRequest() { State = AccountStates.Initialized, Ids = ids }).ToList();

               return View("Create", new CreateAccount() { List = items });
            }

            return View("Create", new CreateAccount() { List = null });
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountCreate)]
        public ActionResult Creates(string strIds, ListAccounts request)
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
                        result = request.Create(intId);
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
            result.CodeText = "建卡成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
             
        }

        [CheckPermission(Permissions.AccountCreate)]
        [HttpPost]
        public ActionResult DoCreate(DoCreateAccount account)
        {
            return Json(account.Save(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountApprove)]
        public ActionResult Approves(string strIds, ListAccounts request)
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
                        result = request.Approve(intId);
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
            result.CodeText = "审核成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountApprove)]
        public ActionResult Opens(string strIds, ListAccounts request)
        {
            var dealway = _dealWayService.Query().FirstOrDefault();
            if (dealway == null)
                throw new Exception("请先创建支付方式");
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
                        result = request.Open(intId, dealway);
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
            result.CodeText = "批量会员卡发放成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountApprove)]
        public ActionResult Approve(int id, ListAccounts request)
        {
            request.Approve(id);

            return List(request);
        }
         
        [CheckPermission(Permissions.AccountReport)]
        public ActionResult Export(ListAccounts request)
        {
            _logger.LogWithSerialNo(LogTypes.AccountExport, SerialNoHelper.Create(), 0);
            return List(request);
        }
        [HttpPost]
        [CheckPermission(Permissions.SystemMessagePanelAccount)]
        public ActionResult SendSmsMessage(ListAccounts request)
        { 
            request.SendMessage();

            return List(request);
        }
        [CheckPermission(Permissions.Account)]
        [DashboardItem]
        public ActionResult List(ListAccounts request)
        {
            string pageHtml = string.Empty;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.New_Query(out pageHtml);
                ViewBag.pageHtml = MvcHtmlString.Create(pageHtml);
            }
            request.Ready();
            return View("List", request);
        }
        [HttpPost]
        public ActionResult ListPost(AccountRequest request)
        {
            var createRole = _unityContainer.Resolve<ListAccounts>();
            string pageHtml = string.Empty;
            var datas = createRole.AjaxGet(request, out pageHtml);
            return Json(new { tables = datas, html = pageHtml });
        }
        //-------------------------
        [HttpPost]
        [CheckPermission(Permissions.Account)]
        public ActionResult SetDistributor(int id, AccountRequest request)
        {
            var createRole = _unityContainer.Resolve<ListAccounts>();
            ResultMsg result = new ResultMsg();
            if (request.ShopId == -10001)
            {
                result.CodeText = "请先选择归属经销商";
                return Json(result);
            }
            return Json(createRole.SetDistributorId(id, Convert.ToInt32(request.ShopId)));
        }
        [HttpPost]
        [CheckPermission(Permissions.Account)]
        public ActionResult SetDistributors(string strIds, AccountRequest request)
        {
            var createRole = _unityContainer.Resolve<ListAccounts>();
             ResultMsg result = new ResultMsg();
             if (request.ShopId == -10001)
            {
                result.CodeText = "请先选择归属经销商";
                return Json(result); 
            }
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
                        result = createRole.SetDistributorId(intId, Convert.ToInt32(request.ShopId));
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
            result.CodeText = "更改经销商成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result); 
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountSuspend)]
        public ActionResult Suspend(SuspendAccount model)
        {
            return Json(model.Save(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountSuspend)]
        public ActionResult Suspends(string strIds, ListAccounts request)
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
            result.CodeText = "停用成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
        }


        [HttpPost]
        [CheckPermission(Permissions.AccountDelete)]
        public ActionResult Deletes(string strIds, ListAccounts request)
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
            result.CodeText = "删除成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountDelete)]
        public ActionResult Delete(int id,ListAccounts request)
        {
            return Json(request.Delete(id));
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountResume)]
        public ActionResult Resume(ResumeAccount model)
        {
            return Json(model.Save(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountResume)]
        public ActionResult Resumes(string strIds, ListAccounts request)
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
            result.CodeText = "启用成功" + successCount + "个会员,失败" + errorCount + "个";
            return Json(result);
        }

        // apis
        [HttpGet]
        [CheckPermission(Permissions.AccountLimit)]
        [CheckUserType(typeof(AdminUser))]
        public ActionResult ChangeLimitAmount(string accountName)
        {
            ViewData["accountName"] = accountName;
            ViewData["accountToken"] = Session[Constants.Session_AccountToken];
            Session[Constants.Session_AccountToken] = null;
            return View("ChangeLimitAmount", CreatePageModel() );
        }

        [HttpPost]
        [CheckPermission(Permissions.AccountLimit)]
        [CheckUserType(typeof(AdminUser))]
        public ActionResult ChangeLimitAmount(ChangeLimitAmount model)
        {
            return Json(model.Ready(), JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(Permissions.AccountLimit)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult GetAccountLimit(AccountModel model)
        {
            model.Ready();
            return Json(model.Account.LimiteAmount, JsonRequestBehavior.AllowGet);
        }
        [CheckPermission(Permissions.AccountLimit)]
        [CheckUserType(typeof(ShopUser), typeof(AdminUser))]
        public ActionResult GotoChangeLimitAmount(int id)
        {
            Account account = _accountService.GetById(id);
            this.Session[Constants.Session_AccountToken] = account.AccountToken;
            return RedirectToAction("ChangeLimitAmount", new { AccountName = account.Name });
        }

        [HttpGet]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser))]
        public ActionResult SystemPay()
        {
            var item = _unityContainer.Resolve<SystemPayAccount>();
            item.Ready();
            var model = new EcardModelItem<SystemPayAccount>(item);
            return View("SystemPay1", model);
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser))]
        public ActionResult SystemPay(SystemPayAccount item)
        {
            //string serialNo = SerialNoHelper.Create();
            try
            {
                //var account = _accountService.GetByName(item.AccountName);
                //var rsp = account != null ? _accountDealService.Pay(new PayRequest(item.AccountName, "", item.PosName, item.Amount, serialNo, account.AccountToken, Constants.SystemShopName) { IsForce = true }) : new AccountServiceResponse(ResponseCode.NonFoundAccount);
                AccountServiceResponse rsp = item.DoPay();
                if (string.IsNullOrWhiteSpace(rsp.CodeText))
                    rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                if (rsp.Code == 0)
                    rsp.CodeText = "交易成功";
                return Json(rsp);
            }
            
            catch (Exception ex)
            {
                AccountServiceResponse rsp = new AccountServiceResponse(ResponseCode.SystemError);
                rsp.CodeText = ModelHelper.GetBoundText(rsp, x => x.Code);
                return Json(rsp);
            }
        }
        [HttpPost]
        [CheckPermission(Permissions.AccountPay)]
        [CheckUserType(typeof(ShopUser))]
        public ActionResult ShowPay(SystemPayAccount item)
        {
            //AdminUser currentUser = new AdminUser();
            //AccountServiceResponse rsp = item.DoPay(currentUser);
            var result = item.ShowPay();
            return Json(result);
        }

       
        //导出txt
        public ActionResult DeriveTXT(ListAccounts request)
        {
            var list = request.Querys();
            string str = "";
            foreach (var item in list)
            {
                str += item.Name +"="+ item.AccountToken + "\r\n";
            }
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=Accounts.txt");
            //指定返回的是一个不能被客户端读取的流，必须被下载     
            Response.ContentType = "text/plain";
            //把文件流发送到客户端     
            Response.Write(str.ToString());
            //停止页面的执行     
            Response.End();
            return List(request);
        }
    }
}
