using System.Web.Mvc;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models.Reports;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        [ CheckPermission(Permissions.ReportShopDeals)]
        public ActionResult ShopDealsExport(ShopDealsReport report)
        {
            return ShopDeals(report);
        }

        [CheckPermission(Permissions.ReportShopDeals)]
        public ActionResult ShopDeals(ShopDealsReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccountDeals)]
        public ActionResult AccountDealsExport(AccountDealsReport report)
        {
            return AccountDeals(report);
        }

        [CheckPermission(Permissions.ReportAccountDeals)]
        public ActionResult AccountDeals(AccountDealsReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccountMonth)]
        public ActionResult AccountMonthExport(AccountMonthReport report)
        {
            return AccountMonth(report);
        }

        [CheckPermission(Permissions.ReportAccountMonth)]
        public ActionResult AccountMonth(AccountMonthReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportSystemDealLogDay)]
        public ActionResult SystemDealLogDayExport(SystemDealLogDayReport systemDealLogDayReport)
        {
            return SystemDealLogDay(systemDealLogDayReport);
        }

        [CheckPermission(Permissions.ReportSystemDealLogDay)]
        public ActionResult SystemDealLogDay(SystemDealLogDayReport report)
        {
            report.Ready();
            return View(report);
        }

        [CheckPermission(Permissions.ReportSystemDealLogByUser)]
        public ActionResult SystemDealLogByUserExport(SystemDealLogByUserReport systemDealLogByUserReport)
        {
            return SystemDealLogByUser(systemDealLogByUserReport);
        }

        [CheckPermission(Permissions.ReportSystemDealLogByUser)]
        public ActionResult SystemDealLogByUser(SystemDealLogByUserReport report)
        {
            report.Ready();
            return View(report);
        }

        [CheckPermission(Permissions.ReportRecharging)]
        public ActionResult RechargingExport(RechargingReport report)
        {
            return Recharging(report);
        }

        [CheckPermission(Permissions.ReportRecharging)]
        public ActionResult Recharging(RechargingReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportRechargingList)]
        public ActionResult RechargingListExport(RechargingListReport report)
        {
            return RechargingList(report);
        }

        [CheckPermission(Permissions.ReportRechargingList)]
        public ActionResult RechargingList(RechargingListReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportShopDealAccountType)]
        public ActionResult ShopDealAccountTypeExport(ShopDealAccountTypeReport report)
        {
            return ShopDealAccountType(report);
        }

        [CheckPermission(Permissions.ReportShopDealAccountType)]
        public ActionResult ShopDealAccountType(ShopDealAccountTypeReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccountDealsList)]
        public ActionResult AccountDealsListExport(AccountDealsListReport report)
        {
            return AccountDealsList(report);
        }

        [CheckPermission(Permissions.ReportAccountDealsList)]
        public ActionResult AccountDealsList(AccountDealsListReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportSaleAccount)]
        public ActionResult SaleAccountExport(SaleAccountReport report)
        {
            return SaleAccount(report);
        }

        [CheckPermission(Permissions.ReportSaleAccount)]
        public ActionResult SaleAccount(SaleAccountReport report)
        {
            report.Ready();
            return View(report);
        }
        [ CheckPermission(Permissions.ReportSaleAccountList)]
        public ActionResult SaleAccountListExport(SaleAccountListReport report)
        {
            return SaleAccountList(report);
        }

        [CheckPermission(Permissions.ReportSaleAccountList)]
        public ActionResult SaleAccountList(SaleAccountListReport report)
        {
            report.Ready();
            return View(report);
        }


        [ CheckPermission(Permissions.ReportCreateAccount)]
        public ActionResult CreateAccountExport(CreateAccountReport report)
        {
            return CreateAccount(report);
        }

        [CheckPermission(Permissions.ReportCreateAccount)]
        public ActionResult CreateAccount(CreateAccountReport report)
        {
            report.Ready();
            return View(report);
        }
        [ CheckPermission(Permissions.ReportCreateAccountList)]
        public ActionResult CreateAccountListExport(CreateAccountListReport report)
        {
            return CreateAccountList(report);
        }

        [CheckPermission(Permissions.ReportCreateAccountList)]
        public ActionResult CreateAccountList(CreateAccountListReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccountDealsForAccount)]
        public ActionResult AccountDealsForAccountExport(AccountDealsForAccountReport report)
        {
            return AccountDealsForAccount(report);
        }

        [CheckPermission(Permissions.ReportAccountDealsForAccount)]
        public ActionResult AccountDealsForAccount(AccountDealsForAccountReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccounts)]
        public ActionResult AccountsExport(AccountsReport report)
        {
            return Accounts(report);
        }
        [CheckPermission(Permissions.ReportAccounts)]
        public ActionResult Accounts(AccountsReport report)
        {
            report.Ready();
            return View(report);
        }

        [ CheckPermission(Permissions.ReportAccounts2)]
        public ActionResult Accounts2Export(Accounts2Report report)
        {
            return Accounts2(report);
        }
        [CheckPermission(Permissions.ReportAccounts2)]
        public ActionResult Accounts2(Accounts2Report report)
        {
            report.Ready();
            return View(report);
        }
        [ CheckPermission(Permissions.ReportExpiredAccounts)]
        public ActionResult ExpiredAccountsExport(ExpiredAccountsReport report)
        {
            return ExpiredAccounts(report);
        }
        [CheckPermission(Permissions.ReportExpiredAccounts)]
        public ActionResult ExpiredAccounts(ExpiredAccountsReport report)
        {
            report.Ready();
            return View(report);
        }
        [ CheckPermission(Permissions.ReportPrepayList)]
        public ActionResult PrepayListExport(PrepayListReport report)
        {
            return PrepayList(report);
        }
        [CheckPermission(Permissions.ReportPrepayList)]
        public ActionResult PrepayList(PrepayListReport report)
        {
            report.Ready();
            return View(report);
        }
        [CheckPermission(Permissions.ReportAllSystemSummary)]
        public ActionResult AllSystemSummary(AllSystemSummaryReport report)
        {
            report.Ready();
            return View(report);
        }
    }
}