using Ecard.Services;
using MicroMall.Models;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace MicroMall.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        private readonly IMembershipService membershipService;

        public LoginController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        public ActionResult Index()
        {
            if (Session[SessionKeys.USERID] == null)
            {
                string url = "";
                return Redirect(url);
            }
            return RedirectToAction("index", "PersonalCentre");

        }

        public ActionResult login(string code, string state)
        {
            OAuthAccessTokenResult result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(WxPayConfig.APPID, WxPayConfig.APPSECRET,code);
            var openid = result.openid;
            var user = membershipService.GetByOpenId(openid);
            if (user != null)
            {
                HttpCookie cookie = new HttpCookie(SessionKeys.USERID, user.UserId.ToString());
                Response.Cookies.Add(cookie);
                return RedirectToAction("index", "PersonalCentre");
            }
            else
            {
                return RedirectToAction("index", "register");
            }
        }

    }
}
