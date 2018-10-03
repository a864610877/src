using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Distributors
{
    public class ViewDistributor : ViewModelBase
    {
        private Distributor _innerObject;
        private DistributorUser _owner;
        protected void SetInnerObject(Distributor distributor, DistributorUser owner)
        {
            _owner = owner;
            _innerObject = distributor;
        }

        [NoRender, Dependency]
        public IDistributorService DistributorService { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }
         
        public string DisplayName
        {
            get { return _owner.DisplayName; }
        }
        public string Name
        {
            get { return _owner.Name; }
        }
        public string OwnerName
        {
            get { return _owner.Name; }
        }
        public string OwnerDisplayName
        {
            get { return _owner.DisplayName; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(_innerObject, x => x.State); }
        }

        public void Read(int id)
        {
            var distributor = DistributorService.GetById(id);
            if (distributor != null)
            {
                var owner = MembershipService.GetUserById(distributor.UserId) as DistributorUser;
                SetInnerObject(distributor, owner);
            }
        }
    }
}