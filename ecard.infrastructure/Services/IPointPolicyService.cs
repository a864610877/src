using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{

    public interface IPointPolicyService
    {
        IEnumerable<PointPolicy> Query();
        void Create(PointPolicy item);
        PointPolicy GetById(int id);
        void Update(PointPolicy item);
        void Delete(PointPolicy item);
    }
}