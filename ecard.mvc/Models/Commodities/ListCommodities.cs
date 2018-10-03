using System.Collections.Generic;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Commodities
{
    public class ListCommodities : EcardModelListRequest<ListCommodity>
    {
        public ListCommodities()
        {
            OrderBy = "CommodityId";
        }

        //private string _name;
        //public string Name
        //{
        //    get { return _name.TrimSafty(); }
        //    set { _name = value; }
        //}
        
        [Dependency]
        [NoRender]
        public ICommodityService CommodityService { get; set; }

        public void Ready()
        {
        }

        public void Delete(int id)
        {
            var item = this.CommodityService.GetById(id);
            if (item != null)
            {
                CommodityService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.CommodityDelete, SerialNoHelper.Create(), item.CommodityId, item.Name);
                AddMessage("delete.success", item.Name);
            }
        }

        private string _name;

        public string Name
        {
            get { return _name.TrimSafty(); }
            set { _name = value; }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName.TrimSafty(); }
            set { _displayName = value; }
        }
        public void Query()
        {
            var request = new CommodityRequest();
            // fill condition
            if (State != States.All)
                request.State  = State;

            request.DisplayNameWith = DisplayName;
            request.NameWith = Name;
            var query = CommodityService.Query(request);
            this.List = query.ToList(this, x => new ListCommodity(x));
        }

        public void Suspend(int id)
        {
            var item = this.CommodityService.GetById(id);
            if (item != null && item.State == CommodityStates.Normal)
            {
                item.State = CommodityStates.Invalid;
                CommodityService.Update(item);
                Logger.LogWithSerialNo(LogTypes.CommoditySuspend, SerialNoHelper.Create(), id, item.Name);
                AddMessage("suspend.success", item.Name);
            }
        }

        public void Resume(int id)
        {
            var item = this.CommodityService.GetById(id);
            if (item != null && item.State == CommodityStates.Invalid)
            {
                item.State = CommodityStates.Normal;
                CommodityService.Update(item);
                Logger.LogWithSerialNo(LogTypes.CommodityResume, SerialNoHelper.Create(), id, item.Name);
                AddMessage("resume.success", item.Name);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListCommodity item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.CommodityId });
            if (item.InnerObject.State == CommodityStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.CommodityId });
            if (item.InnerObject.State == CommodityStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.CommodityId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.CommodityId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Commodity>("State", CommodityStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}