using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.GoodandOrder
{
     public class GoodModelBase: ViewModelBase
    {
         private Good _innerObject;
         public GoodModelBase()
        {
            _innerObject = new Good();
        }

        public GoodModelBase(Good good)
        {
            _innerObject = good;
        }
        [NoRender]
        public Good InnerObject
        {
            get { return _innerObject; }
        }
        protected void SetInnerObject(Good item)
        {
            _innerObject = item;
        }

        [Dependency, NoRender]
        public IOrder1Service OrderService { get; set; }

        protected void OnSave(Good good)
        {
            good.GoodName = GoodName;
            good.State = State;
        }
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入商品名称")]
        public string GoodName 
        {
            get { return InnerObject.GoodName; }
            set{InnerObject.GoodName=value;}
        }
        public Bounded _stateBounded;
        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.CreateEmpty("GoodId", InnerObject.GoodId);
                }
                return _stateBounded;
            }
            set { _stateBounded = value; }
        }
    }
}
