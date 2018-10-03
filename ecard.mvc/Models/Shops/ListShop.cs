using Ecard.Models;

namespace Ecard.Mvc.Models.Shops
{
    public class ListShop
    {
        private readonly Shop _innerObject;
        private ShopUser _owner;

        [NoRender]
        public Shop InnerObject
        {
            get { return _innerObject; }
        }

        public ListShop()
        {
            _innerObject = new Shop();
        }

        public ListShop(Shop adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int ShopId
        {
            get { return InnerObject.ShopId; }
        }
        public string Name
        {
            get { return InnerObject.Name; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }

        [Sort(null)]
        public string OwnerName
        {
            get { return  Owner.Name; }
        }
        [Sort(null)]
        public string Email
        {
            get { return  Owner.Email; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        internal ShopUser Owner
        {
            get {
                //yangwen 2012-06-19
                if (_owner == null)
                    return new ShopUser();
                return _owner;
            }
            set {
                _owner = value;
            }
        }
        [NoRender]
        public string boor { get; set; }
    }
}