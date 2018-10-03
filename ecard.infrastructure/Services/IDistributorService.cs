using System.Collections.Generic;
using Ecard.Models;
using Ecard.Infrastructure;

namespace Ecard.Services
{
    public interface IDistributorService
    {
        void Create(Distributor item);
        void Update(Distributor item);
        void UpdateAccountLevelPolicy(int distributorId, DistributorAccountLevelRate rates);
        void Delete(Distributor item); 
        IEnumerable<Distributor> Query ();
        Distributor GetById(int id);
        List<DistributorAccountLevelRate> GetAccountLevelPolicyRates(int distributorId);
        Distributor GetByUserId(int userId);
        List<Distributor> GetByParentId(int ParentId);
        DataTables<Distributor> New_InnerQuery();
    }
}