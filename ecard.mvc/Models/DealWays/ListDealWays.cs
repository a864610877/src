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

namespace Ecard.Mvc.Models.DealWays
{
    public class ListDealWays : EcardModelListRequest<ListDealWay>
    {
        public ListDealWays()
        {
            OrderBy = "DealWayId";
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName.TrimSafty(); }
            set { _displayName = value; }
        }

        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }

        public void Ready()
        {
        }

        public void Delete(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = DealWayService.GetById(id);
            if (item != null)
            {
                DealWayService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.DealWayDelete, serialNo, item.DealWayId, item.DisplayName);
                AddMessage("delete.success", item.DisplayName, item.DealWayId, item.DisplayName);
            }
        }

        public void Query()
        {
            var query = this.DealWayService.Query();
            if (!string.IsNullOrWhiteSpace(DisplayName))
                query = query.Where(x => x.DisplayName.EqualsIgnoreCase(DisplayName));
            if (this.State != States.All)
                query = query.Where(x => x.State == this.State);
            // fill condition
            List = query.ToList(this, x => new ListDealWay(x));
        }

        public void Suspend(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.DealWayService.GetById(id);
            if (item != null && item.State == DealWayStates.Normal)
            {
                item.State = DealWayStates.Invalid;
                DealWayService.Update(item);
                Logger.LogWithSerialNo(LogTypes.DealWaySuspend, serialNo, item.DealWayId, item.DisplayName);
                AddMessage("suspend.success", item.DisplayName);
            }
        }

        public void Resume(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.DealWayService.GetById(id);
            if (item != null && item.State == DealWayStates.Invalid)
            {
                item.State = DealWayStates.Normal;
                DealWayService.Update(item);
                Logger.LogWithSerialNo(LogTypes.DealWayResume, serialNo, item.DealWayId, item.DisplayName);
                AddMessage("resume.success", item.DisplayName);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListDealWay item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.DealWayId });
            if (item.InnerObject.State == DealWayStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.DealWayId });
            if (item.InnerObject.State == DealWayStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.DealWayId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.DealWayId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<DealWay>("State", DealWayStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}
