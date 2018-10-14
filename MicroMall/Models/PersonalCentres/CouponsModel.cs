using Ecard.Models;
using MicroMall.Models.Parentings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.PersonalCentres
{
    public class CouponsModel:UseCoupons
    {
        public CouponsModel(UserCouponss item) : base(item) { }
        public CouponsModel(Couponss item) : base(item) { }

    }
}