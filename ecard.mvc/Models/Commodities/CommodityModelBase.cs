using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Commodities
{
    public class CommodityModelBase : ViewModelBase
    {
        private Commodity _innerObject;

        public CommodityModelBase()
        {
            _innerObject = new Commodity();
        }

        public CommodityModelBase(Commodity shop)
        {
            _innerObject = shop;
        }

        [NoRender]
        public Commodity InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(Commodity item)
        {
            _innerObject = item;
        }
        [Required(ErrorMessage= "请输入显示名称")]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }

        [Required(ErrorMessage = "请输入名称")]
        public string Name
        {
            get { return InnerObject.Name; }
            set { InnerObject.Name = value; }
        }

        public decimal Price
        {
            get { return InnerObject.Price; }
            set { InnerObject.Price = value; }
        }

        [Dependency]
        [NoRender]
        public ICommodityService CommodityService { get; set; }


    }
}
