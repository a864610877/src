using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Microsoft.Practices.Unity;
using Ecard.Services;

namespace Ecard.Mvc.Models.DistributorBrokerages
{
    public class ListDistributorBrokerage
    {
        private readonly DistributorBrokerage _innerObject;
        [NoRender]
        public int Id 
        {
            get { return InnerObject.Id; }
        }
        [NoRender]
        public DistributorBrokerage InnerObject
        {
            get { return _innerObject; }
        }
        public ListDistributorBrokerage()
        {
            _innerObject = new DistributorBrokerage();
        }

        public ListDistributorBrokerage(DistributorBrokerage adminUser)
        {
            _innerObject = adminUser;
        }
        [NoRender]
        public int AccountId
        {
            get { return InnerObject.AccountId; }
        }
        [NoRender]
        public int settlementDistributorId { get { return InnerObject.settlementDistributorId; } }
        public string DistributorName { get; set; }
        public string SettlementDistributorName { get; set; }
        public string AccountName { get; set; }

        public DateTime Bdate
        {
            get { return InnerObject.Bdate; }
        }
        public DateTime Edate
        {
            get { return InnerObject.Edate; }
        }

        public decimal Brokerage
        {
            get { return InnerObject.brokerage; }
        }


        public decimal Consume
        {
            get { return InnerObject.consume; }
        }
        [NoRender]
        public int DistributorId
        {
            get { return InnerObject.DistributorId; }
        }
        public decimal Rate
        {
            get { return InnerObject.Rate; }
        }

        [NoRender]
        public bool Status
        {
            get { return InnerObject.status; }
        }
        public string StatusText
        {
            get 
            {
                if (InnerObject.status)
                    return "已结算";
                else
                    return "未结算";
            }
        }


    }
}
