using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.AccountLevelPolicies
{
    public class CreateAccountLevelPolicy : AccountLevelPolicyModelBase, IValidator
    {
        private Bounded _accountTypeBounded;

        public Bounded AccountType
        {
            get
            {
                if (_accountTypeBounded == null)
                {
                    _accountTypeBounded = Bounded.CreateEmpty("AccountTypeId", 0);
                }
                return _accountTypeBounded;
            }
            set { _accountTypeBounded = value; }
        }
        public CreateAccountLevelPolicy()
        {
            InnerObject.State = AccountLevelPolicyStates.Normal;
            InnerObject.DiscountRate = 1;
        }

        public CreateAccountLevelPolicy(AccountLevelPolicy item)
            : base(item)
        {
            InnerObject.State = AccountLevelPolicyStates.Normal;
        }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public IMessageProvider Create()
        {
            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            InnerObject.State = AccountLevelPolicyStates.Normal;
            InnerObject.AccountTypeId = this.AccountType;
            OnSave(InnerObject);
            AccountLevelPolicyService.Create(InnerObject);
            AddMessage("success", Level, DisplayName);
            Logger.LogWithSerialNo(LogTypes.AccountLevelCreate, serialNo, Level, DisplayName);

            CacheService.Refresh(CacheKeys.PointPolicyKey);
            return TransactionHelper.CommitAndReturn(this);
        }
        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }

        public int Level
        {
            get { return InnerObject.Level; }
            set { InnerObject.Level = value; }
        }

        public IEnumerable<ValidationError> Validate()
        {
            if (AccountLevelPolicyService.Query().Any(x => x.Level == Level && x.State == AccountLevelPolicyStates.Normal && x.AccountTypeId == this.AccountType))
                yield return new ValidationError("Level", string.Format(Localize("messages.duplicationLevel"), Level));
        }

        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        public void Ready()
        {
            var accountTypes = AccountTypeService.Query(new AccountTypeRequest() { State = AccountTypeStates.Normal }).Select(x => new IdNamePair() { Key = x.AccountTypeId, Name = x.DisplayName });
            this.AccountType.Bind(accountTypes);
        }
    }
}