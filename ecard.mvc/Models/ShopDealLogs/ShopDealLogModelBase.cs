using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopDealLogs
{
    public class ShopDealLogModelBase : ViewModelBase
    {
        private Liquidate _innerObject;

        public ShopDealLogModelBase()
        {
            _innerObject = new Liquidate();
        }

        public ShopDealLogModelBase(Liquidate shop)
        {
            _innerObject = shop;
        }

        [NoRender]
        public Liquidate InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(Liquidate item)
        {
            _innerObject = item;
        }


        [Dependency]
        [NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
    }
}