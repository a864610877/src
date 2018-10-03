using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IPointRebateService
    {
        IEnumerable<PointRebate> Query();
        void Create(PointRebate item);
        PointRebate GetById(int id);
        void Update(PointRebate item);
        void Delete(PointRebate item);
    }
}