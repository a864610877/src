using Ecard.Models;

namespace Ecard.Services
{
    public class SqlDealWayService : CachedSqlService<DealWay>, IDealWayService
    {
        public SqlDealWayService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        }

        protected override string TableName
        {
            get { return "DealWays"; }
        }
    }
}