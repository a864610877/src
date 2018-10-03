using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class ListGood
    {

        private readonly Good _innerObject;
        [NoRender]
        public Good InnerObject { get { return _innerObject; } }
        public ListGood()
        {
            this._innerObject = new Good();
        }

        public ListGood(Good item)
        {
            this._innerObject = item;
        }
        public int GoodId { get { return InnerObject.GoodId; } }
        public string GoodName { get { return InnerObject.GoodName; } }
        public string State { get { return ModelHelper.GetBoundText(InnerObject, x => x.State); } }

    }
}
