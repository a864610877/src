using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.AccountLevelPolicies
{
    [Bind(Prefix = "Item")]
    public class EditAccountLevelPolicy : AccountLevelPolicyModelBase
    {
        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }
        public EditAccountLevelPolicy()
        {
        }
        public EditAccountLevelPolicy(AccountLevelPolicy item)
            : base(item)
        {
        }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }
        [Hidden]
        public int AccountLevelPolicyId
        {
            get { return InnerObject.AccountLevelPolicyId; }
            set { InnerObject.AccountLevelPolicyId = value; }
        }

        public void Read(int id)
        {
            this.SetInnerObject(AccountLevelPolicyService.GetById(id));
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = AccountLevelPolicyService.GetById(AccountLevelPolicyId);
            if (item != null)
            {
                item.DisplayName = DisplayName;
                item.TotalPointStart = TotalPointStart;
                OnSave(item);
                AccountLevelPolicyService.Update(item);
                AddMessage("success", item.Level, DisplayName);
                Logger.LogWithSerialNo(LogTypes.AccountLevelEdit,serialNo, item.Level, DisplayName);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }

        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        public void Ready()
        {
            var accountType = AccountTypeService.GetById(InnerObject.AccountTypeId);
            if (accountType != null)
                AccountTypeName = accountType.DisplayName;
        }

        public string AccountTypeName { get; private set; }
    }
}