using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class ListOrder
    {
        public readonly Order1 _innerObject;
        [NoRender]
        public Order1 InnerObject { get { return this._innerObject; } }

        public ListOrder()
        { _innerObject = new Order1(); }
        public ListOrder(Order1 order)
        {
            _innerObject = order;
        }
        public LinkObject OrderId { get { return new LinkObject(InnerObject.OrderId,InnerObject.Serialnumber,  "Order","Show" ); } }
        public string Creater { get { return InnerObject.Creater; } }
        public string Sender { get { return InnerObject.Sender; } }
        public DateTime CreateDate { get { return InnerObject.createDate; } }
        public DateTime OrderDate { get { return InnerObject.SubmitTime; } }
        public decimal TotalMoney { get { return InnerObject.TotalMoney; } }
        //public string Creater { get { return InnerObject.Creater; } }
        [NoRender]
        public int Serialnumber { get { return InnerObject.Serialnumber; } }

        public string State 
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }        
        }
        public string AccountName { get; set; }

    }
}
