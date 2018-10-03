using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealOnlines
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class PayConfirm
    {
        public string ShopName { get; set; }
        public string AccountName { get; set; }
        public string AccountToken { get; set; }
        public decimal Amount { get; set; }

        public Shop Shop { get; set; }
        public ShopUser ShopOwner { get; set; }
        public Account Account { get; set; }
        public AccountUser AccountOwner { get; set; }
        public void Ready(AdminUser adminUser)
        {
            Shop = ShopService.GetByName(this.ShopName);
            ShopOwner =
                MembershipService.QueryUsersOfShops<ShopUser>(UserStates.Normal, ShopRoles.Owner, Shop.ShopId).
                    FirstOrDefault();
            Account = AccountService.GetByName(AccountName);
            if (Account.OwnerId.HasValue)
            AccountOwner = (AccountUser) MembershipService.GetUserById(Account.OwnerId.Value);
        }
        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        public IAccountService AccountService { get; set; }
    }
}