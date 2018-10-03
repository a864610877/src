using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlShopDealLogService : IShopDealLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "ShopDealLogs";

        public SqlShopDealLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public ShopDealLog GetByAddin(int addin)
        {
            var sql = @"select * from shopdeallogs where addin = @addin";
            return new QueryObject<ShopDealLog>(_databaseInstance, sql, new { addin = addin }).FirstOrDefault();
        }

        public QueryObject<ShopDealLog> QueryForLiquidateByDeal(int shopDealLogType, string serialServerNo, string accountName, string shopName, int shopType)
        {
            int v = 0;
            int? dealLogId = null;
            if (int.TryParse(serialServerNo, out v))
                dealLogId = v;
            var sql =
                @"select t.* from shopdeallogs t inner join deallogs d on t.addin = d.deallogid inner join shops s on s.shopid = t.shopid
                        where s.ShopDealLogType = @ShopDealLogType
                            and (@dealLogId is null or t.addin = @dealLogId) 
                            and (@accountName is null or t.accountName = @accountName)
                            and (@shopName is null or t.shopName = @shopName)
                            and (d.LiquidateDealLogId = 0 or d.LiquidateDealLogId is null) and s.ShopType = {0} and t.dealtype in (1,2,4,6,8) and d.state <> {1}";
            sql = string.Format(sql, shopType, DealLogStates.Normal_);
            return new QueryObject<ShopDealLog>(_databaseInstance, sql, new { dealLogId = dealLogId, accountName = accountName, shopName = shopName, ShopDealLogType = shopDealLogType });
        }

        public QueryObject<ShopDealLog> QueryUnLiquidateDeals(int shopId)
        {
            var sql =
              @"select t.* from shopdeallogs t 
                        where t.shopId = @shopId 
                            and t.LiquidateDealLogId = 0
                            and t.dealtype in (1,2,4,6,8) and t.state <> @state";
            return new QueryObject<ShopDealLog>(_databaseInstance, sql, new { shopId = shopId, state = DealLogStates.Normal_ });
        }

        public int UpdateLiquidateId(List<int> ids, int liquidateId, int originalId, int shopId)
        {
            var sql = @"update shopdeallogs set LiquidateDealLogId = @liquidateId where state <> @state and shopdeallogid in (@ids) and dealtype in (1,2,4,6,8) and shopid = @shopid and LiquidateDealLogId = @originalId";
            return _databaseInstance.ExecuteNonQuery(sql, new { shopId = shopId, state = DealLogStates.Normal_, liquidateId = liquidateId, ids = ids, originalId = originalId });
        }

        public List<int> GetAddins(int[] ids)
        {
            var sql = "select addin from shopdeallogs where shopdeallogid in (@ids)";
            var table = _databaseInstance.Table(sql, new { ids = ids });
            List<int> items = new List<int>();
            foreach (DataRow row in table.Rows)
            {
                items.Add(Convert.ToInt32(row["addin"]));
            }
            return items;
        }

        public QueryObject<ShopDealLog> GetByIds(int[] ids)
        {
            return new QueryObject<ShopDealLog>(_databaseInstance, "select * from shopdeallogs where shopdeallogid in (@ids)", new { ids = ids });
        }


        public QueryObject<ShopDealLog> Query(ShopDealLogRequest request)
        {
            var sql = @"select t.* from shopdeallogs t inner join deallogs d on t.addin = d.deallogid where
            (@shopId is null or t.shopId = @shopId)
            and (@State is null or t.State = @State)
            and (@accountName is null or t.accountName = @accountName)
            and (@SubmitDateMin is null or t.submittime >= @SubmitDateMin)
            and (@SubmitDateMax is null or t.submittime <= @SubmitDateMax)
            and (@SerialNO is null or t.SerialNO = @SerialNO)
            and (@AccountShopId is null or exists(select * from accounts a where a.accountid = d.accountid and a.shopid = @AccountShopId))
            and (@IsLiquidate is null or (@IsLiquidate = 1 and t.state <> 3 and d.LiquidateDealLogId <> 0) or (@IsLiquidate = 0 and t.state <> 3 and d.LiquidateDealLogId = 0 and d.dealtype in (1,2,4,6,8)))
";
            if (request.SubmitDateMax != null)
                request.SubmitDateMax = request.SubmitDateMax.Value.AddDays(1);
            return new QueryObject<ShopDealLog>(_databaseInstance, sql, request);
        }

        public void Create(ShopDealLog item)
        {
            item.ShopDealLogId = _databaseInstance.Insert(item, TableName);
        }

        public ShopDealLog GetById(int id)
        {
            return _databaseInstance.GetById<ShopDealLog>(TableName, id);
        }

        public void Update(ShopDealLog item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(ShopDealLog item)
        {
            _databaseInstance.Delete(item, TableName);
        }

    }

}