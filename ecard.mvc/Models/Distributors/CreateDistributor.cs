using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.Distributors
{
    public class CreateDistributor : DistributorModelBase, IValidator
    {
        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }
         

        [Required(ErrorMessage = "请输入电子邮件")]
        [StringLength(50)]
        [RegularExpression(@"^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$", ErrorMessage = "邮箱的格式不对")]
        public string Email
        {
            get { return Owner.Email; }
            set { Owner.Email = value.TrimSafty(); }
        }

        [Required(ErrorMessage = "请输入登录名")]
        [StringLength(20)]
        public string UserName
        {
            get { return Owner.Name; }
            set { Owner.Name = value.TrimSafty(); }
        }

        [NoRender, Dependency]
        public Site Site { get; set; } 

        #region IValidator Members

        public IEnumerable<ValidationError> Validate()
        { 
            if (MembershipService.GetUserByName(UserName) != null)
                yield return new ValidationError("UserName", string.Format(Localize("messages.duplicationUser"), UserName));
            if (!string.IsNullOrWhiteSpace(Mobile.Value1) && MembershipService.GetUserByMobile<DistributorUser>(this.Mobile.Value1, 0).Count() > 0)
                yield return new ValidationError("Mobile", string.Format(Localize("messages.duplicationMobile"), Mobile.Value1)); 
        }

        #endregion

        protected override void OnDisplayNameChanged(string value)
        {
            Owner.DisplayName = value;
            base.OnDisplayNameChanged(value);
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            var roles = MembershipService.QueryRoles(new RoleRequest { Name = RoleNames.DistributorOwner }).ToList();

            TransactionHelper.BeginTransaction();
            InnerObject.DistributorLevel = DistributorLevel;
            OnSave(InnerObject, Owner);
            var user = this.SecurityHelper.GetCurrentUser();
            if (user is AdminUserModel)
                InnerObject.ParentId = 0;
            else if (user is DistributorUserModel)
            {
                InnerObject.ParentId = ((DistributorUserModel)user).DistributorId;
            }
            Owner.SetPassword(Password);
            InnerObject.State = States.Normal;//经销商状态

            Owner.State = States.Normal;
            MembershipService.CreateUser(Owner);
            InnerObject.UserId = Owner.UserId;
            DistributorService.Create(InnerObject);
            UpdateAccountLevelPolicy(InnerObject);
            MembershipService.AssignRoles(Owner, roles.Select(x => x.RoleId).ToArray());
            DistributorDealLog log = new DistributorDealLog(serialNo, DealTypes.Open, 0, null, null, null, InnerObject, 0);
            //DistributorDealLogService.Create(log);
            AddMessage("success", this.UserName);
            Logger.LogWithSerialNo(LogTypes.DistributorCreate, serialNo, InnerObject.DistributorId, this.UserName);
            CacheService.Refresh(CacheKeys.PosKey);
            return TransactionHelper.CommitAndReturn(this);
        }
    }
}