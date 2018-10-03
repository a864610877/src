using Ecard.Models;

namespace Ecard.Mvc.Models.Sites
{
    public class ListMessageShop
    {
        private readonly Shop InnerObject;

        public ListMessageShop(ShopWithOwner shop )
        {
            InnerObject = shop;
        }

        [NoRender]
        public int ShopId
        {
            get { return InnerObject.ShopId; }
        }
        public string ShopName
        {
            get { return InnerObject.Name; }
        }

        public string ShopDisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public string Mobile
        {
            get { return InnerObject.Mobile; }
        }

    }
}