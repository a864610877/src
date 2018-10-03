using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealWays
{
    public class DealWayModelBase : ViewModelBase
    {
        private DealWay _innerObject;

        public DealWayModelBase()
        {
            _innerObject = new DealWay();
            ApplyTo = new ApplyToModel();
        }
        [Required(ErrorMessage = "«Î ‰»Î÷ß∏∂√˚≥∆")]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }


        public DealWayModelBase(DealWay shop)
        {
            _innerObject = shop;
        }

        public bool IsCash
        {
            get { return InnerObject.IsCash; }
            set { InnerObject.IsCash = value; }
        }
        [NoRender]
        public DealWay InnerObject
        {
            get { return _innerObject; }
        }
        protected void SetInnerObject(DealWay item)
        {
            _innerObject = item;
            ApplyTo = new ApplyToModel(item.ApplyTo);
        }

        public ApplyToModel ApplyTo { get; set; }
        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }

        protected void OnSave(DealWay dealWay)
        {
            dealWay.ApplyTo = ApplyTo.GetValue();
            dealWay.DisplayName = DisplayName;
            dealWay.IsCash = IsCash;
        }
    }
}