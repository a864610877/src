//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Moonlit.Collections;

namespace Ecard.Mvc.ActionFilters
{
    /// <summary>
    /// 负责 对 oxiteModel 的用户进行处理，包括处理 model.User 和 currentUser 参数
    /// </summary>
    public class UserActionFilter : IActionFilter
    {
        private readonly SecurityHelper _securityHelper;

        public UserActionFilter(SecurityHelper securityHelper)
        {
            _securityHelper = securityHelper;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            EcardModel model = filterContext.Controller.ViewData.Model as EcardModel;

            if (model != null)
            {
                var user = filterContext.HttpContext.User.Identity.IsAuthenticated
                    ? _securityHelper.GetCurrentUser()
                    : null;

                if (user != null)
                {
                    model.User = user.CurrentUser;
                    model.UserModel = user;
                }
            }
        }



        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey("currentUser"))
            {
                UserModel userModel = null;

                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                    userModel = _securityHelper.GetCurrentUser();
                if (userModel != null)
                    filterContext.ActionParameters["currentUser"] = userModel.CurrentUser;
            }
        }
    }

    public class AccountUserModel : UserModel
    {
        private readonly AccountUser _accountUser;

        public AccountUserModel(AccountUser accountUser, IEnumerable<Account> accounts)
        {
            _accountUser = accountUser;

            Accounts = new List<Account>(accounts);
        }

        public string UserName
        {
            get { return InnerObject.Name; }
        }
        public string UserDisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        public IEnumerable<Account> Accounts { get; private set; }

        protected AccountUser InnerObject
        {
            get { return _accountUser; }
        }
        public DateTime? LastSignInTime
        {
            get { return InnerObject.LastSignInTime; }
        }

        public override User CurrentUser
        {
            get { return _accountUser; }
        }

    }

    public class ShopUserModel : UserModel
    {
        private readonly ShopUser _shopUser;
        private readonly Shop _shop;

        public ShopUserModel(ShopUser shopUser, Shop shop)
        {
            _shopUser = shopUser;
            _shop = shop;
        }

        public string ShopName
        {
            get { return _shop.Name; }
        }

        public string ShopDisplayName
        {
            get { return _shop.DisplayName; }
        }

        public ShopUser InnerObject
        {
            get { return _shopUser; }
        }

        public string UserName
        {
            get { return InnerObject.Name; }
        }
        public string UserDisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        public DateTime? LastSignInTime
        {
            get { return InnerObject.LastSignInTime; }
        }

        public override User CurrentUser
        {
            get { return _shopUser; }
        }

        public int ShopId
        {
            get { return _shopUser.ShopId; }
        }
    }

    public class AdminUserModel : UserModel
    {
        private readonly AdminUser _adminUser;

        public AdminUserModel(AdminUser adminUser)
        {
            _adminUser = adminUser;

            Roles = adminUser.Roles.Select(x => x.DisplayName).ToList();
        }

        public string Name
        {
            get { return InnerObject.Name; }
        }

        public DateTime? LastSignInTime
        {
            get { return InnerObject.LastSignInTime; }
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        public List<string> Roles { get; set; }
        protected AdminUser InnerObject
        {
            get { return _adminUser; }
        }

        public override User CurrentUser
        {
            get { return _adminUser; }
        }
    }
    public class DistributorUserModel : UserModel
    {
        private readonly DistributorUser _distributorUser;
        private readonly Distributor _distributor;

        public DistributorUserModel(DistributorUser distributorUser, Distributor distributor)
        {
            _distributorUser = distributorUser;
            _distributor = distributor;

            Roles = distributorUser.Roles.Select(x => x.DisplayName).ToList();
        }

        public string Name
        {
            get { return InnerObject.Name; }
        }

        public DateTime? LastSignInTime
        {
            get { return InnerObject.LastSignInTime; }
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        public List<string> Roles { get; set; }
        protected DistributorUser InnerObject
        {
            get { return _distributorUser; }
        }

        public override User CurrentUser
        {
            get { return _distributorUser; }
        }

        public int DistributorId
        {
            get { return _distributor.DistributorId; }
        }
    }
}
