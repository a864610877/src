using System;
using System.Data;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Mvc.Models.ShopLiquidates;
using Ecard.Services;

namespace Ecard.BackgroundServices
{
    public class ShopDealLogService : DailyReportService, IBackgroundService
    {
        private readonly IShopService _shopService;
        private readonly IRollbackShopDealLogService _rollbackShopDealLogService;
        private readonly ILiquidateService _liquidateService;
        private readonly AddShopLiquidates _addModel;

        public ShopDealLogService(DatabaseInstance databaseInstance, IShopService shopService, IRollbackShopDealLogService rollbackShopDealLogService, ILiquidateService  liquidateService, AddShopLiquidates addModel)
            : base(databaseInstance)
        {
            _shopService = shopService;
            _rollbackShopDealLogService = rollbackShopDealLogService;
            _liquidateService = liquidateService;
            _addModel = addModel;
        }

        protected override void OnExecute(DateTime date)
        {
            var shops = _shopService.Query(new ShopRequest()).ToList();
            foreach (var shop in shops)
            {
                var ids = _liquidateService.GetIdsForLiquidate(shop.ShopId, date.Date, date.AddDays(1)).Select(x=>x.ShopDealLogId).ToList();
                if(ids.Count  == 0) continue;

                var items = _rollbackShopDealLogService.Query(shop.ShopId, null).ToList();
                ids = ids.Except(items.Select(x => x.ShopDealLogId)).ToList();
                if (ids.Count == 0) continue;

                _addModel.DoLiquidate(shop.ShopId, ids);
            }
        }
    }
}