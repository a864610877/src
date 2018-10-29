using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.TicketOffss
{
    public class ListTicketOff
    {
        private readonly TicketOffs _innerObject;

        [NoRender]
        public TicketOffs InnerObject
        {
            get { return _innerObject; }
        }

        public ListTicketOff()
        {
            _innerObject = new TicketOffs();
        }

        public ListTicketOff(TicketOffs adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int Id
        {
            get { return InnerObject.id; }
        }

        //public bool Liquidated
        //{
        //    get { return InnerObject.LiquidateDealLogId != 0; }
        //}
        public string Code
        {
            get { return InnerObject.code; }
            set { _innerObject.code = value; }
        }


        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        public string OffType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.offType); }
        }
        public int Timers
        {
            get { return InnerObject.timers; }
        }

        public string ShopDisplayName
        {
            get { return InnerObject.shopDisplayName; }
        }
        public string ShopName
        {
            get { return InnerObject.shopName; }
        }

        public string mobile
        {
            get { return InnerObject.mobile; }
        }
        public string userDisplayName
        {
            get { return InnerObject.userDisplayName; }
        }

        public DateTime SubmitTime
        {
            get { return InnerObject.subTime; }
        }
    }
}
