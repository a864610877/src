using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.PosEndPointes
{
    public class ListPosEndPoint
    {
        private readonly PosEndPoint _innerObject;
        internal Shop Shop { get; set; }
        [NoRender]
        public PosEndPoint InnerObject
        {
            get { return _innerObject; }
        }

        public ListPosEndPoint()
        {
            _innerObject = new PosEndPoint();
        }

        public ListPosEndPoint(PosEndPoint posEndPoint)
        {
            _innerObject = posEndPoint;
        }
        [NoRender]
        public int PosEndPointId
        {
            get { return InnerObject.PosEndPointId; }
            set { InnerObject.PosEndPointId = value; }
        }
        
        public string Name
        {
            get { return InnerObject.Name ; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        [CheckPermission(Ecard.Permissions.PosDataKey)]
        public string DataKey
        {
            get { return InnerObject.DataKey; }
        }
        [Sort("Shop.DisplayName")]
        public string ShopDisplayName
        {
            get { return Shop.DisplayName; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
        [NoRender]
        public string boor { get; set; }
    }
    
}