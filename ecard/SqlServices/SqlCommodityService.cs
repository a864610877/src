using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlCommodityService : ICommodityService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Commodities";
        public SqlCommodityService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Commodity> Query(CommodityRequest request)
        {
            return new QueryObject<Commodity>(_databaseInstance, "Commodity.query", request);
        }

        public void Create(Commodity item)
        {
            item.CommodityId = _databaseInstance.Insert(item, TableName);
        }

        public Commodity GetById(int id)
        {
            return _databaseInstance.GetById<Commodity>(TableName, id);
        }

        public void Update(Commodity item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(Commodity item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}