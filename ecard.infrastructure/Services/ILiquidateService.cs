using System;
using Ecard.Models;

namespace Ecard.Services
{
    public interface ILiquidateService
    {
        QueryObject<Liquidate> QueryShopLiquidate(int? shopId, int? state);
        void Delete(Liquidate item);
        Liquidate GetById(int id);
        QueryObject<Liquidate> GetByIds(int[] ids);
        Liquidate ReadyLiquidate(int shopId, int[] ids);
        void Insert(Liquidate liquidate);
        void Update(Liquidate liquidate);
        QueryObject<ShopDealLog> GetIdsForLiquidate(int shopId, DateTime start, DateTime end);
    }
}