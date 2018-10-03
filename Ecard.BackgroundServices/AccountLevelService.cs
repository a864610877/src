using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class AccountLevelService : IBackgroundService
    {
        private readonly DatabaseInstance _databaseInstance;

        public AccountLevelService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public void Execute()
        {
            var now = DateTime.Now;


            foreach (var policy in _databaseInstance.Query<AccountLevelPolicy>("select * from accountLevelPolicies where state = 1 order by level", null).ToList())
            {
                Update(policy);
            }
        }

        private void Update(AccountLevelPolicy policy)
        {
            List<DbParameter> parameters = new List<DbParameter>();

            var sql = "update Accounts set AccountLevelName = @name, accountLevel = @level where accountLevel <= @level and TotalPoint >= @TotalPointStart and accountTypeId = @accountTypeId";

            _databaseInstance.ExecuteNonQuery(sql, new { name = policy.DisplayName, level = policy.Level, TotalPointStart = policy.TotalPointStart, accountTypeId = policy.AccountTypeId });

        }
    }
}
