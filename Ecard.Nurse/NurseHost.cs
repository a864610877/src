using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Ecard.BackgroundServices;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit.Data;
using log4net;
using log4net.Config;
using System.ServiceModel;

namespace Ecard.Nurse
{
    public partial class NurseHost : ServiceBase
    {
        public NurseHost()
        {
            InitializeComponent();
        }
        public ServiceHost serviceHost = null;

        public void Run()
        {
            this.OnStart(null);
        }
        public class Holder : System.IDisposable
        {
            public static DatabaseInstance Instance { get; set; }
            public static Site HostSite { get; set; }
            public Holder(Database database)
            {
                Instance = new DatabaseInstance(database);
                HostSite = Instance.Query<Site>("select * from sites", null).First();
            }
            public void Dispose()
            {
                using (Instance)
                {

                }
            }
        }
        private Site _site;
        private void LoadParams()
        {
            DatabaseInstance context = _unityContainer.Resolve<DatabaseInstance>();
            string sql = @"select * from sites";
            _site = context.Query<Site>(sql, null).FirstOrDefault();

        }
        public class DelegateLifetimeManager : LifetimeManager
        {
            private readonly Func<object> _getter;

            public DelegateLifetimeManager(Func<object> getter)
            {
                _getter = getter;
            }

            public override object GetValue()
            {
                return _getter();
            }

            public override void SetValue(object newValue)
            {
            }

            public override void RemoveValue()
            {
            }
        }
        private IUnityContainer _unityContainer;
        private Timer _timer;
        private System.Timers.Timer _timerSms;
        protected override void OnStart(string[] args)
        {
            //-----电话平台
            //if (serviceHost != null)
            //{
            //    serviceHost.Close();
            //}
            //serviceHost = new ServiceHost(typeof(OrderClientService));
            //serviceHost.Open();

            //-----

            try
            {
                _unityContainer = new UnityContainer();
                _unityContainer
                    .RegisterInstance<IUnityContainer>(_unityContainer)
                    .RegisterInstance<Database>(new Database("ecard"))
                    .RegisterType<DatabaseInstance>(new DelegateLifetimeManager(() => Holder.Instance))
                    .RegisterType<Site>(new DelegateLifetimeManager(() => Holder.HostSite))
                    .RegisterType<IMembershipService, SqlMembershipService>()
                    .RegisterType<IPrintTicketService, SqlPrintTicketService>()
                    .RegisterType<IAccountService, SqlAccountService>()
                    .RegisterType<IPointPolicyService, SqlPointPolicyService>()
                    .RegisterType<ICashDealLogService, SqlCashDealLogService>()
                    .RegisterType<IDealLogService, SqlDealLogService>()
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
                    .RegisterType<IRollbackShopDealLogService, SqlRollbackShopDealLogService>()
                    .RegisterType<ITemporaryTokenKeyService, SqlTemporaryTokenKeyService>()
                    .RegisterType<ISystemDealLogService, SqlSystemDealLogService>()
                    .RegisterType<ICommodityService, SqlCommodityService>()
                    .RegisterType<ILogService, SqlLogService>()
                    .RegisterType<ILiquidateService, SqlLiquidateService>()
                    .RegisterType<IDealTracker, SmsDealTracker>()
                    .RegisterType<ICacheService, CompositeCacheService>()
                    .RegisterType<IReportService, SqlReportService>()
                    .RegisterType<ISmsService, SqlSmsService>()
                    .RegisterType<IDealWayService, SqlDealWayService>()
                    .RegisterType<IDistributorService, SqlDistributorService>();

                var config = Path.GetDirectoryName(typeof(NurseHost).Assembly.Location) + "\\log4net.config";
                FileInfo repository = new FileInfo(config);
                XmlConfigurator.Configure(repository);

                // run it in two hours
                int time = 60 * 60 * 1 * 1000;
                _timer = new Timer(OnTimer, null, 0, time);
                Log.Info("timer started");

                LoadParams();
                /// _site.SmsTimeSpan = _site.SmsTimeSpan == 0 ? 1 : _site.SmsTimeSpan;
                _timerSms = new System.Timers.Timer(1000 * 6); //new System.Timers.Timer();
                _timerSms.Elapsed += new System.Timers.ElapsedEventHandler(_timerSms_Elapsed);
                _timerSms.Start();
                Log.Info("timer started");
            }
            catch (Exception ex)
            {
                Log.Error("error in start", ex);
            }
        }

        static ILog Log = log4net.LogManager.GetLogger(typeof(NurseHost));
        private void OnTimer(object state)
        {
            Log.Info("OnTimer");
            var now = DateTime.Now;

            Log.Info("OnTimer in time");
            bool indebug = ConfigurationManager.AppSettings["runIn"] == "debug";
            //经销商结算请求，晚上11点执行一次,上线后要改。
            if (indebug||now.Hour > 22)
            {
                try
                {
                    Log.Info("run DistributorBrokerageLogService start");
                    using (_unityContainer.Resolve<Holder>())
                    {
                        _unityContainer.Resolve<DistributorBrokerageLogService>().Execute();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("run DistributorBrokerageLogService error", ex);
                }
                finally
                {
                    Log.Info("run DistributorBrokerageLogService end");
                }
 
            }
            //if (indebug || now.Hour < 6 || now.Hour > 22)
                try
                {
                    Log.Info("run AccountLevelService start");
                    using (_unityContainer.Resolve<Holder>())
                    {
                        _unityContainer.Resolve<AccountLevelService>().Execute();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("run AccountLevelService error", ex);
                }
                finally
                {
                    Log.Info("run AccountLevelService end");
                }
            try
            {
                Log.Info("run ShopDealsReportService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<ShopDealsReportService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run ShopDealsReportService error", ex);
            }
            finally
            {
                Log.Info("run ShopDealsReportService end");
            }
            try
            {
                Log.Info("run AccountMonthReportService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<AccountMonthReportService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run AccountMonthReportService error", ex);
            }
            finally
            {
                Log.Info("run AccountMonthReportService end");
            }
            try
            {
                Log.Info("run SystemDealLogDayReportService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<SystemDealLogDayReportService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run SystemDealLogDayReportService error", ex);
            }
            finally
            {
                Log.Info("run SystemDealLogDayReportService end");
            }

            if (indebug || now.Hour > 8 && now.Hour < 20)
                try
                {
                    Log.Info("run SmsBirthDateService start");
                    using (_unityContainer.Resolve<Holder>())
                    {
                        _unityContainer.Resolve<SmsBirthDateService>().Execute();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("run SmsBirthDateService error", ex);
                }
                finally
                {
                    Log.Info("run SmsBirthDateService end");
                }
            try
            {
                Log.Info("run AccountDealsReportService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<AccountDealsReportService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run AccountDealsReportService error", ex);
            }
            finally
            {
                Log.Info("run AccountDealsReportService end");
            }
            try
            {
                Log.Info("run DealLogDailyService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<DealLogDailyService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run DealLogDailyService error", ex);
            }
            finally
            {
                Log.Info("run DealLogDailyService end");
            }
            try
            {
                Log.Info("run ShopDealLogService start");
                using (_unityContainer.Resolve<Holder>())
                {
                    _unityContainer.Resolve<ShopDealLogService>().Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Error("run ShopDealLogService error", ex);
            }
            finally
            {
                Log.Info("run ShopDealLogService end");
            }
        }

        void _timerSms_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timerSms.Stop();
            try
            {
                SendSms();
            }
            catch (System.Exception ex)
            {
                Log.Error("SendSms Error:" + ex.Message);
            }

            _timerSms.Start();
        }
        private void SendSms()
          {
              Log.Info("SendSms in time");
              //从数据库中读取信息，发送短信。
              if (_site == null)
                  LoadParams();
              string sql = @"select * from Sms where [State] in(@states)";
              var smsService = _unityContainer.Resolve<ISmsService>();
              var query = smsService.Query(new int[] { States.Normal }).ToList();
              foreach (var item in query)
              {
                  //发送
                  //string result = SmsSendHelper.SendSms(SmsSendHelper.SmsUrl, string.Format(SmsSendHelper.SendTemplate, _site.SmsAccount, SmsSendHelper.GetMd532(_site.SmsPwd ?? ""), item.Mobile, item.SmsId, item.Message)).Trim();
                  string result = Ecard.Services.SmsSendHelper.Sends(item.Message, item.Mobile, _site.SmsAccount, _site.SmsPwd);
                  // string[] ress = result.Split('&');
                  string[] strRess = result.Split('&');
                  if (strRess.Length >= 2)
                  {
                      string[] restss = strRess[0].Split('=');
                      if (restss.Length >= 2)
                      {
                          if (restss[1] == "0")
                          {
                              item.State = SmsStates.Complated;
                          }
                          else
                          {
                              item.RetryCount += 1;
                              if (item.RetryCount > _site.RetryCount)
                              {
                                  item.State = SmsStates.Invalid;
                                  item.ExpiredTime = DateTime.Now;
                              }
                          }
                      }
                      else
                      {
                          item.RetryCount += 1;
                          if (item.RetryCount > _site.RetryCount)
                          {
                              item.State = SmsStates.Invalid;
                              item.ExpiredTime = DateTime.Now;
                          }
                      }
                  }
                  else
                  {
                      item.RetryCount += 1;
                      if (item.RetryCount > _site.RetryCount)
                      {
                          item.State = SmsStates.Invalid;
                          item.ExpiredTime = DateTime.Now;
                      }
                  }
                  //if(ress.Length>0&&ress[0]=="0")
                  //    item.State = SmsStates.Complated;
                  //else
                  //{
                  //    item.RetryCount += 1;
                  //    if (item.RetryCount > _site.RetryCount)
                  //    {
                  //        item.State = SmsStates.Invalid;
                  //        item.ExpiredTime = DateTime.Now;
                  //    }
                  //}
                  smsService.Update(item);
              }
              Log.Info("Send count:" + query.Count());

          }
        protected override void OnStop()
        {
            //if (serviceHost != null)
            //{
            //    serviceHost.Close();
            //    serviceHost = null;
            //}

        }
    }
}
