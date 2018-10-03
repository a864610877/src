using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.GoodandOrder;
using Microsoft.Practices.Unity;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Ecard.Models;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IUnityContainer _unityContainer;
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IOrderService OrderService { get; set; }
        [Dependency]
        public IMembershipService MenbershipService { get; set; }
        public OrderController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
        [CheckPermission(Permissions.OrderCreate)]
        public ActionResult Create()
        {
            var createOrder = _unityContainer.Resolve<CreateOrder>();
            var model = new EcardModelItem<CreateOrder>(createOrder);
            createOrder.Read();
            createOrder.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderCreate)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateOrder model)
        {
            #region 检验输入的会员是否正确
            if (!string.IsNullOrWhiteSpace(Request.Form["txtSearch"]))
            {
                string searchtext=Request.Form["txtSearch"];

                if (searchtext.IndexOf("--＞") > 0)
                {
                    var text = Request.Form["txtSearch"].Split("--＞".ToArray());
                    var account = AccountService.GetByName(text[text.Length-1]);
                    var user = MenbershipService.QueryUsers<AccountUser>(new UserRequest() { DisplayNameWith = text[0] });
                    if (account == null && user.Count() < 1)
                    {
                        model.AddMsg("系统没有找到会员信息，请重新输入！");
                        model.Read();
                        model.Ready();
                        return View(new EcardModelItem<CreateOrder>(model, model));
                    }
                    else
                    {
                        if (account == null)
                        {
                            if (user.Count() > 1)
                            {
                                model.AddMsg("根据" + text[0] + "查到不止一个会员,系统无法确定订单所属会员。");
                                model.Read();
                                model.Ready();
                                return View(new EcardModelItem<CreateOrder>(model, model));
                            }
                            else
                                model.AccountId = AccountService.QueryByOwnerId(user.First()).First().AccountId;
                        }
                        else
                            model.AccountId = account.AccountId;
                    }
                }
                else
                {
                    var account = AccountService.GetByName(searchtext);
                    var user = MenbershipService.QueryUsers<AccountUser>(new UserRequest() { DisplayNameWith = searchtext });
                    if (account == null && user.Count() < 1)
                    {
                        model.AddMsg("系统没有找到会员信息，请重新输入！");
                        model.Read();
                        model.Ready();
                        return View(new EcardModelItem<CreateOrder>(model, model));
                    }
                    else
                    {
                        if (account == null)
                        {
                            if (user.Count() > 1)
                            {
                                model.AddMsg("根据" + searchtext + "查到不止一个会员,系统无法确定订单所属会员。");
                                model.Read();
                                model.Ready();
                                return View(new EcardModelItem<CreateOrder>(model, model));
                            }
                            else
                                model.AccountId = AccountService.QueryByOwnerId(user.First()).First().AccountId;
                        }
                        else
                            model.AccountId = account.AccountId;
                    }
                }
            }
            else
            {
                model.AddMsg("系统没有找到会员信息，请重新输入！");
                model.Read();
                model.Ready();
                return View(new EcardModelItem<CreateOrder>(model, model));
            }
            #endregion

            var ss = model.Detial.Count;
            int num = 0;
            List<OrderDetial> detials = new List<OrderDetial>();
            for (int i = 0; i < ss; i++)
            {
                OrderDetial detial = new OrderDetial();
                if (Request.Form["Item.Detial[" + i.ToString() + "].Checked"].ToLower().IndexOf("true") > -1)
                {
                    num++;
                    detial.GoodId = int.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Id"]);
                    detial.Amount = int.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Amount"]);
                    detial.price = decimal.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Price"]);
                    if ((detial.price * detial.Amount) <= 0)
                    {
                        model.AddMsg("没有正确的输入数量或单价");
                        model.Read();
                        model.Ready();
                        return View(new EcardModelItem<CreateOrder>(model, model));
                    }
                    detials.Add(detial);
                }
            }
            if (num > 0)
            {
                model.Detials = detials;
                IMessageProvider msg = null;
                if (ModelState.IsValid)
                {
                    this.ModelState.Clear();

                    msg = model.Create();
                    model = _unityContainer.Resolve<CreateOrder>();
                }
                model.Read();
                model.Ready();
                return View(new EcardModelItem<CreateOrder>(model, msg));
            }
            else
            {
               // new Exception("没有选择商品");
                model.AddMsg("没有选择商品");
                model.Read();
                model.Ready(); return View(new EcardModelItem<CreateOrder>(model,model));
            }
        }

        [HttpGet]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public JsonResult CheckAccountName()
        {
            //txtSearch
            bool result = false;
            string text = Request.Form["txtSearch"].Split("--＞".ToArray())[1];
            var account = AccountService.GetByName(text);
            if (account != null)
                result= true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(Permissions.OrderEdit)]
        public ActionResult Edit(int id)
        {
            var model = _unityContainer.Resolve<EditOrder>();
            var m = new EcardModelItem<EditOrder>(model);
            model.LoadInnerObject(id);
            model.Read();
            model.Ready();
            return View(m);
        }

        [CheckPermission(Permissions.Order)]
        public ActionResult Show(int id)
        {
            var order = OrderService.QueryOrder(new OrderRequest() { Serialnumber = id }).FirstOrDefault();
            if (order != null)
            {
                var model = _unityContainer.Resolve<ViewOrder>();
                model.SetInnerObject(order);
                model.Read();
                return View(new EcardModelItem<ViewOrder>(model));
            }

            else
            {
                return View();
            }
        }
        public JsonResult SearchAccount(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.IndexOf("--＞") > 0)
                {
                    text = text.Split("--＞".ToArray())[0];
                }

                //1、查询Accounts表Name字段中包含text的。
                //2、查询Users表DisplayName字段包含text
                var accounts = AccountService.Query(new AccountRequest());// { NameWith = text }).Select(x=> x.Name);
                var users = MenbershipService.QueryUsers<AccountUser>(new UserRequest() { DisplayNameWith = text }).Select(x => new { userId = x.UserId, accountName = x.DisplayName });//AccountService.QueryAccountWithOwner(new AccountRequest(){ NameWith=text}); 
                var userIds=users.Select(x=>x.userId).ToList();
                var accounts1 = accounts.Where(x => x.Name.IndexOf(text) > 0).Take(15).Select(y => y.Name);
                var temp1 = accounts.Where(z=>z.OwnerId.HasValue).Where(x => userIds.Contains(x.OwnerId.Value)).Take(15).Select(c => new { accountName = c.Name, ownerId = c.OwnerId }).ToList();
                var accounts2 = users.Join(temp1, u => u.userId, t => t.ownerId, (u, t) => new { accountName = t.accountName, DisplayName = u.accountName });

                StringBuilder result = new StringBuilder();
                foreach (var item in accounts1)
                {
                    result.Append(string.Format("无会员信息--＞{0}!~!", item));
                }
                foreach (var item in accounts2)
                {
                    result.Append(string.Format("{0}--＞{1}!~!", item.DisplayName, item.accountName));
                }
                return Json(result.ToString());
            }
            return new JsonResult();
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderEdit)]
        public ActionResult Edit([Bind(Prefix = "Item")] EditOrder model)
        {
            var ss = model.AllCommodity.Count;
            int num = 0;
            int serialnumber = model.Serialnumber;
            model.Detials = new List<OrderDetial>();
            List<OrderDetial> detials = new List<OrderDetial>();
            for (int i = 0; i < ss; i++)
            {
                OrderDetial detial = new OrderDetial();
                if (Request.Form["Item.AllCommodity[" + i.ToString() + "].Checked"].ToLower().IndexOf("true") > -1)
                {
                    num++;
                    detial.GoodId = int.Parse(Request.Form["Item.AllCommodity[" + i.ToString() + "].Id"]);
                    detial.Amount = int.Parse(Request.Form["Item.AllCommodity[" + i.ToString() + "].Amount"]);
                    detial.price = decimal.Parse(Request.Form["Item.AllCommodity[" + i.ToString() + "].Price"]);
                    if ((detial.price * detial.Amount) <= 0)
                    {
                        model.AddMsg("没有正确的输入数量或单价");
                        model.Read();
                        model.Ready();
                        return View(new EcardModelItem<EditOrder>(model, model));
                    }
                    detials.Add(detial);
                }
            }
            if (num > 0)
            {
                model.Detials = detials;
                IMessageProvider msg = null;
                if (ModelState.IsValid)
                {
                    this.ModelState.Clear();

                    msg = model.Edit();
                    model = _unityContainer.Resolve<EditOrder>();
                }
                model.LoadInnerObject(serialnumber);
                model.Read();
                model.Ready();
                return View(new EcardModelItem<EditOrder>(model, msg));
            }
            else
            {
                // new Exception("没有选择商品");
                model.AddMsg("没有选择商品");
                model.LoadInnerObject(serialnumber);
                model.Read();
                model.Ready(); return View(new EcardModelItem<EditOrder>(model, model));
            }
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderDelete)]
        public ActionResult Deletes(listOrders request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Delete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderDelete)]
        public ActionResult Delete(int id, listOrders request)
        {
            request.Delete(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderCarry)]
        public JsonResult Carries(int senderId,listOrders request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            StringBuilder sb = new StringBuilder();
            foreach (var id in ids)
            {
                request.Carry(id, senderId);
                sb.Append(request.GetMessages().FirstOrDefault().Messages);
            }

            return Json(sb.ToString());
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderCarry)]
        public JsonResult Carry(int id,int senderId, listOrders request)
        {
            
            request.Carry(id,senderId);
            //var msg = request.GetMessages(MessageType.Error);
            return Json(request.GetMessages().FirstOrDefault().Messages);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderComplete)]
        public ActionResult Completes(listOrders request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Complete(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderComplete)]
        public ActionResult Complete(int id, listOrders request)
        {
            request.Complete(id);
            return List(request);
        }

        [CheckPermission(Permissions.Order)]
        public virtual ActionResult List(listOrders request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Ready();
                request.Query();
            }
            return View("List", request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderSuspend)]
        public ActionResult Suspend(int id, listOrders request)
        {
            request.Suspend(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderSuspend)]
        public ActionResult Suspends(listOrders request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            foreach (var id in ids)
            {
                request.Suspend(id);
            }

            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderResume)]
        public ActionResult Resume(int id, listOrders request)
        {
            request.Resume(id);
            return List(request);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderResume)]
        public ActionResult Resumes(listOrders request)
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
