﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Microsoft.Practices.Unity;
using Ecard.Services;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class OrderDetialModelBase : ViewModelBase
    {
         private OrderDetial1 _innerObject;
         public OrderDetialModelBase()
        {
            _innerObject = new OrderDetial1();
        }

         public OrderDetialModelBase(string orderId)
        {
            _innerObject = new OrderDetial1() {  OrderId=orderId};
        }
         public OrderDetialModelBase(OrderDetial1 item)
         {
             _innerObject = item;
         }
        [NoRender]
        public OrderDetial1 InnerObject
        {
            get { return _innerObject; }
        }
        protected void SetInnerObject(OrderDetial1 item)
        {
            _innerObject = item;
        }

        [Dependency, NoRender]
        public IOrder1Service OrderService { get; set; }
        [Dependency,NoRender]
        public ICommodityService CommodityService { get; set; }

        protected void OnSave(OrderDetial1 detial)
        {
            detial.GoodId = GoodBounded;
            detial.price = this.InnerObject.price;
            detial.OrderId = this.InnerObject.OrderId;
            detial.Amount = this.InnerObject.Amount;
        }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "请输入商品名称")]
        private Bounded _goodBounded;
        public Bounded GoodBounded 
        {
            get
            {
                if (_goodBounded == null)
                {
                    _goodBounded = Bounded.CreateEmpty("GoodId", InnerObject.GoodId);
                }
                return _goodBounded;
            }
            set { _goodBounded = value; }
        }
    }

    public class OrderDetialBase:OrderDetial1
    {
        public OrderDetialBase(OrderDetial1 item)
        {
            this.Amount = item.Amount;
            this.GoodId = item.GoodId;
            this.OrderId = item.OrderId;
            this.price = item.price;
            this.Serialnumber = item.Serialnumber;
        }
        public string GoodName { get; set; }
        public string DisplayName { get; set; }
        public decimal Total { get { return Amount * price; } }
    }
}
