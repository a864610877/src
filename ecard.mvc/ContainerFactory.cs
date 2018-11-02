//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------

using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ecard.Infrastructure;
using Ecard.Models;

using Ecard.Mvc.Infrastructure;
using Ecard.Mvc.Skinning;
using Ecard.Routing;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Moonlit;
using Oxite.Infrastructure;
using Oxite.Mvc.Infrastructure;
using Oxite.Mvc.Skinning;
using Oxite.Routing;
using System.Web.Hosting;
using Ecard.SqlServices;

namespace Ecard.Mvc
{
    public class Constants
    {
        public const string SystemShopName = "000000000000000";
        public const string KeyOfDatabaseInstance = "database_instance";

        public const string Session_AccountToken = "accountToken";
    }
    /// <summary>
    /// </summary>
    public class ContainerFactory
    {
        public IUnityContainer GetEcardContainer()
        {
            IUnityContainer parentContainer = new UnityContainer();

            parentContainer
                .RegisterInstance(new AppSettingsHelper(ConfigurationManager.AppSettings))
                .RegisterInstance(RouteTable.Routes)
                .RegisterInstance(HostingEnvironment.VirtualPathProvider);

            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);
            }

            parentContainer
                .RegisterInstance<RouteCollection>(RouteTable.Routes)
                .RegisterType<IPermissionService, PermissionService>()
                .RegisterType<IActionInvoker, EcardControllerActionInvoker>()
                .RegisterType<IFormsAuthentication, FormsAuthenticationWrapper>()
                .RegisterType<ISkinEngine, VirtualPathProviderSkinEngine>() 
                .RegisterType<ITokenHelper, TokenHelper>()
                .RegisterInstance<II18N>(new I18N(HttpContext.Current.Server.MapPath("~/configs")))
                .RegisterType<I18NManager>()
                .RegisterType<IMenuService, XmlReflectionMenuService>()
                .RegisterType<ITaskService, SqlTaskService>()
                .RegisterType<IMembershipService, SqlMembershipService>()
                .RegisterType<IPrintTicketService, SqlPrintTicketService>()
                .RegisterType<IAccountService, SqlAccountService>()
                .RegisterType<IPointPolicyService, SqlPointPolicyService>()
                .RegisterType<IDealLogService, SqlDealLogService>()
                .RegisterType<IRollbackShopDealLogService, SqlRollbackShopDealLogService>()
                .RegisterType<IAccountDealService, AccountDealService>()
                .RegisterType<IAccountDealDal, CachedSqlAccountDealDal>()
                .RegisterType<IPosEndPointService, SqlPosEndPointService>()
                .RegisterType<ISiteService, SqlSiteService>()
                .RegisterType<IShopService, SqlShopService>()
                .RegisterType<IShopDealLogService, SqlShopDealLogService>()
                .RegisterType<IAccountLevelPolicyService, SqlAccountLevelPolicyService>()
                .RegisterType<IPointRebateService, SqlPointRebateService>()
                .RegisterType<IPointGiftService, SqlPointGiftService>()
                .RegisterType<IPrePayService, SqlPrePayService>()
                .RegisterType<IAmountRateService, SqlAmountRateService>()
                .RegisterType<IAccountTypeService, SqlAccountTypeService>()
                .RegisterType<ITemporaryTokenKeyService, SqlTemporaryTokenKeyService>()
                .RegisterType<ISystemDealLogService, SqlSystemDealLogService>()
                .RegisterType<ICommodityService, SqlCommodityService>()
                .RegisterType<ILogService, SqlLogService>()
                .RegisterType<IDealTracker, SmsDealTracker>()
                .RegisterType<ICacheService, CompositeCacheService>()
                .RegisterType<IReportService, SqlReportService>()
                .RegisterType<ILiquidateService, SqlLiquidateService>()
                .RegisterType<IDistributorService, SqlDistributorService>()
                .RegisterType<IPointRebateLogService, SqlPointRebateLogService>()
                .RegisterType<IDealWayService, SqlDealWayService>()
                .RegisterType<ICashDealLogService, SqlCashDealLogService>()
                .RegisterType<ICashDealLogSummaryService, SqlCashDealLogSummaryService>()
                .RegisterType<ISmsService, SqlSmsService>()
                .RegisterType<IPosKeyService,SqlPosKeyService>()
                .RegisterType<IOrder1Service, SqlOrder1Service>()
                .RegisterType<IRechargingLogService, SqlRechargingLogService>()
                .RegisterType<IAdmissionTicketService,SqlAdmissionTicketService>()
                .RegisterType<ICouponsService,SqlCouponsService>()
                .RegisterType<IArticlesService, SqlArticlesService>()
                .RegisterType<ISetWeChatService, SqlSetWeChatService>()
                .RegisterType<IOrdersService, SqlOrdersService>()
                .RegisterType<IOrderDetialService, SqlOrderDetialService>()
                .RegisterType<IUseCouponslogService,SqlUseCouponslogService>()
                .RegisterType<ITicketOffService, SqlTicketOffService>()
                .RegisterType<ITicketsService,SqlTicketsService>()
                .RegisterType<IWindowTicketingService,SqlWindowTicketingService>()
                .RegisterType<IDashboardItemRepository, DashboardItemRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRegisterRoutes, EcardRoutes>(
                    new InjectionConstructor(
                        new ResolvedParameter<RouteCollection>(),
                        new ResolvedParameter<AppSettingsHelper>(),
                        new ResolvedParameter<Site>()
                        ));

            // TODO: CreateChildContainer?
            IUnityContainer oxiteContainer = parentContainer.CreateChildContainer();

            oxiteContainer.LoadConfiguration("ecard");

            oxiteContainer.RegisterInstance(oxiteContainer);

            return oxiteContainer;
        }
    }
}
