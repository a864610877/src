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

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class ListGoods : EcardModelListRequest<ListGood>
    {
        public ListGoods()
        {
            OrderBy = "GoodId";
        }

        [Dependency, NoRender]
        public IOrder1Service OrderService { get; set; }

       private string _goodName;
        public string GoodName
        {
            get { return _goodName.TrimSafty(); }
            set { _goodName = value; }
        }
        public void Ready()
        {
        }

        public void Delete(int id)
        {
            int[] ids= {id};
            var serialNo = SerialNoHelper.Create();
            var item = OrderService.GetGoodsByIds(ids).FirstOrDefault();
            if (item != null)
            {
                OrderService.DeleteGood(item);

                Logger.LogWithSerialNo(LogTypes.DeleteGood, serialNo, item.GoodId, item.GoodName);
                AddMessage("delete.success", serialNo, item.GoodId, item.GoodName);
            }
        }

        public void Query()
        {
            var query = this.OrderService.QueryGood(new GoodRequest()).ToList();
            if (!string.IsNullOrWhiteSpace(GoodName))
                query =query.Where(x=>x.GoodName.EqualsIgnoreCase(GoodName)).ToList();
            if (this.State != States.All)
                query = query.Where(x => x.State == this.State).ToList();
            // fill condition
            List = query.ToList(this, x => new ListGood(x));
        }

        public void Suspend(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.GetById(id);
            if (item != null && item.State == GoodState.Normal)
            {
                item.State = GoodState.Invalid;
                OrderService.UpdateGood(item);
                Logger.LogWithSerialNo(LogTypes.EditGood, serialNo, item.GoodId, item.GoodName);
                AddMessage("suspend.success", item.GoodName);
            }
        }

        public void Resume(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.GetById(id);
            if (item != null && item.State == GoodState.Invalid)
            {
                item.State = GoodState.Normal;
                OrderService.UpdateGood(item);
                Logger.LogWithSerialNo(LogTypes.EditGood, serialNo, item.GoodId, item.GoodName);
                AddMessage("resume.success", item.GoodName);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListGood item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.GoodId });
            if (item.InnerObject.State == DealWayStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.GoodId });
            if (item.InnerObject.State == DealWayStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.GoodId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.GoodId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Good>("State", GoodState.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
    }
