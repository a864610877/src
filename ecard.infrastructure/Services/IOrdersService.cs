using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IOrdersService
    {
        Orders GetById(int id);
        Orders GetByOrderNo(string orderNo);

        void Update(Orders item);

        //DataTables<IOrders> Query(CouponsRequest request);

        void Create(Orders item);
    }
}
