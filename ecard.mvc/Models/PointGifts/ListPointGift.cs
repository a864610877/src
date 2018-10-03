using Ecard.Models;

namespace Ecard.Mvc.Models.PointGifts
{
    public class ListPointGift
    {
        private readonly PointGift _innerObject;

        [NoRender]
        public PointGift InnerObject
        {
            get { return _innerObject; }
        }

        public ListPointGift()
        {
            _innerObject = new PointGift();
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        public string Category
        {
            get { return InnerObject.Category; }
        }
        public string Description
        {
            get { return InnerObject.Description; }
        }
        public int Point
        {
            get { return InnerObject.Point; }
        }
        public int Priority
        {
            get { return InnerObject.Priority; }
        }
        public ListPointGift(PointGift innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int PointGiftId
        {
            get { return InnerObject.PointGiftId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}