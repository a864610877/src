using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlAccountLevelPolicyService : CachedSqlService<AccountLevelPolicy>, IAccountLevelPolicyService
    {
        protected override string TableName
        {
            get { return "AccountLevelPolicies"; }
        }
        public SqlAccountLevelPolicyService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        } 
    }
}