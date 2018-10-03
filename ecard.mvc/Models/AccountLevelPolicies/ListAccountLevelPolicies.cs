using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.AccountLevelPolicies
{
    public class ListAccountLevelPolicies : EcardModelListRequest<ListAccountLevelPolicy>
    {
        public ListAccountLevelPolicies()
        {
            OrderBy = "AccountLevelPolicyId";
        }

        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName.TrimSafty(); }
            set { _displayName = value; }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };

        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListAccountLevelPolicy item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.AccountLevelPolicyId });
            if (item.InnerObject.State == AccountLevelPolicyStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.AccountLevelPolicyId });
            if (item.InnerObject.State == AccountLevelPolicyStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.AccountLevelPolicyId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.AccountLevelPolicyId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<AccountLevelPolicy>("State", AccountLevelPolicyStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        public void Query()
        {
            var query = this.AccountLevelPolicyService.Query();

            if (!string.IsNullOrWhiteSpace(DisplayName))
                query = query.Where(x => x.DisplayName.ToLower().Contains(DisplayName.ToLower()));
            if (State != AccountLevelPolicyStates.All)
                query = query.Where(x => x.State == State);
            List = query.ToList(this, x => new ListAccountLevelPolicy(x));

            var accountTypes = AccountTypeService.Query(new AccountTypeRequest()).ToList();

            List.Merge(accountTypes,
                       (a, b) => a.InnerObject.AccountTypeId == b.AccountTypeId,
                       (a, b) => a.AccountTypeName = b.Any() ? b.FirstOrDefault().DisplayName : ""
                );
        }

        public void Suspend(int id)
        {
            var item = AccountLevelPolicyService.GetById(id);
            if (item != null && item.State == AccountLevelPolicyStates.Normal)
            {
                item.State = AccountLevelPolicyStates.Invalid;
                AccountLevelPolicyService.Update(item);
                Logger.LogWithSerialNo(LogTypes.AccountLevelSuspend, SerialNoHelper.Create(), item.Level, item.DisplayName);
                AddMessage("suspend.success", item.Level, item.DisplayName);
            }
        }

        public void Resume(int id)
        {
            var item = AccountLevelPolicyService.GetById(id);
            if (item != null && item.State == AccountLevelPolicyStates.Invalid)
            {
                if (AccountLevelPolicyService.Query().Any(x => x.State == AccountLevelPolicyStates.Normal && x.Level == item.Level && x.AccountTypeId == item.AccountTypeId))
                {
                    AddError("resume.fail", item.Level, item.DisplayName);
                }
                else
                {
                    item.State = AccountLevelPolicyStates.Normal;
                    AccountLevelPolicyService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.AccountLevelResume, SerialNoHelper.Create(), item.Level, item.DisplayName);
                    AddMessage("resume.success", item.Level, item.DisplayName);
                }
            }
        }

        protected int? Level { get; set; }

        public void Delete(int id)
        {
            var item = AccountLevelPolicyService.GetById(id);
            if (item != null)
            {
                AccountLevelPolicyService.Delete(item);
                Logger.LogWithSerialNo(LogTypes.AccountLevelDelete, SerialNoHelper.Create(), item.Level, item.DisplayName);
                AddMessage("delete.success", item.Level, item.DisplayName);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }
    }
}