using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.DealOnlines
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class PayShop
    {
        private string _shopName;
        public string ShopName
        {
            get { return _shopName.TrimSafty(); }
            set { _shopName = value; }
        }

        public Shop Shop{ get; set; }
        public ShopUser ShopOwner { get; set; }
        public void Ready()
        {
            this.Shop = ShopService.GetByName(ShopName);
            if (Shop != null)
                ShopOwner = MembershipService.QueryUsersOfShops<ShopUser>(
                                UserStates.Normal, ShopRoles.Owner, Shop.ShopId).
                            FirstOrDefault();
        }

        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
    }
}