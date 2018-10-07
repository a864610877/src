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

        public PersonalCentreController(IAccountService accountService, ITicketsService ticketsService
            , IAdmissionTicketService admissionTicketService, IUserCouponsService userCouponsService,
            ICouponsService couponsService, TransactionHelper transactionHelper, IOrdersService ordersService,
            IOrderDetialService orderDetialService, IUseCouponslogService useCouponslogService, IMembershipService membershipService)
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
        }

        public ActionResult Index()
        {
            int userId = 0;
            var cookieId = Request.Cookies[SessionKeys.USERID].ToString();
            int.TryParse(cookieId, out userId);
            List<CardModel> result = new List<CardModel>();
            var list = accountService.GetOwnerId(userId);
            if (list != null)
            {
                result = list.Select(x => new CardModel(x)).ToList();  
            }
            return View(result);
        }

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
            var cookieId = Request.Cookies[SessionKeys.USERID].ToString();
            int.TryParse(cookieId, out userId);
            var query = ticketsService.GetList(userId);
            if (query != null)
            {
                result.buyTickets = new BuyTickets();
                result.buyTickets.pageIndex = 1;
                result.buyTickets.pageSize = 15;
                result.buyTickets.ListTickets = query.ModelList;
            }
            result.ListCoupons = userCouponsService.GetUserId(userId).Select(x=>new UseCoupons(x)).ToList();
            return View(result);
        }
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
            var cookieId = Request.Cookies[SessionKeys.USERID].ToString();
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
                    deductible = discount * amount;
                }
                else if (coupon.couponsType == CouponsType.FullVolumeReduction)
                {
                    if (amount >= coupon.fullAmount)
                        deductible = amount - coupon.reduceAmount;
                }
                else if (coupon.couponsType == CouponsType.OffsetRoll)
                {
                    deductible = amount - coupon.deductibleAmount;
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

    }
}
