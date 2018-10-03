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

namespace Ecard.Mvc.Models.PointPolicies
{
    public class ListPointPolicies : EcardModelListRequest<ListPointPolicy>
    {
        public ListPointPolicies()
        {
            OrderBy = "PointPolicyId";
        }

        [Dependency]
        [NoRender]
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

        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPointPolicy item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.PointPolicyId });
            if (item.InnerObject.State == PointPolicyStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.PointPolicyId });
            if (item.InnerObject.State == PointPolicyStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.PointPolicyId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.PointPolicyId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<PointPolicy>("State", PointPolicyStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
        [Dependency, NoRender]
        public IPointPolicyService PointPolicyService { get; set; }
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        public void Query()
        {
            var query = this.PointPolicyService.Query();
            if (State != States.All)
                query = query.Where(x => x.State == State);
            if (!string.IsNullOrWhiteSpace(DisplayName))
                query = query.Where(x => x.DisplayName.ToLower().Contains(DisplayName.ToLower()));

            List = query.ToList(this, x => new ListPointPolicy(x));
            var levels = AccountLevelPolicyService.Query().ToList();

            foreach (var pointPolicy in List)
            {
                var ids = (pointPolicy.InnerObject.AccountLevels ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] names = levels.Where(x => ids.Contains(x.AccountLevelPolicyId.ToString())).Select(x => x.DisplayName).ToArray();
                pointPolicy.AccountLevelName = string.Join(",", names);
            }
        }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Suspend(int id)
        {
            var item = PointPolicyService.GetById(id);
            if (item != null && item.State == PointPolicyStates.Normal)
            {
                item.State = PointPolicyStates.Invalid;
                PointPolicyService.Update(item);

                AddMessage("suspend.success", item.DisplayName, item.PointPolicyId);
                Logger.LogWithSerialNo(LogTypes.PointPolicySuspend, SerialNoHelper.Create(), id, item.DisplayName, item.PointPolicyId);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }
        public void Resume(int id)
        {
            var item = PointPolicyService.GetById(id);
            if (item != null && item.State == PointPolicyStates.Invalid)
            {
                if (PointPolicyService.Query().Any(x => x.IncludeLevel(item.PointPolicyId) && item.State == PointPolicyStates.Normal))
                {
                    AddError(Localize("resume.fail"), item.DisplayName, item.PointPolicyId);
                }
                else
                {
                    item.State = PointPolicyStates.Normal;
                    PointPolicyService.Update(item);
                    AddMessage("resume.success", item.DisplayName, item.PointPolicyId);
                    Logger.LogWithSerialNo(LogTypes.PointPolicyResume, SerialNoHelper.Create(), id, item.DisplayName, item.PointPolicyId);
                    CacheService.Refresh(CacheKeys.PointPolicyKey);
                }
            }
        }

        public void Delete(int id)
        {
            var item = PointPolicyService.GetById(id);
            if (item != null)
            {
                PointPolicyService.Delete(item);
                AddMessage("delete.success", item.DisplayName, item.PointPolicyId);
                Logger.LogWithSerialNo(LogTypes.PointPolicyDelete, SerialNoHelper.Create(), id, item.DisplayName, item.PointPolicyId);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }
    }
}