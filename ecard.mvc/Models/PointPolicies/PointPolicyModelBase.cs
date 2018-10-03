using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.Models.AccountLevelPolicies;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.PointPolicies
{
    public class PointPolicyModelBase : ViewModelBase
    {
        private PointPolicy _innerObject;

        public PointPolicyModelBase()
        {
            _innerObject = new PointPolicy();
        }

        public PointPolicyModelBase(PointPolicy shop)
        {
            _innerObject = shop;
        }

        [MultiCheckList, NoRender]
        public MultiCheckList<ListAccountLevelPolicy> Levels { get; set; }
        private AccountDependency _accountDependency;
        [NoRender]
        public AccountDependency AccountDependency
        {
            get
            {
                if (_accountDependency == null)
                {
                    _accountDependency = new AccountDependency(InnerObject);
                }
                return _accountDependency;
            }
            set { _accountDependency = value; }
        }

        [NoRender]
        public PointPolicy InnerObject
        {
            get { return _innerObject; }
        }
        protected void SetInnerObject(PointPolicy item)
        {
            _innerObject = item;
        }
        [RegularExpression(@"(?:^(?:[1-9][\d]?)(?:\.[\d]{1,2})?$)", ErrorMessage = "积分值有误")]
        public decimal Point
        {
            get { return InnerObject.Point; }
            set { InnerObject.Point = value; }
        }
        public int Priority
        {
            get { return InnerObject.Priority; }
            set { InnerObject.Priority = value; }
        }

        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        protected void OnSave(PointPolicy item)
        {
            this.AccountDependency.Save(item);
            item.AccountLevels = Levels == null ? "" : string.Join(",", this.Levels.GetCheckedIds().Select(x => x.ToString()).ToArray());
            item.Priority = Priority;
        }
        protected void OnReady()
        {
            var accountLevels = AccountLevelPolicyService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal).Select(x => new ListAccountLevelPolicy(x)).ToList();
            Levels = new MultiCheckList<ListAccountLevelPolicy>(accountLevels.Where(x => InnerObject.IncludeLevel(x.AccountLevelPolicyId)));
            Levels.Merge(accountLevels);
        }
    }
}