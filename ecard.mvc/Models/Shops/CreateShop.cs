using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.Shops
{
    public class CreateShop : ShopModelBase, IValidator
    {
        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }

        [Required(ErrorMessage = "请输入商户编号")]
        [StringLength(15, ErrorMessage = "商户编号长度不可超过 15")]
        [UIHint("ShopName")]
        public string ShopName
        {
            get { return InnerObject.Name; }
            set { InnerObject.Name = value.TrimSafty(); }
        }

        [Required(ErrorMessage = "请输入电子邮件")]
        [StringLength(50)]
        public string Email
        {
            get { return Owner.Email; }
            set { Owner.Email = value.TrimSafty(); }
        }
        [Required(ErrorMessage = "请输入商户登录姓名")]
        [StringLength(20)]
        public string UserName
        {
            get { return Owner.Name; }
            set { Owner.Name = value.TrimSafty(); }
        }

        [NoRender, Dependency]
        public Site Site { get; set; }
        [NoRender, Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }

        #region IValidator Members

        public IEnumerable<ValidationError> Validate()
        {
            if (ShopService.Query(new ShopRequest { Name = ShopName }).Count() > 0)
                yield return new ValidationError("ShopName", string.Format(Localize("messages.duplicationShop"), ShopName));
            if (MembershipService.GetUserByName(UserName) != null)
                yield return new ValidationError("UserName", string.Format(Localize("messages.duplicationUser"), UserName));
            if (!string.IsNullOrWhiteSpace(Mobile.Value1) && MembershipService.GetUserByMobile<ShopUser>(this.Mobile.Value1, 0).Count() > 0)
                yield return new ValidationError("Mobile", string.Format(Localize("messages.duplicationMobile"), Mobile.Value1));
            if (!string.IsNullOrWhiteSpace(PhoneNumber.Value1) && ShopService.GetShopByMobileNumber(this.PhoneNumber.Value1, 0).Count() > 0)
                yield return new ValidationError("PhoneNumber", string.Format(Localize("messages.duplicationPhoneNumber"), PhoneNumber.Value1));

        }

        #endregion

        protected override void OnDisplayNameChanged(string value)
        {
            Owner.DisplayName = value;
            base.OnDisplayNameChanged(value);
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            var roles = MembershipService.QueryRoles(new RoleRequest { Name = RoleNames.ShopOwner }).ToList();

            TransactionHelper.BeginTransaction();
            OnSave(InnerObject, Owner);
            Owner.SetPassword(Password);
            InnerObject.State = States.Normal;
            InnerObject.RechargeAmount = RechargeAmount;
            Owner.State = States.Normal;
            ShopService.Create(InnerObject);
            Owner.ShopId = InnerObject.ShopId;
            MembershipService.CreateUser(Owner);
            MembershipService.AssignRoles(Owner, roles.Select(x => x.RoleId).ToArray());
            ShopDealLog log = new ShopDealLog(serialNo, DealTypes.Open, 0, null, null, null, InnerObject, 0);
            ShopDealLogService.Create(log);
            AddMessage("success", ShopName);
            Logger.LogWithSerialNo(LogTypes.ShopCreate, serialNo, InnerObject.ShopId, ShopName);
            CacheService.Refresh(CacheKeys.PosKey);
            return TransactionHelper.CommitAndReturn(this);
        }
    }
}