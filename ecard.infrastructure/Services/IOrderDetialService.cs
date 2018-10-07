using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IOrderDetialService
    {
        OrderDetial GetById(int id);

        void Update(OrderDetial item);

       // DataTables<Coupons> Query(CouponsRequest request);

        void Create(OrderDetial item);

        List<OrderDetial> GetOrderNo(string orderNo);
    }
}
