using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Shops
{
    public class ListHandRingPrint
    {
        private readonly HandRingPrint _innerObject;

        [NoRender]
        public HandRingPrint InnerObject
        {
            get { return _innerObject; }
        }

        public ListHandRingPrint()
        {
            _innerObject = new HandRingPrint();
        }

        public ListHandRingPrint(HandRingPrint adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int Id
        {
            get { return InnerObject.id; }
        }
        public string Code
        {
            get { return InnerObject.code; }
        }
        
        public string TicketType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.ticketType); }
        }

        public string Mobile
        {
            get { return InnerObject.mobile; }
        }
        public string UserName
        {
            get { return InnerObject.userName; }
        }
        public string BabyName
        {
            get { return InnerObject.babyName; }
        }
        public string BabySex
        {
            get { return InnerObject.babySex; }
        }
        public int ChildNum
        {
            get { return InnerObject.childNum; }
        }
        public int AdultNum
        {
            get { return InnerObject.adultNum; }
        }
        public DateTime? printTime
        {
            get { return InnerObject.printTime; }
        }
        //public string State
        //{
        //    get { return ModelHelper.GetBoundText(InnerObject, x => x.state); }
        //}

        public DateTime createTime
        {
            get { return InnerObject.createTime; }
        }
        [NoRender]
        public string boor { get; set; }
    }
}
