using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.AccountLevelPolicies;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.PointRebates
{
    public class PointRebateModelBase : ViewModelBase
    {
        private PointRebate _innerObject;

        public PointRebateModelBase()
        {
            _innerObject = new PointRebate();
        }

        [MultiCheckList, NoRender]
        public MultiCheckList<ListAccountLevelPolicy> Levels { get; set; }

        [NoRender]
        public PointRebate InnerObject
        {
            get { return _innerObject; }
        }
        [Range(0,double.MaxValue)]
        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }

        [Range(1d, double.MaxValue)]
        public int Point
        {
            get { return InnerObject.Point; }
            set { InnerObject.Point = value; }
        }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(40)]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }
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
        [Dependency, NoRender]
        public IPointRebateService PointRebateService { get; set; }

        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Ready()
        {
            List<AccountLevelPolicy> accountLevels = AccountLevelPolicyService.Query().Where(x => x.State == States.Normal).ToList();
            Levels = new MultiCheckList<ListAccountLevelPolicy>(accountLevels.Where(x=>InnerObject.IncludeLevel(x.AccountLevelPolicyId)).Select(x => new ListAccountLevelPolicy(x)));
            Levels.Merge(accountLevels.Select(x => new ListAccountLevelPolicy(x)));
        }

        protected void SetInnerObject(PointRebate item)
        {
            _innerObject = item;
        }
        protected void OnSave(PointRebate item)
        {
            this.AccountDependency.Save(item);
            item.AccountLevels = Levels == null ? "" : string.Join(",", this.Levels.GetCheckedIds().Select(x => x.ToString()).ToArray());
        }
    }
}