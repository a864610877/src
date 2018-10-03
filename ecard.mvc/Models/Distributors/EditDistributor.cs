using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.Distributors
{
    [Bind(Prefix = "Item")]
    public class EditDistributor : DistributorModelBase, IValidator
    {

        public string UserName
        {
            get { return Owner.Name; }
        }

        [Hidden]
        public int DistributorId
        {
            get { return InnerObject.DistributorId; }
            set { InnerObject.DistributorId = value; }
        }

        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }
        [Dependency, NoRender]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            var distributor = DistributorService.GetById(DistributorId);

            if (distributor != null)
            {
                var owner = MembershipService.GetUserById(distributor.UserId) as DistributorUser;

                if (owner == null)
                {
                    AddError(LogTypes.DistributorEdit, "noOwner");
                    return this;
                }

                owner.DisplayName = DisplayName;

                if (!string.IsNullOrEmpty(Password))
                {
                    owner.SetPassword(Password);
                }
                OnSave(distributor, owner);
               
                DistributorService.Update(distributor);
                UpdateAccountLevelPolicy(distributor);
                MembershipService.UpdateUser(owner);
                AddMessage("success", owner.Name);
                Logger.LogWithSerialNo(LogTypes.DistributorEdit, serialNo, distributor.DistributorId, owner.Name);
                CacheService.Refresh(CacheKeys.PosKey);
            }
            return this;
        }

        public IEnumerable<ValidationError> Validate()
        {
            var distributor = DistributorService.GetById(InnerObject.DistributorId) as Distributor;
            if (!string.IsNullOrWhiteSpace(Mobile.Value1) && MembershipService.GetUserByMobile<DistributorUser>(this.Mobile.Value1, distributor.UserId).Count() > 0)
                yield return new ValidationError("Mobile", string.Format(Localize("messages.duplicationMobile"), Mobile.Value1));
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