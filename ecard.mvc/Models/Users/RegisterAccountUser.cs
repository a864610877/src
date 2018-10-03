using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Users
{
    public class RegisterAccountUser
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string AccountPassword { get; set; }
        [Required]
        public string UserPassword { get; set; }
        public string Code { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        public LogHelper Logger { get; set; }

        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }

        public SimpleAjaxResult Register()
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                var account = AccountService.GetByName(AccountName);
                if (account == null || (account.State != AccountStates.Invalid && account.State != AccountStates.Normal))
                    return new SimpleAjaxResult("卡号不存在或已注册");
                if (User.SaltAndHash(AccountPassword, account.PasswordSalt) != account.Password)
                    return new SimpleAjaxResult("卡号不存在或已注册");
                AccountUser owner = null;
                bool hasOwner = false;
                if (account.OwnerId.HasValue)
                {
                    owner = MembershipService.GetUserById(account.OwnerId.Value) as AccountUser;
                    hasOwner = true;
                    if (owner.SignOnTime != null)
                    {
                        Logger.Error(LogTypes.RegisterAccountUser, "卡号 " + AccountName + " 不存在或已注册");
                        return new SimpleAjaxResult("卡号不存在或已注册");
                    }
                }
                {
                    var user = MembershipService.GetUserByName(UserName);
                    if (user != null)
                    {
                        Logger.Error(LogTypes.RegisterAccountUser, "用户名 " + UserName + " 已存在");
                        return new SimpleAjaxResult("用户名已存在");
                    }
                }

                owner = owner ?? new AccountUser();
                owner.Name = UserName;
                owner.DisplayName = string.IsNullOrWhiteSpace(owner.DisplayName) ? UserName : owner.DisplayName;
                owner.SetPassword(UserPassword);
                owner.SignOnTime = DateTime.Now;
                TransactionHelper.BeginTransaction();
                if (!hasOwner)
                {
                    MembershipService.CreateUser(owner);
                    account.OwnerId = owner.UserId;
                    AccountService.Update(account);
                }
                else
                {
                    MembershipService.UpdateUser(owner);
                }
                if (owner.Roles == null || owner.Roles.Count == 0)
                    MembershipService.AssignRoles(owner, MembershipService.QueryRoles(new RoleRequest() { Name = RoleNames.Account }).Select(x => x.RoleId).ToArray());

                Logger.LogWithSerialNo(LogTypes.RegisterAccountUser, serialNo, owner.UserId, owner.Name, AccountName);
                TransactionHelper.Commit();
                return new SimpleAjaxResult();
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.RegisterAccountUser, ex);
                return new SimpleAjaxResult("系统错误");
            }
        }
    }
}