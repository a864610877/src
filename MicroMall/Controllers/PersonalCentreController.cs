using Ecard;
using Ecard.Models;
using Ecard.Services;
using MicroMall.Models;
using MicroMall.Models.Parentings;
using MicroMall.Models.PersonalCentres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace MicroMall.Controllers
{
    public class PersonalCentreController : BaseController
    {
        //
        // GET: /PersonalCentre/

        private readonly IAccountService accountService;
        private readonly ITicketsService ticketsService;
        private readonly IAdmissionTicketService admissionTicketService;
        private readonly IUserCouponsService userCouponsService;
        private readonly ICouponsService couponsService;
        private readonly TransactionHelper transactionHelper;
        private readonly IOrdersService ordersService;
        private readonly IOrderDetialService orderDetialService;
        private readonly IUseCouponslogService useCouponslogService;
        private readonly IMembershipService membershipService;
        private readonly IAccountTypeService accountTypeService;
        private readonly ILog4netService log4NetService;
        private readonly DatabaseInstance _databaseInstance;

        public PersonalCentreController(IAccountService accountService, ITicketsService ticketsService
            , IAdmissionTicketService admissionTicketService, IUserCouponsService userCouponsService,
            ICouponsService couponsService, TransactionHelper transactionHelper, IOrdersService ordersService,
            IOrderDetialService orderDetialService, IUseCouponslogService useCouponslogService, IMembershipService membershipService
            , IAccountTypeService accountTypeService, ILog4netService log4NetService, DatabaseInstance _databaseInstance)
        {
            this.accountService = accountService;
            this.ticketsService = ticketsService;
            this.admissionTicketService = admissionTicketService;
            this.userCouponsService = userCouponsService;
            this.couponsService = couponsService;
            this.transactionHelper = transactionHelper;
            this.ordersService = ordersService;
            this.orderDetialService = orderDetialService;
            this.useCouponslogService = useCouponslogService;
            this.membershipService = membershipService;
            this.accountTypeService = accountTypeService;
            this.log4NetService = log4NetService;
            this._databaseInstance = _databaseInstance;
        }

        public ActionResult Index()
        {
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            List<CardModel> result = new List<CardModel>();
            var list = accountService.GetOwnerId(userId);
            if (list != null)
            {
                result = list.Select(x => new CardModel(x)).ToList();  
            }
            return View(result);
        }
        /// <summary>
        /// 购票
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyTicket()
        {
            TicketResult result = new TicketResult();
            result.ListTicket = admissionTicketService.GetNormalALL().Select(x => new ticketModel()
            {
                admissionTicketId=x.id,
                name = x.name,
                introduce = x.introduce,
                price = DateHelper.m_IsWorkingDay() == true ? x.amount : x.weekendAmount,
            }).ToList();
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            result.buyTickets = new BuyTickets();
            result.buyTickets.pageIndex = 1;
            result.buyTickets.pageSize = 15;
            var query = ticketsService.GetList(userId, result.buyTickets.pageIndex, result.buyTickets.pageSize);
            if (query != null)
            {
                result.buyTickets.ListTickets = query.ModelList;
            }
            result.ListCoupons = userCouponsService.GetUserId(userId).Select(x=>new UseCoupons(x)).ToList();
            return View(result);
        }
        [HttpPost]
        public ActionResult BuyTicketPage(int pageIndex)
        {
            int userId = 0;
            var buyTickets = new BuyTickets();
            buyTickets.pageIndex = pageIndex;
            buyTickets.pageSize = 15;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var query = ticketsService.GetList(userId, buyTickets.pageIndex, buyTickets.pageSize);
            if (query != null)
            {
                buyTickets.ListTickets = query.ModelList;
            }
            return Json(buyTickets);
        }
        /// <summary>
        /// 购票下单
        /// </summary>
        /// <param name="admissionTicketId"></param>
        /// <param name="num"></param>
        /// <param name="userCouponsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TicketPlaceOrder(int admissionTicketId,int num,int userCouponsId=0)
        {
            if(num<=0)
               return Json(new ResultMessage() { Code = -1, Msg = "购买数量必须大于0" });
            var admissionTicket = admissionTicketService.GetById(admissionTicketId);
            if (admissionTicket == null)
                return Json(new ResultMessage() { Code = -1, Msg = "门票不存在" });
            if(admissionTicket.state!= AdmissionTicketState.Normal)
                return Json(new ResultMessage() { Code = -1, Msg = "门票已下架，请选择购买其他门票" });
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var user = membershipService.GetUserById(userId) as AccountUser;
            if(user==null)
                return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
            decimal price = 0;//门票单价
            string orderNo= string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + userId;
            decimal deductible = 0;//优惠卷抵扣金额
            decimal discount = 0;//折扣
            string useScope = "";//指定门店
            int couponId = 0;//优惠卷id
            if (DateHelper.m_IsWorkingDay())
                price = admissionTicket.amount;
            else
                price = admissionTicket.weekendAmount;
            decimal amount = price * num;
            UserCoupons userCoupons = null;
            if (userCouponsId > 0)
            {
                userCoupons = userCouponsService.GetById(userCouponsId);
                if(userCoupons == null)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if(userCoupons.state!= UserCouponsState.NotUse)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已失效" });
                if (userCoupons.userId !=userId)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不可用" });
                var coupon = couponsService.GetById(userCoupons.couponsId);
                if(coupon==null&& coupon.state!=CouponsState.Normal)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if(coupon.validity.HasValue&&coupon.validity>DateTime.Now.Date)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已过期" });
                if (coupon.couponsType == CouponsType.DiscountedVolume)
                {
                    discount = coupon.discount;
                    deductible = amount-(discount * amount);
                }
                else if (coupon.couponsType == CouponsType.FullVolumeReduction)
                {
                    if (amount >= coupon.fullAmount)
                        deductible = coupon.reduceAmount;
                }
                else if (coupon.couponsType == CouponsType.OffsetRoll)
                {
                    deductible = coupon.deductibleAmount;
                }
                useScope = coupon.useScope;
                couponId = coupon.id;
            }
            var order = new Orders();
            order.amount = num * price;
            order.orderNo = orderNo;
            order.deductible = deductible;
            order.orderState = OrderStates.awaitPay;
            order.payAmount = order.amount-deductible;
            order.subTime = DateTime.Now;
            order.type = OrderTypes.ticket;
            order.userId = userId;
            order.useScope = useScope;

            var orderDetial = new OrderDetial();
            orderDetial.amount = price;
            orderDetial.cardNo = "";
            orderDetial.num = num;
            orderDetial.orderNo = orderNo;
            orderDetial.sourceId = admissionTicket.id;
            orderDetial.subTime = DateTime.Now;

            var tran=transactionHelper.BeginTransaction();
            try
            {
                ordersService.Create(order);
                orderDetialService.Create(orderDetial);
                if (couponId > 0)
                {
                    var useCouponslog = new UseCouponslog();
                    useCouponslog.amount = deductible;
                    useCouponslog.couponsId = couponId;
                    useCouponslog.discount = discount;
                    useCouponslog.orderNo = orderNo;
                    useCouponslog.userId = userId;
                    useCouponslog.useTime = DateTime.Now;
                    useCouponslogService.Create(useCouponslog);
                    if (userCoupons != null)
                    {
                        userCoupons.state = UserCouponsState.Used;
                        userCouponsService.Update(userCoupons);
                    }
                }

                JsApiPay jsApiPay = new JsApiPay();
                jsApiPay.openid = user.openId;
                jsApiPay.total_fee = (int)(order.payAmount * 100);
                WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order.orderNo);
                string wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                tran.Commit();
                return Json(new ResultMessage() { Code = 0, Msg = wxJsApiParam });
                //在页面上显示订单信息
                //Response.Write("<span style='color:#00CD00;font-size:20px'>订单详情：</span><br/>");
                //Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), ex.Message.ToString());
                return Json(new ResultMessage() { Code = -1, Msg = ex.Message.ToString() });
                //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
                //submit.Visible = false;
            }
            finally
            {
                tran.Dispose();
            }
        }

        /// <summary>
        /// 批量购票下单
        /// </summary>
        /// <param name="admissionTicketId"></param>
        /// <param name="num"></param>
        /// <param name="userCouponsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TicketPlaceOrders(string admissionTicketIds, string nums, int userCouponsId = 0)
        {
            var arrId = admissionTicketIds.Split(',');
            var arrNum = nums.Split(',');
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var user = membershipService.GetUserById(userId) as AccountUser;
            if (user == null)
                return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
            if (arrId.Length != arrNum.Length)
                return Json(new ResultMessage() { Code = -1, Msg = "购买异常，请刷出后购买" });
            List<OrderDetial> OrderDetials = new List<OrderDetial>();
            string orderNo = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + userId;
            decimal amount = 0; //总价格
            for (int i = 0; i < arrId.Length; i++)
            {
                int admissionTicketId = string.IsNullOrWhiteSpace(arrId[i]) ? 0 : Int32.Parse(arrId[i]);
                int num = string.IsNullOrWhiteSpace(arrNum[i]) ? 0 : Int32.Parse(arrNum[i]);
                if(admissionTicketId<=0&&num<=0)
                    return Json(new ResultMessage() { Code = -1, Msg = "购买异常，请刷出后购买" });
                var admissionTicket = admissionTicketService.GetById(admissionTicketId);
                if (admissionTicket == null)
                    return Json(new ResultMessage() { Code = -1, Msg = "门票不存在" });
                if (admissionTicket.state != AdmissionTicketState.Normal)
                    return Json(new ResultMessage() { Code = -1, Msg = ""+ admissionTicket .name+ "已下架，请选择购买其他门票" });
                decimal price = 0;//门票单价
                if (DateHelper.m_IsWorkingDay())
                    price = admissionTicket.amount;
                else
                    price = admissionTicket.weekendAmount;
                var orderDetial = new OrderDetial();
                orderDetial.amount = price;
                orderDetial.cardNo = "";
                orderDetial.num = num;
                orderDetial.orderNo = orderNo;
                orderDetial.sourceId = admissionTicket.id;
                orderDetial.subTime = DateTime.Now;
                OrderDetials.Add(orderDetial);
                amount += (price * num);
            }
            decimal deductible = 0;//优惠卷抵扣金额
            decimal discount = 0;//折扣
            string useScope = "";//指定门店
            int couponId = 0;//优惠卷id
            UserCoupons userCoupons = null;
            if (userCouponsId > 0)
            {
                userCoupons = userCouponsService.GetById(userCouponsId);
                if (userCoupons == null)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if (userCoupons.state != UserCouponsState.NotUse)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已失效" });
                if (userCoupons.userId != userId)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不可用" });
                var coupon = couponsService.GetById(userCoupons.couponsId);
                if (coupon == null && coupon.state != CouponsState.Normal)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if (coupon.validity.HasValue && coupon.validity > DateTime.Now.Date)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已过期" });
                if (coupon.couponsType == CouponsType.DiscountedVolume)
                {
                    discount = coupon.discount;
                    deductible = amount-( discount * amount);
                }
                else if (coupon.couponsType == CouponsType.FullVolumeReduction)
                {
                    if (amount >= coupon.fullAmount)
                        deductible = coupon.reduceAmount;
                }
                else if (coupon.couponsType == CouponsType.OffsetRoll)
                {
                    deductible = coupon.deductibleAmount;
                }
                useScope = coupon.useScope;
                couponId = coupon.id;
            }
            var order = new Orders();
            order.amount = amount;
            order.orderNo = orderNo;
            order.deductible = deductible;
            order.orderState = OrderStates.awaitPay;
            order.payAmount = amount - deductible;
            order.subTime = DateTime.Now;
            order.type = OrderTypes.ticket;
            order.userId = userId;
            order.useScope = useScope;

            

            var tran = transactionHelper.BeginTransaction();
            try
            {
                ordersService.Create(order);
                foreach (var orderDetial in OrderDetials)
                {
                    orderDetialService.Create(orderDetial);
                }
                if (couponId > 0)
                {
                    var useCouponslog = new UseCouponslog();
                    useCouponslog.amount = deductible;
                    useCouponslog.couponsId = couponId;
                    useCouponslog.discount = discount;
                    useCouponslog.orderNo = orderNo;
                    useCouponslog.userId = userId;
                    useCouponslog.useTime = DateTime.Now;
                    useCouponslogService.Create(useCouponslog);
                    if (userCoupons != null)
                    {
                        userCoupons.state = UserCouponsState.Used;
                        userCouponsService.Update(userCoupons);
                    }
                }

                //JsApiPay jsApiPay = new JsApiPay();
                //jsApiPay.openid = user.openId;
                //jsApiPay.total_fee = (int)(order.payAmount * 100);
                //WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order.orderNo);
                //string wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                //WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                tran.Commit();
                hd(orderNo);
                return Json(new ResultMessage() { Code = 0, Msg = "" });
                //在页面上显示订单信息
                //Response.Write("<span style='color:#00CD00;font-size:20px'>订单详情：</span><br/>");
                //Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), ex.Message.ToString());
                return Json(new ResultMessage() { Code = -1, Msg = ex.Message.ToString() });
                //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
                //submit.Visible = false;
            }
            finally
            {
                tran.Dispose();
            }
        }
        /// <summary>
        /// 购买卡
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyCard()
        {
            var result = new BuyCardResult();
            result.accountTypes = accountTypeService.Query(new AccountTypeRequest() { State = AccountTypeStates.Normal }).ToList();
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            result.ListCoupons = userCouponsService.GetUserId(userId).Select(x => new UseCoupons(x)).ToList();
            return View(result);
        }

        /// <summary>
        /// 购卡下单
        /// </summary>
        /// <param name="admissionTicketId"></param>
        /// <param name="num"></param>
        /// <param name="userCouponsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CardPlaceOrder(int accountTypeId, int userCouponsId = 0)
        {
            var accountType = accountTypeService.GetById(accountTypeId);
            if (accountType == null)
                return Json(new ResultMessage() { Code = -1, Msg = "你要购买得卡不存在" });
            if (accountType.State != AccountTypeStates.Normal)
                return Json(new ResultMessage() { Code = -1, Msg = "此卡已下架，请选择购买其他卡" });
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var user = membershipService.GetUserById(userId) as AccountUser;
            if (user == null)
                return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
            decimal price = accountType.Amount;//卡单价
            string orderNo = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + userId;
            decimal deductible = 0;//优惠卷抵扣金额
            decimal discount = 0;//折扣
            string useScope = "";//指定门店
            int couponId = 0;//优惠卷id
            decimal amount = price;
            UserCoupons userCoupons = null;
            if (userCouponsId > 0)
            {
                userCoupons = userCouponsService.GetById(userCouponsId);
                if (userCoupons == null)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if (userCoupons.state != UserCouponsState.NotUse)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已失效" });
                if (userCoupons.userId != userId)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不可用" });
                var coupon = couponsService.GetById(userCoupons.couponsId);
                if (coupon == null && coupon.state != CouponsState.Normal)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷不存在" });
                if (coupon.validity.HasValue && coupon.validity > DateTime.Now.Date)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已过期" });
                if (coupon.couponsType == CouponsType.DiscountedVolume)
                {
                    discount = coupon.discount;
                    deductible = amount-(discount * amount);
                }
                else if (coupon.couponsType == CouponsType.FullVolumeReduction)
                {
                    if (amount >= coupon.fullAmount)
                        deductible = coupon.reduceAmount;
                }
                else if (coupon.couponsType == CouponsType.OffsetRoll)
                {
                    deductible =coupon.deductibleAmount;
                }
                useScope = coupon.useScope;
                couponId = coupon.id;
            }
            var order = new Orders();
            order.amount = price;
            order.orderNo = orderNo;
            order.deductible = deductible;
            order.orderState = OrderStates.awaitPay;
            order.payAmount = order.amount - deductible;
            order.subTime = DateTime.Now;
            order.type = OrderTypes.card;
            order.userId = userId;
            order.useScope = useScope;

            var orderDetial = new OrderDetial();
            orderDetial.amount = price;
            orderDetial.cardNo = "";
            orderDetial.num = 1;
            orderDetial.orderNo = orderNo;
            orderDetial.sourceId = accountType.AccountTypeId;
            orderDetial.subTime = DateTime.Now;

            var tran = transactionHelper.BeginTransaction();
            try
            {
                ordersService.Create(order);
                orderDetialService.Create(orderDetial);
                if (couponId > 0)
                {
                    var useCouponslog = new UseCouponslog();
                    useCouponslog.amount = deductible;
                    useCouponslog.couponsId = couponId;
                    useCouponslog.discount = discount;
                    useCouponslog.orderNo = orderNo;
                    useCouponslog.userId = userId;
                    useCouponslog.useTime = DateTime.Now;
                    useCouponslogService.Create(useCouponslog);
                    if (userCoupons != null)
                    {
                        userCoupons.state = UserCouponsState.Used;
                        userCouponsService.Update(userCoupons);
                    }
                }
                //微信支付
                //JsApiPay jsApiPay = new JsApiPay();
                //jsApiPay.openid = user.openId;
                //jsApiPay.total_fee = (int)(order.payAmount * 100);
                //WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(order.orderNo);
                //string wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                //WxPayAPI.Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                tran.Commit();

                hd(orderNo);



                //return Json(new ResultMessage() { Code = 0, Msg = wxJsApiParam });
                return Json(new ResultMessage() { Code = 0, Msg = "" });
                

            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Error(this.GetType().ToString(), ex.Message.ToString());
                return Json(new ResultMessage() { Code = -1, Msg = ex.Message.ToString() });
                //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
                //submit.Visible = false;
            }
            finally
            {
                tran.Dispose();
            }
        }

        public void hd(string orderNo)
        {
            _databaseInstance.BeginTransaction();
            var sql = "select * from fz_Orders where orderNo=@orderNo";
            var order = new QueryObject<Orders>(_databaseInstance, sql, new { orderNo = orderNo }).FirstOrDefault();
            if (order != null && order.orderState == OrderStates.awaitPay)
            {
                order.orderState = OrderStates.paid;
                order.payTime = DateTime.Now;
                _databaseInstance.Update(order, "fz_Orders");
                var list = orderDetialService.GetOrderNo(orderNo);
                if (list != null)
                {
                    if (order.type == OrderTypes.ticket)
                    {
                        foreach (var item in list)
                        {
                            var admissionTicket = admissionTicketService.GetById(item.sourceId);
                            for (var i = 0; i < item.num; i++)
                            {
                                var ticket = new Tickets();
                                ticket.AdmissionTicketId = admissionTicket.id;
                                ticket.orderNo = orderNo;
                                ticket.Price = item.amount;
                                ticket.State = TicketsState.NotUse;
                                ticket.UserId = order.userId;
                                ticket.useScope = order.useScope;
                                ticket.adultNum = admissionTicket.adultNum;
                                ticket.BuyTime = DateTime.Now;
                                ticket.childNum = admissionTicket.childNum;
                                ticket.Code = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + i.ToString() + order.userId.ToString();
                                ticket.ExpiredDate = DateTime.Now.Date;
                                _databaseInstance.Insert(ticket, "Tickets");
                            }
                        }
                    }
                    else if (order.type == OrderTypes.card)
                    {
                        string sqlSite = "select * from Sites";
                        var site = new QueryObject<Site>(_databaseInstance, sqlSite, null).FirstOrDefault();
                        string cardNo = "";
                        int minxCode = 1;
                        Int32.TryParse(site.MixCode, out minxCode);
                        int i = 1;
                        while (true)
                        {
                            minxCode = minxCode + i;
                            cardNo = string.Format("60000000{0}", minxCode.ToString().PadLeft(8, '0'));
                            string sqlCard = "select * from Accounts where Name=@Name";
                            var card = new QueryObject<Account>(_databaseInstance, sqlCard, new { Name = cardNo }).FirstOrDefault();
                            if (card == null)
                                break;
                            i++;
                        }
                        int shopId = 0;
                        if (!string.IsNullOrWhiteSpace(order.useScope))
                        {
                            string sqlShop = "select * from shops where Name=@Name";
                            var shop = new QueryObject<Account>(_databaseInstance, sqlShop, new { Name = order.useScope }).FirstOrDefault();
                            if (shop != null)
                                shopId = shop.ShopId;
                        }
                        var item = list.FirstOrDefault();
                        var accountType = _databaseInstance.GetById<AccountType>("AccountTypes", item.sourceId);
                        var account = new Account();
                        account.AccountLevel = 0;
                        account.AccountToken = "11111111";
                        account.AccountTypeId = accountType.AccountTypeId;
                        account.Amount = 0;
                        account.ExpiredDate = DateTime.Now.AddMonths(accountType.ExpiredMonths);
                        account.Frequency = accountType.Frequency;
                        account.FrequencyUsed = 0;
                        account.LastDealTime = DateTime.Now;
                        account.Name = cardNo;
                        account.OpenTime = DateTime.Now;
                        account.OwnerId = order.userId;
                        account.ShopId = shopId;
                        account.useScope = order.useScope;
                        account.State = AccountStates.Normal;
                        _databaseInstance.Insert(account, "Accounts");
                        site.MixCode = minxCode.ToString();
                        _databaseInstance.Update(site, "Sites");
                    }
                }
            }
            _databaseInstance.Commit();
        }

        public ActionResult Coupons()
        {
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            //var user = membershipService.GetUserById(userId) as AccountUser;
            //if (user == null)
            //    return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
            var conupons = couponsService.GetByUserCoupon(userId);//可领
            var result = new List<CouponsModel>();
            result.AddRange(conupons.Select(x => new CouponsModel(x)));
            var con = userCouponsService.GetUserId(userId);
            result.AddRange(con.Select(x => new CouponsModel(x)));
            return View(result);
        }
        [HttpPost]
        public ActionResult ReceiveCoupons(int id)
        {
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var user = membershipService.GetUserById(userId) as AccountUser;
            if (user == null)
                return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
           var tran= transactionHelper.BeginTransaction();
            try
            {
                var conupon = couponsService.GetById(id);
                if (conupon == null || conupon.state != CouponsState.Normal)
                    return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已失效" });
                if (conupon.validity.HasValue)
                {
                    if (conupon.validity > DateTime.Now.Date)
                        return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已失效" });
                    if (conupon.quantity - conupon.leadersOfNum <= 0)
                        return Json(new ResultMessage() { Code = -1, Msg = "优惠卷已领完" });
                }
                var userCoupons = new UserCoupons();
                userCoupons.couponsId = conupon.id;
                userCoupons.receiveTime = DateTime.Now;
                userCoupons.state = UserCouponsState.NotUse;
                userCoupons.userId = userId;
                userCouponsService.Create(userCoupons);
                conupon.leadersOfNum += 1;
                couponsService.Update(conupon);
                tran.Commit();
                return Json(new ResultMessage() { Code = 0, Msg = "" });
            }
            catch (Exception ex)
            {
                tran.Rollback();
                log4NetService.Insert(ex);
                return Json(new ResultMessage() { Code = -1, Msg = "领取异常，请联系管理员" });
            }
            finally
            {
                tran.Dispose();
            }
            
            
        }
        [HttpPost]
        public ActionResult DiscountAmount(decimal amount, int userCouponsId)
        {
            decimal discountAmount = 0;
            var userCoupon = userCouponsService.GetById(userCouponsId);
            if (userCoupon == null)
                return Json(new ResultMessage() { Code = 0, Msg = "0" });
            if(userCoupon.state!= UserCouponsState.NotUse)
                return Json(new ResultMessage() { Code = 0, Msg = "0" });
            var coupon = couponsService.GetById(userCoupon.couponsId);
            if(coupon==null)
                return Json(new ResultMessage() { Code = 0, Msg = "0" });
            if(coupon.state!= CouponsState.Normal)
                return Json(new ResultMessage() { Code = 0, Msg = "0" });
            if (coupon.couponsType == CouponsType.DiscountedVolume)
            {
                discountAmount =amount-(amount * coupon.discount);
            }
            else if (coupon.couponsType == CouponsType.FullVolumeReduction)
            {
                if (amount >= coupon.fullAmount)
                    discountAmount = coupon.reduceAmount;
            }
            else if (coupon.couponsType == CouponsType.OffsetRoll)
            {
                discountAmount = coupon.deductibleAmount;
            }
            return Json(new ResultMessage() { Code = 0, Msg = discountAmount.ToString() });
        }

        public ActionResult ConsumptionLog()
        {
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var user = membershipService.GetUserById(userId) as AccountUser;
            if (user == null)
                return Json(new ResultMessage() { Code = -1, Msg = "用户不存在" });
            ConsumptionLogResult result = new ConsumptionLogResult();
            var request = new OrdersRequest();
            request.userId = userId;
            request.pageIndex = 1;
            request.pageSize = 15;
            request.orderState = OrderStates.paid;
            var datas = ordersService.Query(request);
            if (datas != null)
            {
                result.pageSize = request.pageSize;
                result.pageIndex = request.pageIndex;
                result.ListConsumptionLog = datas.ModelList.Select(x => new ConsumptionLogModel(x)).ToList();
            }
            return View(result);
        }

        [HttpPost]
        public ActionResult ConsumptionLogPage(int pageIndex)
        {
            int userId = 0;
            var result = new ConsumptionLogResult();
            result.pageIndex = pageIndex;
            result.pageSize = 15;
            var cookieId = Request.Cookies[SessionKeys.USERID].Value.ToString();
            int.TryParse(cookieId, out userId);
            var request = new OrdersRequest();
            request.userId = userId;
            request.pageIndex = result.pageIndex;
            request.pageSize = result.pageSize;
            request.orderState = OrderStates.paid;
            var query = ordersService.Query(request);
            if (query != null)
            {
                result.ListConsumptionLog = query.ModelList.Select(x=>new ConsumptionLogModel(x)).ToList();
            }
            return Json(result);
        }
    }
}
