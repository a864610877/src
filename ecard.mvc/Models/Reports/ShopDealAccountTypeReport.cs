using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Reports
{
    public class ShopDealAccountTypeReport : EcardModelReportRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        protected override void OnReady()
        {
            SetParameter("Start", Start);
            SetParameter("End", End == null ? null : (DateTime?)End.Value.Date.AddDays(1));
        }

        public ShopDealAccountTypeReport()
        {
            OrderBy = "ShopName";
        }
        protected override string[] GetColumns()
        {
            var names = base.GetColumns();
            var accountTypes = DatabaseInstance.Query<AccountType>("select * from accountTypes where state = 1", null).ToList();
            return names.Union(accountTypes.Select(x => x.DisplayName)).ToArray();
        }
        protected override string GetSql(string sqlName)
        {
            var accountTypes = DatabaseInstance.Query<AccountType>("select * from accountTypes where state = 1", null).ToList();
            if (accountTypes.Count == 0)
                throw new Exception(Localize("missAccountTypes", "请先创建帐户类型"));
            if (sqlName == "sql")
            {
                var subQuerySelector = @"   (select isnull(sum(lg.amount) , 0.0) as amount
                                            from deallogs lg inner join accounts acc on acc.accountId = lg.accountId and acc.accountTypeId = {3}
                                            where lg.dealtype in ({0}, {1}) and lg.shopId = s.shopid
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [{2}]";
                var subQuerys = from x in accountTypes
                                select string.Format(subQuerySelector,
                                    DealTypes.Deal, DealTypes.DonePrePay, x.DisplayName, x.AccountTypeId);
                var summaryQuery = string.Format(@"   (select isnull(sum(lg.amount) , 0.0) as amount
                                            from deallogs lg inner join accounts acc on acc.accountId = lg.accountId 
                                            where lg.dealtype in ({0}, {1}) and lg.shopId = s.shopid
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [合计]", DealTypes.Deal, DealTypes.DonePrePay);

                string sql =
                    string.Format(
                        @"select s.Name as shopName, s.DisplayName as shopDisplayName, {0}, {1} from shops s",
                        string.Join(",", subQuerys.ToArray()), summaryQuery
                        );

                return sql;
            }
            else
            {

                var subQuerySelector = @"   (select isnull(sum(lg.amount) , 0.0) as amount
                                            from deallogs lg inner join accounts acc on acc.accountId = lg.accountId and acc.accountTypeId = {3}
                                            where lg.dealtype in ({0}, {1})
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [{2}]";
                var subQuerys = from x in accountTypes
                                select string.Format(subQuerySelector,
                                    DealTypes.Deal, DealTypes.DonePrePay, x.DisplayName, x.AccountTypeId);
                var summaryQuery = string.Format(@"   (select isnull(sum(lg.amount) , 0.0) as amount
                                            from deallogs lg inner join accounts acc on acc.accountId = lg.accountId 
                                            where lg.dealtype in ({0}, {1}) 
                                                and (@start is null or @start <= submittime) 
                                                and (@end is null or @end > submittime)) as [合计]", DealTypes.Deal, DealTypes.DonePrePay);
                string sql =
                    string.Format(
                        @"select '' as shopName, '' as shopDisplayName, {0}, {1} from shops s",
                        string.Join(",", subQuerys.ToArray()), summaryQuery
                        );

                return sql;
            }
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("ShopDealAccountTypeExport", "Report", new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("Print", "Unity");
        }
    }
}