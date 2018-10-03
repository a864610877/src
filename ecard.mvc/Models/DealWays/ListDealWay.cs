using Ecard.Models;

namespace Ecard.Mvc.Models.DealWays
{
    public class ListDealWay
    {
        private readonly DealWay _innerObject;

        [NoRender]
        public DealWay InnerObject
        {
            get { return _innerObject; }
        }

        public ListDealWay()
        {
            _innerObject = new DealWay();
        }

        public ListDealWay(DealWay innerObject)
        {
            _innerObject = innerObject;
        }
         
        public bool IsCash
        {
            get { return _innerObject.IsCash; }
        }
        public string DisplayName
        {
            get { return _innerObject.DisplayName; }
        } 
        public ApplyToModel ApplyTo
        {
            get
            {
                return new ApplyToModel(_innerObject.ApplyTo);
            }
        }
        [NoRender]
        public int DealWayId
        {
            get { return InnerObject.DealWayId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}