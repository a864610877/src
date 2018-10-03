using Ecard.Models;
using Ecard.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Couponss
{
    public class CouponEdit : CouponModelBase, IValidator
    {
        [Hidden]
        public int Id
        {
            get { return InnerObject.id; }
            set { InnerObject.id = value; }
        }
        public string code { get { return InnerObject.code; } }

        public void Read(int id)
        {
            var item = CouponsService.GetById(id);
            if (item != null)
                SetInnerObject(item);
            Ready();
            Shop.Key = item.id;
        }

        public IMessageProvider Save()
        {
            var item = CouponsService.GetById(Id);
            if (item != null)
            {
                item.name = this.name;
                item.discount = this.discount;
                item.deductibleAmount = this.deductibleAmount;
                item.fullAmount = this.fullAmount;
                item.reduceAmount = this.reduceAmount;
                item.quantity = this.quantity;
                if (Shop.Key == Globals.All)
                {
                    item.useScope = "";
                }
                else
                {
                    var shop = ShopService.GetById(Shop.Key);
                    if (shop != null)
                        item.useScope = shop.Name;
                    else
                    {
                        AddError(LogTypes.CouponEdit, "商户不存在");
                        return this;
                    }
                }
                item.validity = this.validity;
                item.couponsType = CouponType.Key;
                CouponsService.Update(item);
                Logger.LogWithSerialNo(LogTypes.CouponEdit, item.name, InnerObject.id, item);
               
            }
            AddMessage("success", item.name);
            SetInnerObject(item);
            Ready();
            if (string.IsNullOrWhiteSpace(item.useScope))
            {
                Shop.Key = Globals.All;
            }
            else
            {
                var shop = ShopService.GetByName(item.useScope);
                if (shop != null)
                {
                    Shop.Key = shop.ShopId;
                }
                else
                {
                    Shop.Key = Globals.All;
                }
            }
            
            return this;

        }

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
    }
}
