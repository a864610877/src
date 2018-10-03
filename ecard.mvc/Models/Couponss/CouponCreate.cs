using Ecard.Models;
using Ecard.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Couponss
{
    public class CouponCreate : CouponModelBase, IValidator
    {
        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
        public IMessageProvider Save()
        {
            var model = new Coupons();
            model.code =DateTime.Now.ToString("yyyyMMddHHmmssffff");
            model.name = this.name;
            model.state = CouponsState.Invalid;
            model.createOp = SecurityHelper.GetCurrentUser().CurrentUser.Name;
            model.createTime =DateTime.Now;
            model.discount = this.discount;
            model.deductibleAmount = this.deductibleAmount;
            model.fullAmount = this.fullAmount;
            model.reduceAmount = this.reduceAmount;
            model.leadersOfNum =0;
            model.quantity = this.quantity;
            if (Shop.Key == Globals.All)
            {
                model.useScope = "";
            }
            else
            {
                var shop = ShopService.GetById(Shop.Key);
                if (shop != null)
                    model.useScope = shop.Name;
                else
                {
                    AddError(LogTypes.CouponCreate, "商户不存在");
                    return this;
                }
            }
            model.validity = this.validity;
            model.couponsType = CouponType.Key;
            CouponsService.Create(model);
            AddMessage("success", model.name);
            Logger.LogWithSerialNo(LogTypes.CouponCreate, model.name, InnerObject.id, model);
            return this;
        }
    }
}
