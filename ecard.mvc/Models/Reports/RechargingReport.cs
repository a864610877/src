using System;
using System.Linq;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class RechargingReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
        }
        protected override string[] GetColumns()
        {
            List <string> items = new List<string>();
            items.Add("行号");
            items.Add("员工登录名");
            items.Add("员工姓名");
           var accountTypes =  EnsureAccountTypes();
            foreach (var accountType in accountTypes)
            {
                items.Add(accountType.DisplayName);
            }
            return items.ToArray();
        }

        private List<AccountType> EnsureAccountTypes()
        {
            if (AccountTypes != null) return AccountTypes;
            var accountTypes = DatabaseInstance.Query<AccountType>("select * from accountTypes where state = 1", null).ToList();
            if (accountTypes.Count == 0)
                throw new Exception(Localize("missAccountTypes", "请先创建帐户类型"));
            AccountTypes = accountTypes;
            return AccountTypes;
        }

        public RechargingReport()
        {
            OrderBy = "UserName";
        }

         List<AccountType> AccountTypes { get; set; }
        protected override string GetSql(string sqlName)
        {
            var accountTypes = EnsureAccountTypes();
            if (sqlName == "sql")
            {
                var subQuerySelector = @"  (select isnull(sum(lg.amount) , 0.0) as amount
                                            from systemdeallogs lg inner join accounts acc on acc.accountId = lg.addin and acc.accountTypeId = {0}
                                            where lg.dealtype in ({2}) and lg.userid = u.userid
                                                and (@start is null or @start <= submittime) 
                                                and lg.state = 1
                                                and (@end is null or @end > submittime)) as [{1}]";
                var subQuerys = from x in accountTypes
                                select string.Format(subQuerySelector, x.AccountTypeId, x.DisplayName, SystemDealLogTypes.Recharge);
                var summaryQuery = string.Format(@" (select isnull(sum(lg.amount) , 0.0) as amount
                                            from systemdeallogs lg inner join accounts acc on acc.accountId = lg.addin 
                                            where lg.dealtype in ({0}) and lg.userid = u.userid
                                                and lg.state = 1
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [合计]", SystemDealLogTypes.Recharge);

                string sql =
                    string.Format(
                        @"select u.Name as userName, u.DisplayName as userDisplayName, {0}, {1} from users u where u.discriminator = 'AdminUser'",
                        string.Join(",", subQuerys.ToArray()), summaryQuery
                        );

                return sql;
            }
            else
            {
                var subQuerySelector = @"  (select isnull(sum(lg.amount) , 0.0) as amount
                                            from systemdeallogs lg inner join accounts acc on acc.accountId = lg.addin and acc.accountTypeId = {0}
                                            where lg.dealtype in ({2}) 
                                                and lg.state = 1
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [{1}]";
                var subQuerys = from x in accountTypes
                                select string.Format(subQuerySelector, x.AccountTypeId, x.DisplayName, SystemDealLogTypes.Recharge);
                var summaryQuery = string.Format(@" (select isnull(sum(lg.amount) , 0.0) as amount
                                            from systemdeallogs lg inner join accounts acc on acc.accountId = lg.addin 
                                            where lg.dealtype in ({0}) 
                                                and lg.state = 1
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [合计]", SystemDealLogTypes.Recharge);
                string sql =
                    string.Format(
                        @"select '' as userName, '' as userDisplayName, {0}, {1} from users u where u.discriminator = 'AdminUser'",
                        string.Join(",", subQuerys.ToArray()), summaryQuery
                        );

                return sql;
            }
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("RechargingExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}