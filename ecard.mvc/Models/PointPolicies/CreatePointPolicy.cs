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

namespace Ecard.Mvc.Models.PointPolicies
{
    public class CreatePointPolicy : PointPolicyModelBase, IValidator
    {

        public CreatePointPolicy()
        {
            InnerObject.State = States.Normal;
        }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(40)]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }

        [Dependency]
        [NoRender]
        public IPointPolicyService PointPolicyService { get; set; }

        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }

        #region IValidator Members

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }

        #endregion

        public void Ready()
        {
            OnReady();
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            InnerObject.State = States.Normal;
            OnSave(InnerObject);
            PointPolicyService.Create(InnerObject);
            AddMessage("success", DisplayName, (int)InnerObject.PointPolicyId);
            Logger.LogWithSerialNo(LogTypes.PointPolicyCreate, serialNo, InnerObject.PointPolicyId, DisplayName, (int)InnerObject.PointPolicyId);
            CacheService.Refresh(CacheKeys.PointPolicyKey);
            return this;
        }
    }
}