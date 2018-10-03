using System;
using System.Linq;
using Ecard.Models;
using Moonlit;

namespace Ecard.Services
{
    public class SqlLiquidateService : ILiquidateService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Liquidates";

        public SqlLiquidateService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Liquidate> QueryShopLiquidate(int? shopId, int? state)
        {
            var sql = "select * from Liquidates where (@shopId is null or shopId = @shopId) and (@state is null or state = @state)";
            return new QueryObject<Liquidate>(_databaseInstance, sql, new { shopId = shopId, state = state });
        }

        public Liquidate GetById(int id)
        {
            return _databaseInstance.GetById<Liquidate>(TableName, id);
        }
        public QueryObject<Liquidate> GetByIds(int[] ids)
        {
            var sql = "select * from Liquidates where liquidateid in (@ids)";
            return new QueryObject<Liquidate>(_databaseInstance, sql, new { ids = ids });
        }

        public Liquidate ReadyLiquidate(int shopId, int[] ids)
        {
            var sql =
                "select @shopId ShopId, (select count(*) from shopdeallogs where LiquidateDealLogId = 0 and shopid = @shopId and dealtype in (1,2,4,6) and [state] <> @state and ShopDealLogId in (@ids)) [Count]" +
                      ",(select sum(amount) from shopdeallogs where LiquidateDealLogId = 0 and shopid = @shopId and dealtype in (1,4,8) and [state] <> @state and ShopDealLogId in (@ids)) [DealAmount]" +
                      ",(select -sum(amount) from shopdeallogs where LiquidateDealLogId = 0 and shopid = @shopId and dealtype in (2,6,8) and [state] <> @state and ShopDealLogId in (@ids)) [CancelAmount]";
            var liquidate = new QueryObject<Liquidate>(_databaseInstance, sql,
                                              new { shopId = shopId, ids = ids, state = DealLogStates.Normal_ }).First();

            return liquidate;
        }
        public QueryObject<ShopDealLog> GetIdsForLiquidate(int shopId, DateTime start, DateTime end)
        {
            var sql =
                "select shopDealLogId from shopdeallogs where LiquidateDealLogId = 0 " +
                " and shopid = @shopId and dealtype in (1,2,4,6,8)" +
                " and [state] <> @state and (submittime >= @start and submittime < @end) ";
            var liquidate = new QueryObject<ShopDealLog>(_databaseInstance, sql,
                                              new { shopId = shopId, state = DealLogStates.Normal_, start = start, end = end }) ;
            return liquidate;
        }

        public void Insert(Liquidate liquidate)
        {
            liquidate.LiquidateId = _databaseInstance.Insert(liquidate, TableName);
        }

        public void Update(Liquidate liquidate)
        {
            _databaseInstance.Update(liquidate, TableName);
        }

        public void Delete(Liquidate item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}