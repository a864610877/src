using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.DealOnlines
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class Shops
    {
        private string _shopName;
        public string ShopName
        {
            get { return _shopName.TrimSafty(); }
            set { _shopName = value; }
        }
        public List<Shop> Items { get; set; }
        public void Ready(AdminUser adminUser)
        {
            Items = ShopService.QueryWithName(this.ShopName).ToList();
        }
        [Dependency]
        public IShopService ShopService { get; set; }
    }
}