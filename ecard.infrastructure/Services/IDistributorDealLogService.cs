using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IDistributorDealLogService
    {
        QueryObject<DistributorDealLog> Query(DistributorDealLogRequest request);
        void Create(DistributorDealLog item);
        DistributorDealLog GetById(int id);
        void Update(DistributorDealLog item);
        void Delete(DistributorDealLog item);
        DistributorDealLog GetByAddin(int addin);
        QueryObject<DistributorDealLog> QueryForLiquidateByDeal(int shopDealLogType, string serialServerNo, string accountName, string shopName, int shopType);
        QueryObject<DistributorDealLog> QueryUnLiquidateDeals(int shopId);
        int UpdateLiquidateId(List<int> ids, int liquidateId, int originalId, int shopid);
        List<int> GetAddins(int[] ids);
        QueryObject<DistributorDealLog> GetByIds(int[] ids);
    }
}