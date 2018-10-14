using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface ICouponsService
    {
        Coupons GetById(int id);

        void Update(Coupons item);

        DataTables<Coupons> Query(CouponsRequest request);

        void Create(Coupons item);
        void Delete(Coupons item);
        /// <summary>
        /// 找出用户可以领取的优惠卷
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Couponss> GetByUserCoupon(int userId);
    }
}
