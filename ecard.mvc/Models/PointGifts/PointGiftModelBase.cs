using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.AccountLevelPolicies;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PointGifts
{
    public class PointGiftModelBase : ViewModelBase
    {
        private PointGift _innerObject;

        public PointGiftModelBase()
        {
            _innerObject = new PointGift();
        }

        [MultiCheckList,NoRender]
        public MultiCheckList<ListAccountLevelPolicy> Levels { get; set; }

        [NoRender]
        public PointGift InnerObject
        {
            get { return _innerObject; }
        }

        public Picture Photo { get; set; }
        [Required(ErrorMessage = "«Î ‰»Î¿Ò∆∑√Ë ˆ")]
        public string Description
        {
            get { return InnerObject.Description; }
            set { InnerObject.Description = value; }
        }
        public int Priority
        {
            get { return InnerObject.Priority; }
            set { InnerObject.Priority = value; }
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
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(128)]
        public string Category
        {
            get { return InnerObject.Category; }
            set { InnerObject.Category = value; }
        }


        [Dependency, NoRender]
        public IPointGiftService PointGiftService { get; set; }

        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Ready()
        {
            Photo = new Picture("~/content/pointgiftphotos/{0}", InnerObject.Photo, 120);
            List<AccountLevelPolicy> accountLevels = AccountLevelPolicyService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal).ToList();
            Levels = new MultiCheckList<ListAccountLevelPolicy>(accountLevels.Where(x => InnerObject.IncludeLevel(x.AccountLevelPolicyId)).Select(x => new ListAccountLevelPolicy(x)));
            Levels.Merge(accountLevels.Select(x => new ListAccountLevelPolicy(x)));
        }

        protected void SetInnerObject(PointGift item)
        {
            _innerObject = item;
        }

        protected void OnSave(PointGift item)
        {
            this.AccountDependency.Save(item);
            item.AccountLevels = Levels == null ? "" : string.Join(",", this.Levels.GetCheckedIds().Select(x => x.ToString()).ToArray());
            item.Priority = Priority;
        }
    }
}