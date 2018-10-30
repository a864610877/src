using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Ticketss
{
    public class ListTickets
    {
        private readonly Ecard.Models.Ticketss _innerObject;

        [NoRender]
        public Ecard.Models.Ticketss InnerObject
        {
            get { return _innerObject; }
        }

        public ListTickets()
        {
            _innerObject = new Ecard.Models.Ticketss();
        }

        public ListTickets(Ecard.Models.Ticketss adminUser)
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
        public string Code
        {
            get { return InnerObject.Code; }
        }
        public string TicketName
        {
            get { return InnerObject.TicketName; }
        }
        public string Mobile
        {
            get { return InnerObject.Mobile; }
        }
        public string UserDisplayName
        {
            get { return InnerObject.UserDisplayName; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public int ChildNum
        {
            get { return InnerObject.childNum; }
        }
        public int AdultNum
        {
            get { return InnerObject.adultNum; }
        }
        public DateTime ExpiredDate
        {
            get { return InnerObject.ExpiredDate; }
        }
        public string ShopName
        {
            get { return string.IsNullOrWhiteSpace(InnerObject.ShopName)?"全部": InnerObject.ShopName; }
        }
        public DateTime BuyTime
        {
            get { return InnerObject.BuyTime; }
        }
    }
}
