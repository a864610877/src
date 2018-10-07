using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IUserCouponsService
    {
        UserCoupons GetById(int id);

        void Update(UserCoupons item);

        //DataTables<UserCoupons> Query(CouponsRequest request);

        void Create(UserCoupons item);

        List<UserCouponss> GetUserId(int userId);
    }
}
