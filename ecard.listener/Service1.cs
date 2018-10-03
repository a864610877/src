using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceProcess;
//using MWW.Net;
using PI8583;
using PI8583.Network;
using PI8583.Protocal;
using log4net;
using log4net.Config;

namespace DealListener
{
    public partial class Service1 : ServiceBase
    {
        //private static readonly Reactor<I8583ReactorHandler<DealServiceFactory>, IPEndPoint> reactor =
        //    new Reactor<I8583ReactorHandler<DealServiceFactory>, IPEndPoint>();
        static Reactor<I8583ReactorHandler, IPEndPoint> reactor = new Reactor<I8583ReactorHandler, IPEndPoint>();
        private ServiceHost _host;
        private ServiceHost _host2;

        public Service1()
        {
            InitializeComponent();
        }

        public void Run()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            string config = Path.GetDirectoryName(typeof (Service1).Assembly.Location) + "\\log4net.config";
            var repository = new FileInfo(config);
            XmlConfigurator.Configure(repository);
            ILog log = LogManager.GetLogger(typeof (Service1));
            var port = InitPort(log);
            reactor.Accept(port, new IPEndPoint(IPAddress.Any, port));

            InitI8583Initializer();

            _host = new ServiceHost(typeof (WebCacheService));
            _host.Open();
            log.Info("WebCacheService started");


            _host2 = new ServiceHost(typeof (EcardOnlineService));
            _host2.Open();
            log.Info("EcardOnlineService started");
        }

        private void InitI8583Initializer()
        {
            var version = ConfigurationManager.AppSettings["i8583_version"]??"v1";
            switch (version.ToLower())
            {
                case "v1":
                    I8583Initializer.Current = new I8583InitializerV1();
                    break;
                case "v2":
                    I8583Initializer.Current = new I8583InitializerV2();
                    break;
            }
        }

        private static int InitPort(ILog log)
        {
            string portName = ConfigurationManager.AppSettings["port"];
            int port = 3130;
            if (!string.IsNullOrEmpty(portName))
                port = Convert.ToInt32(portName);

            log.Info("service started");
            log.Info("start listen: " + port);
            return port;
        }

        protected override void OnStop()
        {
            ILog log = LogManager.GetLogger(typeof (Service1));
            log.Info("service stoped");
        }
    }
}