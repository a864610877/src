using Ecard.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Mvc.Models.Commodities
{
    public class ListCommodity : IKeyObject
    {
        private readonly Commodity _innerObject;

        [NoRender]
        public Commodity InnerObject
        {
            get { return _innerObject; }
        }

        public ListCommodity()
        {
            _innerObject = new Commodity();
        }

        public ListCommodity(Commodity innerObject)
        {
            _innerObject = innerObject;
        }

        public string Name
        {
            get { return InnerObject.Name; }
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        [Range(0, double.MaxValue)]
        public decimal Price
        {
            get { return InnerObject.Price; }
        }
        [NoRender]
        public int CommodityId
        {
            get { return InnerObject.CommodityId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
        [NoRender]
        public int Id
        {
            get { return CommodityId; }
        }
    }
}