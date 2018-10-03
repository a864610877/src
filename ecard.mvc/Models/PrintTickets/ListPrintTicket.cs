using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.PrintTickets
{
    public class ListPrintTicket
    {
        private readonly PrintTicket _innerObject;

        [NoRender]
        public PrintTicket InnerObject
        {
            get { return _innerObject; }
        }

        public ListPrintTicket()
        {
            _innerObject = new PrintTicket();
        }
        public int PrintCount
        {
            get { return InnerObject.PrintCount; }
        }
        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
        }

        public string LogType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.LogType); }
        }

        public string SerialNo
        {
            get { return _innerObject.SerialNo; }
        }
        public string AccountName
        {
            get { return InnerObject.AccountName; }
        }
        public ListPrintTicket(PrintTicket innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int PrintTicketId
        {
            get { return InnerObject.PrintTicketId; }
        }
    }
}