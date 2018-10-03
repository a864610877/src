using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Couponss
{
    public class ListCoupon
    {
        private readonly Coupons _innerObject;

        [NoRender]
        public Coupons InnerObject
        {
            get { return _innerObject; }
        }

        public ListCoupon()
        {
            _innerObject = new Coupons();
        }

        public ListCoupon(Coupons account)
        {
            _innerObject = account;
        }

        [NoRender]
        public int id { get { return InnerObject.id; } }
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get { return InnerObject.code; } }
        /// <summary>
        /// 优惠卷类型 1折扣卷 2抵扣卷 3满减卷
        /// </summary>
        public string couponsType
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.couponsType); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get { return InnerObject.name; } }
        /// <summary>
        /// 使用范围 为空不限制使用，否则绑定商户号
        /// </summary>
        public string useScope { get { return InnerObject.useScope.Length <= 0 ? "不限" : InnerObject.useScope; } }
        /// <summary>
        /// 折扣 类型为1使用
        /// </summary>
        public decimal discount { get { return InnerObject.discount; } }
        /// <summary>
        /// 抵扣金额 类型为2使用
        /// </summary>
        public decimal deductibleAmount { get { return InnerObject.deductibleAmount; } }
        /// <summary>
        /// 满减金额 满金额，当支付金额达到满金额时可减 减金额额
        /// </summary>
        public decimal fullAmount { get { return InnerObject.fullAmount; } }
        /// <summary>
        /// 满减金额 减金额， 当支付金额达到满金额时可减 减金额额
        /// </summary>
        public decimal reduceAmount { get { return InnerObject.reduceAmount; } }
        /// <summary>
        /// 发放数量，0：所有会员在领卷有效期内都可以领取一张；大于0：每个会员限领一张，领完为止
        /// </summary>
        public int quantity { get { return InnerObject.quantity; } }
        /// <summary>
        /// 有效期 超过此有效期，不可使用
        /// </summary>
        public DateTime? validity { get { return InnerObject.validity; } }
        /// <summary>
        /// 已领数量
        /// </summary>
        public int leadersOfNum { get { return InnerObject.leadersOfNum; } }
        /// <summary>
        /// 状态
        /// </summary>
        public string state
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.state); }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string createOp { get { return InnerObject.createOp; } }
        public DateTime createTime { get { return InnerObject.createTime; } }

        [NoRender]
        public string boor { get; set; }
    }
}
