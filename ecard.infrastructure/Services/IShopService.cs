using Ecard.Models;
using Moonlit;
using Ecard.Infrastructure;

namespace Ecard.Services
{
    public interface IShopService
    {
        QueryObject<Shop> Query(ShopRequest request);
        void Create(Shop item);
        Shop GetById(int id);
        void Update(Shop item);
        void Delete(Shop item);
        QueryObject<Shop> GetShopByMobileNumber(string mobileNumber, int shopId);
        QueryObject<ShopWithOwner> QueryShopWithOwner(ShopRequest request);
        Shop GetByName(string shopName);
        QueryObject<Shop> QueryByName(string name);
        QueryObject<Shop> QueryWithName(string shopName);
        QueryObject<Shop> GetByIds(int[] ids);
        DataTables<Shop> NewQuery(ShopRequest request);
    }
}