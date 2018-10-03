using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.PointRebates
{
    public class ListPointRebates : EcardModelListRequest<ListPointRebate>
    {
        public ListPointRebates()
        {
            OrderBy = "PointRebateId";
        }

        //private string _name;
        //public string Name
        //{
        //    get { return _name.TrimSafty(); }
        //    set { _name = value; }
        //}

        [Dependency]
        [NoRender]
        public IPointRebateService PointRebateService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Ready()
        {
            var query = from x in AccountLevelPolicyService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal)
                        orderby x.AccountTypeId, x.Level
                        select new IdNamePair { Key = x.AccountLevelPolicyId, Name = x.DisplayName };
            this.AccountLevel.Bind(query, true);
        }

        public void Query()
        {
            var query = PointRebateService.Query();
            if (State != States.All)
                query = query.Where(x => x.State == State);
            if (AccountLevel != Globals.All)
                query = query.Where(x => x.IncludeLevel(AccountLevel));
            this.List = query.ToList(this, x => new ListPointRebate(x));
        }

        private Bounded _accountLevelBounded;

        public Bounded AccountLevel
        {
            get
            {
                if (_accountLevelBounded == null)
                {
                    _accountLevelBounded = Bounded.CreateEmpty("AccountLevelId", Globals.All);
                }
                return _accountLevelBounded;
            }
            set { _accountLevelBounded = value; }
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPointRebate item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.PointRebateId });
            if (item.InnerObject.State == PointPolicyStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.PointRebateId });
            if (item.InnerObject.State == PointPolicyStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.PointRebateId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.PointRebateId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<PointRebate>("State", PointRebateStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }

        public void Delete(int id)
        {
            var item = this.PointRebateService.GetById(id);
            if (item != null)
            {
                PointRebateService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.PointRebateDelete, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("delete.success", item.DisplayName);
            }
        }

        public void Suspend(int id)
        {
            var item = this.PointRebateService.GetById(id);
            if (item != null && item.State == PointRebateStates.Normal)
            {
                item.State = PointRebateStates.Invalid;
                PointRebateService.Update(item);

                Logger.LogWithSerialNo(LogTypes.PointRebateSuspend, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("suspend.success", item.DisplayName);
            }
        }

        public void Resume(int id)
        {
            var item = this.PointRebateService.GetById(id);
            if (item != null && item.State == PointRebateStates.Invalid)
            {
                item.State = PointRebateStates.Normal;
                PointRebateService.Update(item);

                Logger.LogWithSerialNo(LogTypes.PointRebateResume, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("resume.success", item.DisplayName);
            }
        }
    }
}
