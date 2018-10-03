using System.Collections.Generic;
using System.Linq;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IDealWayService
    {
        IEnumerable<DealWay> Query();
        void Create(DealWay item);
        DealWay GetById(int id);
        void Update(DealWay item);
        void Delete(DealWay item);
    }
}