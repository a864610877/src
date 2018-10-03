using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealWays
{
    public class ViewDealWay : ViewObjectModelBase<DealWay>, ICommandProvider
    {
        [NoRender, Dependency]
        public IDealWayService DealWayService { get; set; }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }


        public void Read(int id)
        {
            DealWay obj = DealWayService.GetById(id);
            InnerObject = obj;
        }

        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            yield break;
            // if (this._innerObject.State == DealWayStates.Normal)
            //     yield return new ActionMethodDescriptor("Suspend", "DealWay", new { id = this._innerObject.DealWayId });
            // if (this._innerObject.State == DealWayStates.Invalid)
            //     yield return new ActionMethodDescriptor("Resume", "DealWay", new { id = this._innerObject.DealWayId });
        }

        public void Ready()
        {
             
        }
    }
     
}