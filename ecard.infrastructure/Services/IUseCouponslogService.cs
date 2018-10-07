using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IUseCouponslogService
    {
       // UseCouponslog GetById(int id);

        //void Update(UseCouponslog item);

        // DataTables<Coupons> Query(CouponsRequest request);

        void Create(UseCouponslog item);
    }
}
