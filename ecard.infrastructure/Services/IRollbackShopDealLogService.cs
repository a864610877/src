using Ecard.Models;

namespace Ecard.Services
{
    public interface IRollbackShopDealLogService
    {
        QueryObject<RollbackShopDealLog> Query(RollbackShopDealLogRequest request);
        void Create(RollbackShopDealLog item);
        RollbackShopDealLog GetById(int id);
        void Update(RollbackShopDealLog item);
        void Delete(RollbackShopDealLog item);
        QueryObject<RollbackShopDealLog> Query(int? shopId, int? state);
        RollbackShopDealLog GetByShopDealLogId(int shopDealLogId);
    }
}