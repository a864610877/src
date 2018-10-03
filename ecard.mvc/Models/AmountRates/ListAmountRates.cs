using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.AmountRates
{
    public class ListAmountRates : EcardModelListRequest<ListAmountRate>
    {
        public ListAmountRates()
        {
            OrderBy = "AmountRateId";
        }


        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }
        [Dependency]
        [NoRender]
        public IAmountRateService AmountRateService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Ready()
        {
        }

        public void Delete(int id)
        {
            var item = this.AmountRateService.GetById(id);
            if (item != null)
            {
                AmountRateService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.AmountRateDelete, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("delete.success", item.DisplayName);
                CacheService.Refresh(CacheKeys.AmountRateKey);
            }
        }

        public void Query()
        {
            var query = AmountRateService.Query();
            if (State != AmountRateStates.All)
                query = query.Where(x => x.State == State);
            this.List = query.ToList(this, x => new ListAmountRate(x));
        }

        public void Suspend(int id)
        {
            var item = this.AmountRateService.GetById(id);
            if (item != null && item.State == AmountRateStates.Normal)
            {
                item.State = AmountRateStates.Invalid;
                AmountRateService.Update(item);

                Logger.LogWithSerialNo(LogTypes.AmountRateSuspend, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("suspend.success", item.DisplayName);
                CacheService.Refresh(CacheKeys.AmountRateKey);
            }
        }

        public void Resume(int id)
        {
            var item = this.AmountRateService.GetById(id);
            if (item != null && item.State == AmountRateStates.Invalid)
            {
                item.State = AmountRateStates.Normal;
                AmountRateService.Update(item);

                Logger.LogWithSerialNo(LogTypes.AmountRateResume, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("resume.success", item.DisplayName);
                CacheService.Refresh(CacheKeys.AmountRateKey);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListAmountRate item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.AmountRateId });
            if (item.InnerObject.State == AmountRateStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.AmountRateId });
            if (item.InnerObject.State == AmountRateStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.AmountRateId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.AmountRateId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<AmountRate>("State", UserStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}