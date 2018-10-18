using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.Parentings
{
    public class TicketResult
    {
        public List<ticketModel> ListTicket { get; set; }
        public BuyTickets buyTickets { get; set; }
        public List<UseCoupons> ListCoupons { get; set; }
    }

    public class BuyTickets
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public List<Ticketss> ListTickets{ get; set; }
    }

    public class ticketModel
    {
       public int admissionTicketId { get; set; }
       public string name { get; set; }
       public decimal price { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
       public string introduce { get; set; }
    }

    public class UseCoupons
    {
        public UseCoupons(Couponss item)
        {
            couponsType = CouponsType.GetName(item.couponsType);
            if (item.couponsType == CouponsType.DiscountedVolume)
            {
                describe = "折扣卷";
                amount = string.Format("折<span>{0}</span><br>", Convert.ToInt32(item.discount * 10).ToString());
            }
            else if (item.couponsType == CouponsType.FullVolumeReduction)
            {
                describe = string.Format("满{0}减{1}", item.fullAmount, item.reduceAmount);
                amount = string.Format("￥<span>{0}</span><br>", Convert.ToInt32(item.reduceAmount).ToString());
            }
            else if (item.couponsType == CouponsType.OffsetRoll)
            {
                describe = "抵扣卷";
                amount = string.Format("￥<span>{0}</span><br>", Convert.ToInt32(item.deductibleAmount).ToString());
            }
            else
                describe = "";
            if (item.validity.HasValue)
                validity = item.validity.Value.ToString("yyyy-MM-dd");
            else
                validity = "永久有效";
            useScope = string.IsNullOrWhiteSpace(item.useScope) ? "所有门店" : item.shopName;
            CouponsId = item.id;
        }
        public UseCoupons(UserCouponss item)
        {
            couponsType = CouponsType.GetName(item.couponsType);
            if (item.couponsType == CouponsType.DiscountedVolume)
            {
                describe = "折扣卷";
                amount = string.Format("折<span>{0}</span><br>", Convert.ToInt32(item.discount * 10).ToString());
            }
            else if (item.couponsType == CouponsType.FullVolumeReduction)
            {
                describe = string.Format("满{0}减{1}", item.fullAmount, item.reduceAmount);
                amount = string.Format("￥<span>{0}</span><br>", Convert.ToInt32(item.reduceAmount).ToString());
            }
            else if (item.couponsType == CouponsType.OffsetRoll)
            {
                describe = "抵扣卷";
                amount = string.Format("￥<span>{0}</span><br>", Convert.ToInt32(item.deductibleAmount).ToString());
            }
            else
                describe = "";
            if (item.validity.HasValue)
                validity = item.validity.Value.ToString("yyyy-MM-dd");
            else
                validity = "永久有效";
            useScope = string.IsNullOrWhiteSpace(item.useScope) ? "所有门店" : item.shopName;
            UserCouponsId = item.id;
        }

        public int CouponsId { get; set; }
        /// <summary>
        /// 优惠卷id
        /// </summary>
        public int UserCouponsId { get; set; }
        /// <summary>
        /// 优惠类型
        /// </summary>
        public string couponsType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string validity { get; set; }
        /// <summary>
        /// 优惠描述
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 使用门店
        /// </summary>
        public string useScope { get; set; }
        
    }
}