using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Microsoft.Practices.Unity;
using Ecard.Services;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class OrderModelBase:ViewModelBase
    {
         private Order1 _innerObject;

         public OrderModelBase()
        {
            _innerObject = new Order1();
        }

         public OrderModelBase(Order1 order)
        {
            _innerObject = order;
        }
        [NoRender]
        public Order1 InnerObject
        {
            get { return _innerObject; }
        }
        protected void SetInnerObject(Order1 item)
        {
            _innerObject = item;
        }

        [Required(AllowEmptyStrings=false,ErrorMessage="地址必须填写")]
        public string Address
        { get { return InnerObject.Address; }
            set { InnerObject.Address = value; }
        }
        [StringLength(50,ErrorMessage="备注不能超过50个字符")]
        public string OrderDemo
        {
            get { return InnerObject.Demo; }
            set { InnerObject.Demo = value; }
        }

        [NoRender]
        public List<OrderDetial1> Detials { get; set; }
        [Dependency, NoRender]
        public IOrder1Service OrderService { get; set; }
        [Dependency, NoRender]
        public ICommodityService CommodityService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        protected void OnSave(Order1 order)
        {
            //order.AccountId = AccountId;
            order.Demo = OrderDemo;
            order.Address = this.InnerObject.Address;
            order.SubmitTime = DateTime.Now;
            order.TotalMoney = Detials.Sum(x => x.price * x.Amount);
        }
        protected void AddDetial(OrderDetial1 item)
        {
            Detials.Add(item);
        }
    }
}
