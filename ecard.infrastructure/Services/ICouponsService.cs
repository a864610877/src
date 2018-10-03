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
    }
}
