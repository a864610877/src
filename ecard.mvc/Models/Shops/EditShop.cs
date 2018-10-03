using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Shops
{
    [Bind(Prefix = "Item")]
    public class EditShop : ShopModelBase, IValidator
    {
        public string Name
        {
            get { return InnerObject.Name; }
        }


        public string UserName
        {
            get { return Owner.Name; }
        }

        [Hidden]
        public int ShopId
        {
            get { return InnerObject.ShopId; }
            set { InnerObject.ShopId = value; }
        }

        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }
        [Dependency, NoRender]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            var shop = ShopService.GetById(ShopId);

            if (shop != null)
            {
                var owner = MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, shop.ShopId).FirstOrDefault();
                shop.DisplayName = DisplayName;
                shop.ShopDealLogChargeRate = InnerObject.ShopDealLogChargeRate;
                shop.RechargeAmount = RechargeAmount;
                if (!string.IsNullOrEmpty(Password))
                {
                    if (owner == null)
                    {
                        AddError(LogTypes.ShopEdit, "noOwner");
                        return this;
                    }
                    owner.SetPassword(Password);
                } 
                OnSave(shop, owner);
                ShopService.Update(shop);
                MembershipService.UpdateUser(owner);
                AddMessage("success", shop.Name);
                Logger.LogWithSerialNo(LogTypes.ShopEdit, serialNo, shop.ShopId, shop.Name);
                CacheService.Refresh(CacheKeys.PosKey);
            }
            return this;
        }

        public IEnumerable<ValidationError> Validate()
        {
            var users = MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, InnerObject.ShopId);
            var owner = users.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(Mobile.Value1) && MembershipService.GetUserByMobile<ShopUser>(this.Mobile.Value1, owner.UserId).Count() > 0)
                yield return new ValidationError("Mobile", string.Format(Localize("messages.duplicationMobile"), Mobile.Value1));
            if (!string.IsNullOrWhiteSpace(PhoneNumber.Value1) && ShopService.GetShopByMobileNumber(this.PhoneNumber.Value1, InnerObject.ShopId).Count() > 0)
                yield return new ValidationError("PhoneNumber", string.Format(Localize("messages.duplicationPhoneNumber"), PhoneNumber.Value1));
        }

        public void Read(int id)
        {
            var shop = ShopService.GetById(id);
            if (shop != null)
            {
                var owner = MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, id).FirstOrDefault();
                SetInnerObject(shop, owner);
            }
        }
    }
}