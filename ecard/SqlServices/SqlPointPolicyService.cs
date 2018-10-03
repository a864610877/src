using Ecard.Models;

namespace Ecard.Services
{
    public class SqlPointPolicyService : CachedSqlService<PointPolicy>, IPointPolicyService
    {
        protected override string TableName
        {
            get { return "PointPolicies"; }
        }

        public SqlPointPolicyService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        }
    }
}