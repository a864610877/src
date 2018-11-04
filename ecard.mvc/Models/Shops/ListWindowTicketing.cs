using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Shops
{
    public class ListWindowTicketing
    {
        private readonly WindowTicketings _innerObject;

        [NoRender]
        public WindowTicketings InnerObject
        {
            get { return _innerObject; }
        }

        public ListWindowTicketing()
        {
            _innerObject = new WindowTicketings();
        }

        public ListWindowTicketing(WindowTicketings adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int id
        {
            get { return InnerObject.id; }
        }
        public string Code
        {
            get { return InnerObject.code; }
        }
        public string TicketName
        {
            get { return InnerObject.ticketName; }
        }

        public string ShopName
        {
            get { return InnerObject.shopName; }
        }
        public string ShopDisplayName
        {
            get { return InnerObject.shopDisplayName; }
        }
        public decimal Amount
        {
            get { return InnerObject.amount; }
        }
        public decimal Price
        {
            get { return InnerObject.price; }
        }
        public decimal Discount
        {
            get { return InnerObject.discount; }
        }
        public int Num
        {
            get { return InnerObject.num; }
        }
        public string PayType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.payType); }
        }
        public string DisplayName
        {
            get { return InnerObject.displayName; }
        }
        public string Mobile
        {
            get { return InnerObject.mobile; }
        }
        public string BabyName
        {
            get { return InnerObject.babyName; }
        }
        public DateTime? BabyBirthDate
        {
            get { return InnerObject.babyBirthDate; }
        }
        public string BabySex
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.babySex); }
        }

        public DateTime CreateTime
        {
            get { return InnerObject.createTime; }
        }
    }
}
