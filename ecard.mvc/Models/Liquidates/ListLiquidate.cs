using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.Liquidates
{
    public class ListLiquidate
    {
        private readonly Liquidate _innerObject;

        [NoRender]
        public Liquidate InnerObject
        {
            get { return _innerObject; }
        }

        public ListLiquidate()
        {
            _innerObject = new Liquidate();
        }

        public ListLiquidate(Liquidate innerObject)
        {
            _innerObject = innerObject;
        }

        public LinkObject LiquidateId
        {
            get { return new LinkObject(InnerObject.LiquidateId.ToString(), "Liquidate", "View", new { id = _innerObject.LiquidateId }); }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }
        public int Count
        {
            get { return InnerObject.Count; }
        }

        public decimal DealAmount
        {
            get { return InnerObject.DealAmount; }
        }
        public decimal CancelAmount
        {
            get { return InnerObject.CancelAmount; }
        }
        public string ShopName { get; set; }
    }
}