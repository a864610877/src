using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IShopDealLogService
    {
        QueryObject<ShopDealLog> Query(ShopDealLogRequest request);
        void Create(ShopDealLog item);
        ShopDealLog GetById(int id);
        void Update(ShopDealLog item);
        void Delete(ShopDealLog item);
        ShopDealLog GetByAddin(int addin);
        QueryObject<ShopDealLog> QueryForLiquidateByDeal(int shopDealLogType, string serialServerNo, string accountName, string shopName, int shopType);
        QueryObject<ShopDealLog> QueryUnLiquidateDeals(int shopId);
        int UpdateLiquidateId(List<int> ids, int liquidateId, int originalId, int shopid);
        List<int> GetAddins(int[] ids);
        QueryObject<ShopDealLog> GetByIds(int[] ids);
    }
}