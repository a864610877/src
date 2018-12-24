using Ecard.Models;
using Ecard.Mvc.Models.PosApi;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WxPayAPI;

namespace Ecard.Mvc.Controllers
{
    public class PosApiController: Controller
    {
        private readonly IPosEndPointService posEndPointService;
        private readonly IPostTokenService postTokenService;
        private readonly ITicketsService ticketsService;
        private readonly ITicketOffService ticketOffService;
        private readonly IAccountService accountService;
        private readonly IMembershipService membershipService;
        private readonly IAccountTypeService accountTypeService;
        private readonly IShopService shopService;
        private readonly TransactionHelper transactionHelper;
        private readonly IAdmissionTicketService admissionTicketService;
        private readonly IHandRingPrintService handRingPrintService;
        private readonly IUnityContainer _unityContainer;

        public PosApiController(IUnityContainer unityContaine, IPosEndPointService posEndPointService, IPostTokenService postTokenService, IAccountTypeService accountTypeService,
            ITicketsService ticketsService, ITicketOffService ticketOffService, IAccountService accountService, IMembershipService membershipService, IShopService shopService,
            TransactionHelper transactionHelper, IAdmissionTicketService admissionTicketService, IHandRingPrintService handRingPrintService)
        {
            this._unityContainer = unityContaine;
            this.posEndPointService = posEndPointService;
            this.postTokenService = postTokenService;
            this.ticketsService = ticketsService;
            this.ticketOffService = ticketOffService;
            this.accountService = accountService;
            this.membershipService = membershipService;
            this.accountTypeService = accountTypeService;
            this.shopService = shopService;
            this.transactionHelper = transactionHelper;
            this.admissionTicketService = admissionTicketService;
            this.handRingPrintService = handRingPrintService;
        }
        [HttpPost]
        public ActionResult Login(LoginRequest request)
        {
            try
            {
                WxPayAPI.Log.Info("PosApiController", string.Format("请求登录：账号{0},密码:{1}", request.username, request.password));
                LoginRespone result = new LoginRespone();
                
                if (string.IsNullOrEmpty(request.username))
                {
                    return Json(new ApiResponse() { Code = "-1", Msg = "请输入账号" });
                }
                var model = posEndPointService.GetByName(request.username);
                if (model == null)
                    return Json(new ApiResponse() { Code = "-1", Msg = "登录账号不存在" });
                if (model.DataKey != request.password)
                    return Json(new ApiResponse() { Code = "-1", Msg = "密码错误" });
                if (model.State!= States.Normal)
                    return Json(new ApiResponse() { Code = "-1", Msg = "账号已被停用" });
                string token = request.username + "_" + DateTime.Now.ToShortTimeString();
                token = SaltAndHash(token, Guid.NewGuid().ToString("N").Substring(0, 8));
                var posToken = postTokenService.GetByPosName(request.username);
                if (posToken == null)
                {
                    posToken = new PostToken();
                    posToken.createTime = DateTime.Now;
                    posToken.posName = request.username;
                    posToken.token = token;
                    postTokenService.Insert(posToken);

                }
                else
                {
                    posToken.token = token;
                    posToken.createTime = DateTime.Now;
                    postTokenService.Update(posToken);
                }
                result.Code = "1";
                result.Msg = "SUCCESS";
                result.token = token;
                return Json(result);
            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Info("PosApiController", string.Format("请求登录：账号{0},密码:{1},异常:{2}", request.username, request.password, ex.Message));
                return Json(new ApiResponse() { Code = "-1", Msg = "系统异常，请联系管理员" });
            }

        }
        [HttpPost]
        public ActionResult GetTicketInfo(TicketWriteOffRequest request)
        {
           
            try
            {
                WxPayAPI.Log.Info("PosApiController", string.Format("请求获取门票：code{0},token:{1}", request.code, request.token));
                TicketWriteOffResponse result = new TicketWriteOffResponse();
                if (string.IsNullOrEmpty(request.code))
                {
                    return Json(new ApiResponse() { Code = "-1", Msg = "门票代码为空" });
                }
                if (string.IsNullOrEmpty(request.token))
                {
                    return Json(new ApiResponse() { Code = "-1", Msg = "token为空" });
                }
                var postToken = postTokenService.GetByToken(request.token);
                if (postToken == null)
                    return Json(new ApiResponse() { Code = "-1", Msg = "token失效请重新登录" });
                var pos = posEndPointService.GetByName(postToken.posName);
                if (pos == null && pos.State != States.Normal)
                    return Json(new ApiResponse() { Code = "-1", Msg = "pos账号不存在或已停用" });
                var ticket = ticketsService.GetByCode(request.code);
                if (ticket != null)
                {
                    result.codeNo = ticket.Code;
                    result.buyTime = ticket.BuyTime.ToString("yyyy-MM-dd hh:mm:ss");
                    result.ExpiredDate = ticket.ExpiredDate.ToString("yyyy-MM-dd hh:mm:ss");
                    result.name = ticket.TicketName;
                    if (ticket.State == TicketsState.Used)
                        result.State = "已使用";
                    else if (ticket.State == TicketsState.NotUse)
                    {
                        if (DateTime.Now.Date > ticket.ExpiredDate)
                            result.State = "已过期";
                        else
                            result.State = "未使用";
                    }
                    result.userTime = ticket.userTime.HasValue ? ticket.userTime.Value.ToString("yyyy-MM-dd hh:mm:ss") : "";
                    result.useScope = ticket.ShopName;
                    result.babyName = ticket.babyName;
                    result.babySex = ticket.babySex == 1 ? "男" : ticket.babySex == 2 ? "女" : "";
                    result.userName = ticket.UserDisplayName;
                    result.mobile = ticket.Mobile;
                    return Json(result);
                }

                var card = accountService.GetByName(request.code);
                if (card == null)
                    return Json(new ApiResponse() { Code = "-1", Msg = "门票不存在" });
                var cardType = accountTypeService.GetById(card.AccountTypeId);
                var user = membershipService.GetUserById(card.OwnerId.HasValue ? card.OwnerId.Value : 0) as AccountUser;
                if (user != null)
                {

                    result.babyName = user.babyName;
                    result.babySex = user.babySex == 1 ? "男" : user.babySex == 2 ? "女" : "";
                    result.mobile = user.Mobile;
                    result.mobile = user.Mobile;
                    result.userName = user.DisplayName;
                }
                result.name = cardType != null ? cardType.DisplayName : "";
                result.buyTime = card.OpenTime.HasValue ? card.OpenTime.Value.ToString("yyyy-MM-dd hh:mm:ss") : "";
                result.codeNo = card.Name;
                result.ExpiredDate = card.ExpiredDate.ToString("yyyy-MM-dd hh:mm:ss");
                result.Frequency = card.Frequency;
                result.FrequencyUsed = card.FrequencyUsed;
                result.State = ModelHelper.GetBoundText(card, x => x.State);
                var shop = shopService.GetById(card.ShopId);
                if (shop != null)
                    result.useScope = shop.DisplayName;
                return Json(result);
            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Info("WindowTicketOffController", string.Format("请求获取门票异常：{0}", ex.Message));
                return Json(new ApiResponse() { Code = "-1", Msg = "异常，请联系管理员" });
            }
            
        }
        [HttpPost]
        public ActionResult TicketWriteOff(TicketWriteOffRequest request)
        {
            WxPayAPI.Log.Info("PosApiController", string.Format("核销门票：code{0},token:{1}", request.code, request.token));
            //TicketWriteOffResponse result = new TicketWriteOffResponse();
            if (string.IsNullOrEmpty(request.code))
            {
                return Json(new ApiResponse() { Code = "-1", Msg = "门票代码为空" });
            }
            if (string.IsNullOrEmpty(request.token))
            {
                return Json(new ApiResponse() { Code = "-1", Msg = "token为空" });
            }
            var postToken = postTokenService.GetByToken(request.token);
            if (postToken == null)
                return Json(new ApiResponse() { Code = "-1", Msg = "token失效请重新登录" });
            var pos = posEndPointService.GetByName(postToken.posName);
            if (pos == null && pos.State != States.Normal)
                return Json(new ApiResponse() { Code = "-1", Msg = "pos账号不存在或已停用" });
            var transaction = transactionHelper.BeginTransaction();
            try
            {
                var ticket = ticketsService.GetByCodeModel(request.code);
                if (ticket != null)
                {
                    var admissionTicket = admissionTicketService.GetById(ticket.AdmissionTicketId);
                    if (ticket.State == TicketsState.Used)
                    {
                        transaction.Dispose();
                        return Json(new ApiResponse() { Code = "-1", Msg = "已使用" });
                    }
                    if (ticket.State == TicketsState.NotUse)
                    {
                        if (DateTime.Now.Date > ticket.ExpiredDate)
                        {
                            ticket.State = TicketsState.BeOverdue;
                            ticketsService.Update(ticket);
                            transactionHelper.Commit();
                            return Json(new ApiResponse() { Code = "-1", Msg = "已过期" });
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(ticket.useScope))
                    {
                        var shop = shopService.GetById(pos.ShopId);
                        if (shop != null)
                        {
                            if(ticket.useScope!=shop.Name)
                                return Json(new ApiResponse() { Code = "-1", Msg = "该门票不可以在此门店消费" });
                        }
                       
                    }
                    ticket.State = TicketsState.Used;
                    ticket.userTime = DateTime.Now;
                    ticketsService.Update(ticket);
                    var ticketOff = new TicketOff();
                    ticketOff.code = ticket.Code;
                    ticketOff.DisplayName = admissionTicket != null ? admissionTicket.name : "";
                    ticketOff.offOp = pos.Name;
                    ticketOff.offType = OffTypes.TicKet;
                    ticketOff.shopId = pos.ShopId;
                    ticketOff.subTime = DateTime.Now;
                    ticketOff.timers = 1;
                    ticketOff.userId = ticket.UserId;
                    ticketOffService.Insert(ticketOff);
                    var handRingPrint = new HandRingPrint();
                    var user = membershipService.GetUserById(ticket.UserId) as AccountUser;
                    if (user != null)
                    {
                        handRingPrint.babyBirthDate = user.babyBirthDate.ToString("yyyy-MM-dd hh:mm;ss");
                        handRingPrint.babyName = user.babyName;
                        handRingPrint.babySex = user.babySex == 1 ? "男" : user.babySex == 2 ? "女" : "";
                        handRingPrint.userName = user.DisplayName;
                        handRingPrint.mobile = user.Mobile;
                    }
                    handRingPrint.adultNum = ticket.adultNum;
                    handRingPrint.childNum = ticket.childNum;
                    handRingPrint.code = ticket.Code;
                    handRingPrint.createTime = DateTime.Now;
                    handRingPrint.state = 1;
                    handRingPrint.ticketType = 1;
                    handRingPrint.shopId = pos.ShopId;
                    handRingPrintService.Insert(handRingPrint);
                    transactionHelper.Commit();
                    return Json(new ApiResponse() { Code = "1", Msg = "核销成功" });
                }
                var card = accountService.GetByName(request.code);
                if (card == null)
                {
                    transaction.Dispose();
                    return Json(new ApiResponse() { Code = "-1", Msg = "门票不存在" });
                }
                if (card.State != AccountStates.Normal)
                {
                    transaction.Dispose();
                    return Json(new ApiResponse() { Code = "-1", Msg = "此卡无效" });
                }
                if (card.Frequency <= 0)
                {
                    transaction.Dispose();
                    return Json(new ApiResponse() { Code = "-1", Msg = "此卡无效" });
                }

                if (DateTime.Now.Date > card.ExpiredDate)
                {
                    card.State = AccountStates.UseComplete;
                    card.LastDealTime = DateTime.Now;
                    accountService.Update(card);
                    transactionHelper.Commit();
                    return Json(new ApiResponse() { Code = "-1", Msg = "此卡已过期" });
                }
                if (card.ShopId>0)
                {
                    if(card.ShopId!=pos.ShopId)
                        return Json(new ApiResponse() { Code = "-1", Msg = "该卡不可以在此门店消费" });
                }
                card.Frequency -= 1;
                card.FrequencyUsed += 1;
                card.LastDealTime = DateTime.Now;
                if (card.Frequency <= 0)
                    card.State = AccountStates.UseComplete;
                accountService.Update(card);
                var cardType = accountTypeService.GetById(card.AccountTypeId);
                var ticketOff1 = new TicketOff();
                ticketOff1.code = card.Name;
                ticketOff1.DisplayName = cardType != null ? cardType.DisplayName : "";
                ticketOff1.offOp = pos.Name;
                ticketOff1.offType = OffTypes.Card;
                ticketOff1.shopId = pos.ShopId;
                ticketOff1.subTime = DateTime.Now;
                ticketOff1.timers = 1;
                ticketOff1.userId = card.OwnerId.HasValue?card.OwnerId.Value:0;
                ticketOffService.Insert(ticketOff1);
                var handRingPrint1 = new HandRingPrint();
                var user1 = membershipService.GetUserById(card.OwnerId.HasValue?card.OwnerId.Value:0) as AccountUser;
                if (user1 != null)
                {
                    handRingPrint1.babyBirthDate = user1.babyBirthDate.ToString("yyyy-MM-dd hh:mm;ss");
                    handRingPrint1.babyName = user1.babyName;
                    handRingPrint1.babySex = user1.babySex == 1 ? "男" : user1.babySex == 2 ? "女" : "";
                    handRingPrint1.userName = user1.DisplayName;
                    handRingPrint1.mobile = user1.Mobile;
                }
                handRingPrint1.adultNum = 1;
                handRingPrint1.childNum = cardType.NumberOfPeople;
                handRingPrint1.code = card.Name;
                handRingPrint1.createTime = DateTime.Now;
                handRingPrint1.state = 1;
                handRingPrint1.ticketType = 2;
                handRingPrint1.shopId = pos.ShopId;
                handRingPrintService.Insert(handRingPrint1);
                transactionHelper.Commit();
                return Json(new ApiResponse() { Code = "1", Msg = "核销成功" });
            }
            catch (Exception ex)
            {
                transaction.Dispose();
                WxPayAPI.Log.Info("PosApiController", string.Format("请求获取门票异常：{0}", ex.Message));
                return Json(new ApiResponse() { Code = "-1", Msg = "异常，请联系管理员" });
            }
            

        }

        public  string SaltAndHash(string rawString, string salt)
        {
            byte[] salted = Encoding.UTF8.GetBytes(string.Concat(rawString, salt));

            SHA256 hasher = new SHA256Managed();
            byte[] hashed = hasher.ComputeHash(salted);

            return Convert.ToBase64String(hashed);
        }
    }
}
