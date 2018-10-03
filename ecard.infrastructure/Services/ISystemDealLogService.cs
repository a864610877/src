using System.Data;

using System.Linq;
using Ecard.Models;

namespace Ecard.Services
{ 
    public interface ISystemDealLogService
    {
        QueryObject<SystemDealLog> Query(SystemDealLogRequest request);
        void Create(SystemDealLog item);
        SystemDealLog GetById(int id);
        void Update(SystemDealLog item);
        void Delete(SystemDealLog item);
    }
}