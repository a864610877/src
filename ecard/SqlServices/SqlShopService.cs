using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;
using Ecard.Infrastructure;
using System.Data.SqlClient;

namespace Ecard.Services
{
    public class SqlShopService : IShopService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Shops";

        public SqlShopService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Shop> Query(ShopRequest request)
        {
            var sql =
                @"select t.* from shops t where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@IsBuildIn is null or t.BuildIn = @IsBuildIn)
and (@IsMobileAvailable is null 
        or ( @IsMobileAvailable = 1 and exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
        or ( @IsMobileAvailable = 0  and  not exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
    )
and (@ShopId is null or t.ShopId = @ShopId)
and (@NameWith is null or t.Name like '%'+ @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%'+ @DisplayNameWith + '%')
and (@ShopIds is null or t.ShopId in (@ShopIds))";
            return new QueryObject<Shop>(_databaseInstance, sql, request);
        }
        public DataTables<Shop> NewQuery(ShopRequest request)
        {
            SqlParameter[] param = { 
                                   new SqlParameter("@Name",request.Name),
                                   new SqlParameter("@NameWith",request.NameWith),
                                   new SqlParameter("@DisplayNameWith",request.DisplayNameWith),
                                   new SqlParameter("@State",request.State),
                                   new SqlParameter("@pageIndex",request.PageIndex),
                                   new SqlParameter("@pageSize",request.PageSize),
                                   new SqlParameter("@IsMobileAvailable",request.IsMobileAvailable) 
                                   };
            StoreProcedure sp = new StoreProcedure("P_getShops", param);
            return _databaseInstance.GetTables<Shop>(sp);
        }
        public QueryObject<Shop> QueryByName(string name)
        {
            var sql =
                @"select t.* from shops t where (t.Name like @name + '%' or t.displayName like @name + '%') and t.State = @State";
            return new QueryObject<Shop>(_databaseInstance, sql, new { name = name , state = ShopStates.Normal});
        }

        public QueryObject<Shop> QueryWithName(string shopName)
        {
            var sql = @"select t.* from shops t where (t.Name like '%' + @name + '%' or t.mobile like '%' + @name + '%') and t.State = @State";
            return new QueryObject<Shop>(_databaseInstance, sql, new { name = shopName, state = ShopStates.Normal });
        }

        public QueryObject<Shop> GetByIds(int[] ids)
        {
            var sql = "select * from shops where shopid in (@ids)";
            return new QueryObject<Shop>(_databaseInstance, sql, new { ids= ids });
        } 

        public void Create(Shop item)
        {
            item.ShopId = _databaseInstance.Insert(item, TableName);
        }

        public Shop GetById(int id)
        {
            if (id == 0)
                return Shop.Default;
            return _databaseInstance.GetById<Shop>(TableName, id);
        }

        public void Update(Shop item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(Shop item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public QueryObject<Shop> GetShopByMobileNumber(string mobileNumber, int shopId)
        {
            return new QueryObject<Shop>(_databaseInstance, "Shop.GetShopByMobileNumber", new { phoneNumber = mobileNumber, exceptShopId = shopId });
        }

        public QueryObject<ShopWithOwner> QueryShopWithOwner(ShopRequest request)
        {
            var sql =
                @"select t.*, u.DisplayName as OwnerDisplayName, u.Mobile as OwnerMobileNumber from shops t inner join users u on t.ShopId = u.ShopId where
u.IsMobileAvailable = 1
and (@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@ShopId is null or t.ShopId = @ShopId)
and (@NameWith is null or t.Name like '%'+ @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%'+ @DisplayNameWith + '%')
and (@ShopIds is null or t.ShopId in (@ShopIds))";
            return new QueryObject<ShopWithOwner>(_databaseInstance, sql, request);
        }

        public Shop GetByName(string shopName)
        {
            return new QueryObject<Shop>(_databaseInstance, "Select * from shops where name = @name", new { name = shopName }).FirstOrDefault();
        }

    }
}