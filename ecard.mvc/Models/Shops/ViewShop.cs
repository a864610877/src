using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Shops
{
    public class ViewShop : ViewModelBase
    {
        private Shop _innerObject;
        private ShopUser _owner;
        protected void SetInnerObject(Shop shop, ShopUser owner)
        {
            _owner = owner;
            _innerObject = shop;
        }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }

        public decimal? ShopDealLogChargeRate
        {
            get { return _innerObject.ShopDealLogChargeRate.HasValue ? _innerObject.ShopDealLogChargeRate.Value * 100 : (decimal?)null; }
        }
        public string DisplayName
        {
            get { return _innerObject.DisplayName; }
        }
        public string Name
        {
            get { return _innerObject.Name; }
        }
        public string OwnerName
        {
            get { return _owner.Name; }
        }
        public string OwnerDisplayName
        {
            get { return _owner.DisplayName; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(_innerObject, x => x.State); }
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