using Ecard.Models;

namespace Ecard.Services
{
    public interface ICommodityService
    {
        QueryObject<Commodity> Query( CommodityRequest request);
        void Create(Commodity item);
        Commodity GetById(int id);
        void Update(Commodity item);
        void Delete(Commodity item);
    }
}