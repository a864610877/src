using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Moonlit.Data;
using Moonlit.Reflection.Emit;

namespace Ecard.Services
{
    public class ShopDealLogExecutor
    {
        private readonly Site _site;
        private readonly DatabaseInstance _databaseInstance;

        public ShopDealLogExecutor(Site site, DatabaseInstance databaseInstance)
        {
            _site = site;
            _databaseInstance = databaseInstance;
        }

        public class ShopDealLogItem
        {
            public decimal DealAmount { get; set; }
            public decimal CancelAmount { get; set; }
        }
        public void Refresh(DateTime start, DateTime end, Shop shop, Liquidate liquidate)
        {
            // 1, pay, 4 doneprepay, 3, cancel, 6, canceldoneprepay
            var sql =
                string.Format(
                    @"select 
	                            isnull((select sum(amount) from deallogs where shopId = @shopId and state <> 3 and  (dealtype = 1 or dealType = 4) and submittime >= @start and submittime < @end), 0.0) as dealAmount, 
	                            isnull((select sum(amount) from deallogs where shopId = @shopId and state <> 3 and (dealtype = 2 or dealType = 6) and submittime >= @start and submittime < @end), 0.0) as cancelAmount;
	                        ");
            var list = _databaseInstance.Query<ShopDealLogItem>(sql, new { start = start, end = end, shopId = shop.ShopId });
            ShopDealLogItem item = list.FirstOrDefault();

            if (item != null)
            {
                liquidate.CancelAmount = item.CancelAmount;
                liquidate.DealAmount = item.DealAmount;
            }
            liquidate.SubmitTime = DateTime.Now;
        }
    }
}
