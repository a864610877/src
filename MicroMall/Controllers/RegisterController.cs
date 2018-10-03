﻿using Ecard.Models;
using Ecard.Services;
using MicroMall.Models;
using MicroMall.Models.Registers;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace MicroMall.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/


        private readonly IMembershipService membershipService;
        public RegisterController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }
        public ActionResult Index()
        {
            string url = "";
            return View();
        }

        public ActionResult register(string code)
        {
            return View();
        }
        [HttpPost]
        public ActionResult register(register register)
        {
            if (string.IsNullOrWhiteSpace(register.code))
            {
                return Json(new ResultMessage() { Code = -1, Msg = "授权失效，请重新进入" });
            }

            if (string.IsNullOrWhiteSpace(register.name))
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请输入姓名" });
            }
            if (register.sex<=0)
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请选择性别" });
            }
            if (string.IsNullOrWhiteSpace(register.babyName))
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请输入宝宝姓名" });
            }
            if (register.babySex<=0)
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请选择宝宝性别" });
            }
            if (string.IsNullOrWhiteSpace(register.mobile))
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请输入手机号" });
            }
            if (string.IsNullOrWhiteSpace(register.verifiCode))
            {
                return Json(new ResultMessage() { Code = -1, Msg = "请输入验证码" });
            }

            var mobileUser = membershipService.GetByMobile(register.mobile);
            if(mobileUser!=null)
                return Json(new ResultMessage() { Code = -1, Msg = "手机号码已注册" });
            WxPayAPI.Log.Info(this.GetType().ToString(), string.Format("---进入了用户信息回调---"));
            WxPayAPI.Log.Info(this.GetType().ToString(), string.Format("---code:{0},state:{1}---", register.code, ""));
            //jsApiPay.GetOpenidAndAccessTokenFromCode(code);
            if (string.IsNullOrEmpty(register.code))
            {
                //return Json(new ResultMessage() { Code = -1, Msg = "参数错误，请联系管理员！" });
                return Content("您拒绝了授权！");
            }
            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                //var model = Iwx_interfaceService.GetModel(new KodyCRM.DomainModels.Query.Admin.wx_interfaceQuery());
                result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(WxPayConfig.APPID, WxPayConfig.APPSECRET, register.code);
                var openid = result.openid;
                OAuthUserInfo userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(result.access_token, result.openid);
                AccountUser user = new AccountUser();
                user.Mobile = register.mobile;
                user.Name = register.mobile;
                user.babyName = register.babyName;
                user.babySex = register.babySex;
                user.DisplayName = register.name;
                user.Gender = register.sex;
                user.State = UserStates.Normal;
                user.babyBirthDate = register.babyBirthDate;
                user.openId = openid;
                membershipService.CreateUser(user);

               // membershipService.AssignRoles(user, 0);
               
            }
            catch (Exception ex)
            {
                WxPayAPI.Log.Info(this.GetType().ToString(), string.Format("---openid:出错---"));
                return Content("您拒绝了授权！");
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                //return Json(new Result(-2, "请求失效,请重新进入购票"));
                return Content("错误：" + result.errmsg);
            }
            return Json(new ResultMessage() {Code=0 });

        }

    }
}
