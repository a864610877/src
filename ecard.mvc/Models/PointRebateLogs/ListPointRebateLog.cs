using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.PointRebateLogs
{
    public class ListPointRebateLog
    {
        private readonly PointRebateLog _innerObject;

        [NoRender]
        public PointRebateLog InnerObject
        {
            get { return _innerObject; }
        }

        public ListPointRebateLog()
        {
            _innerObject = new PointRebateLog();
        }

        public ListPointRebateLog(PointRebateLog innerObject)
        {
            _innerObject = innerObject;
        }
        public DateTime SubmitTime { get { return InnerObject.SubmitTime; } }
        public decimal Amount { get { return InnerObject.Amount; } }
        public int Point { get { return InnerObject.Point; } } 
        public string UserName { get; set; }
        public string PointRebateName { get; set; }
        public string AccountName { get; set; }
        public string AccountShopName { get; set; }
    }
}