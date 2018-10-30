using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Orderss
{
    public class ListOrders
    {
        private readonly Ordersss _innerObject;

        [NoRender]
        public Ordersss InnerObject
        {
            get { return _innerObject; }
        }

        public ListOrders()
        {
            _innerObject = new Ordersss();
        }

        public ListOrders(Ordersss adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int Id
        {
            get { return InnerObject.id; }
        }

        public string OrderNo
        {
            get { return InnerObject.orderNo; }
            set { _innerObject.orderNo = value; }
        }
        public string Mobile
        {
            get { return InnerObject.mobile; }
        }
        public string UserDisplayName
        {
            get { return InnerObject.userDisplayName; }
        }
        public string Type
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.type); }
        }
        public string OrderState
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.orderState); }
        }
        public decimal Amount
        {
            get { return InnerObject.amount; }
        }
        public decimal Deductible
        {
            get { return InnerObject.deductible; }
        }
        public decimal PayAmount
        {
            get { return InnerObject.payAmount; }
        }
        public string ShopName
        {
            get { return InnerObject.shopName; }
        }
        public DateTime SubTime
        {
            get { return InnerObject.subTime; }
        }

    }
}
