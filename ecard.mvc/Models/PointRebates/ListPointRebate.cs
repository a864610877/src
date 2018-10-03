using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;

namespace Ecard.Mvc.Models.PointRebates
{
    public class ListPointRebate
    {
        private readonly PointRebate _innerObject;

        [NoRender]
        public PointRebate InnerObject
        {
            get { return _innerObject; }
        }

        public ListPointRebate()
        {
            _innerObject = new PointRebate();
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        } 
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }
        public int Point
        {
            get { return InnerObject.Point; }
        }
        public ListPointRebate(PointRebate innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int PointRebateId
        {
            get { return InnerObject.PointRebateId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}
