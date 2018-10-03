using Ecard.Models;
using Microsoft.Practices.Unity;
using Ecard.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ecard.Mvc.Models.Distributors
{
    public class ListDistributor
    {
        private readonly Distributor _innerObject;
        private DistributorUser _owner;

        [NoRender]
        public Distributor InnerObject
        {
            get { return _innerObject; }
        }

        public ListDistributor()
        {
            _innerObject = new Distributor();
        }

        public ListDistributor(Distributor adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public int DistributorId
        {
            get { return InnerObject.DistributorId; }
        }
        public string Name
        {
            get { if (Owner != null) return Owner.Name; else return ""; }
        }

        public string Level { get { if (_innerObject.DistributorLevel == 1)return "总经销商"; else return "分经销商"; } }
        public string DisplayName
        {
            get { if (Owner != null) return Owner.DisplayName; else return ""; }
        }
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }

        [Sort(null)]
        public string OwnerName
        {
            get { return Owner.Name; }
        }
        [Sort(null)]
        public string Email
        {
            get { return Owner.Email; }
        }
        public decimal Rate
        {
            get;
            set;
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        internal DistributorUser Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }
        [NoRender]
        public string boor { get; set; }
    }
}