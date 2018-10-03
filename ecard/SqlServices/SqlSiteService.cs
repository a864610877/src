using Ecard.Models;

namespace Ecard.Services
{
    public class SqlSiteService : ISiteService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Sites";

        public SqlSiteService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Site> Query(SiteRequest request)
        {
            return new QueryObject<Site>(_databaseInstance, "Site.query", request);
        }
         

        public void Update(Site item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(Site item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}