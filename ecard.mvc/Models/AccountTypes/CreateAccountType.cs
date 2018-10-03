using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AccountTypes
{
    public class CreateAccountType : AccountTypeModelBase, IValidator
    {
        public CreateAccountType()
        {
            InnerObject.State = AccountTypeStates.Normal;
        }
        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }
        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            InnerObject.State = AccountTypeStates.Normal;
            AccountTypeService.Create(InnerObject);

            AddMessage("success", DisplayName);
            Logger.LogWithSerialNo(LogTypes.AccountTypeCreate, serialNo, InnerObject.AccountTypeId, DisplayName);
            CacheService.Refresh(CacheKeys.PointPolicyKey);
            return this;
        } 

        public IEnumerable<ValidationError> Validate()
        {
            if (AccountTypeService.Query(new AccountTypeRequest() { DisplayName = DisplayName }).Count() > 0)
                yield return new ValidationError("DisplayName", string.Format(Localize("messages.duplicationDisplayName"), DisplayName));
        }
    }
}