using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Couponss
{
    public class CouponModelBase : ViewModelBase
    {
        private Coupons _innerObject;
        public CouponModelBase()
        {
            _innerObject = new Coupons();
        }
        public CouponModelBase(Coupons admissionTicket)
        {
            _innerObject = admissionTicket;
        }
        [NoRender]
        public Coupons InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(Coupons item)
        {
            _innerObject = item;
        }

        public void Ready()
        {
            var query = (from x in ShopService.Query(new ShopRequest() { IsBuildIn = false })
                         select new IdNamePair { Key = x.ShopId, Name = x.DisplayName }).ToList();
           // query.Insert(0, new IdNamePair { Key = 0, Name = "所有商户" });
            this.Shop.Bind(query, true);
        }

        [NoRender, Dependency]
        public ICouponsService CouponsService { get; set; }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        
        private Bounded _couponTypeBounded;
        public Bounded CouponType
        {
            get
            {
                if (_couponTypeBounded == null)
                {
                    _couponTypeBounded = Bounded.Create<Coupons>("couponsType", CouponsType.DiscountedVolume);
                }
                return _couponTypeBounded;
            }
            set { _couponTypeBounded = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "请输入门票名称")]
        [StringLength(100)]
        public string name { get { return InnerObject.name; } set { InnerObject.name = value; } }

        private Bounded _shopBounded;
        public Bounded Shop
        {
            get
            {
                if (_shopBounded == null)
                {
                    _shopBounded = Bounded.CreateEmpty("ShopId", Globals.All);
                }
                return _shopBounded;
            }
            set { _shopBounded = value; }
        }
        /// <summary>
        /// 折扣 类型为1使用
        /// </summary>
        public decimal discount { get { return InnerObject.discount; } set { InnerObject.discount = value; } }
        /// <summary>
        /// 抵扣金额 类型为2使用
        /// </summary>
        public decimal deductibleAmount { get { return InnerObject.deductibleAmount; } set { InnerObject.deductibleAmount = value; } }
        /// <summary>
        /// 满减金额 满金额，当支付金额达到满金额时可减 减金额额
        /// </summary>
       public decimal fullAmount { get { return InnerObject.fullAmount; } set { InnerObject.fullAmount = value; } }
        /// <summary>
        /// 满减金额 减金额， 当支付金额达到满金额时可减 减金额额
        /// </summary>
        public decimal reduceAmount { get { return InnerObject.reduceAmount; } set { InnerObject.reduceAmount = value; } }
        /// <summary>
        /// 发放数量，0：所有会员在领卷有效期内都可以领取一张；大于0：每个会员限领一张，领完为止
        /// </summary>
        [RegularExpression(@"\d{1,7}(\.\d{2})?", ErrorMessage = "输入的金额有误")]
       
        public int quantity { get { return InnerObject.quantity; } set { InnerObject.quantity = value; } }
        /// <summary>
        /// 有效期 超过此有效期，不可使用
        /// </summary>
        public DateTime? validity { get { return InnerObject.validity; } set { InnerObject.validity = value; } }
       
    }
}
