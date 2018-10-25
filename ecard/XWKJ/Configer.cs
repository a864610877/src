using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.XWKJ
{
    class Config
    {
        static Config()
        {
            //ConfigHelper ch = new ConfigHelper();
            //userName = ch.GetOrSet("sms_userName");
            //pwd = ch.GetOrSet("sms_pwd");
            cmIp = "211.147.239.62";
            cmPort = 9070;
            dlIp = "211.147.239.62";
            dlPort = 9080;
        }
        private static string userName = "";
        /// <summary>
        /// 账号
        /// </summary>
        public static string UserName
        {
            get { return Config.userName; }
            set { Config.userName = value; }
        }

        private static string pwd = "";
        /// <summary>
        /// 密码
        /// </summary>
        public static string Pwd
        {
            get { return Config.pwd; }
            set { Config.pwd = value; }
        }

        private static string cmIp = "";
        /// <summary>
        /// 上行网关IP
        /// </summary>
        public static string CmIp
        {
            get { return Config.cmIp; }
        }

        private static int cmPort = 0;
        /// <summary>
        /// 上行网关端口
        /// </summary>
        public static int CmPort
        {
            get { return Config.cmPort; }
        }

        private static string dlIp = "";
        /// <summary>
        /// 下行网关IP
        /// </summary>
        public static string DlIp
        {
            get { return Config.dlIp; }
        }

        private static int dlPort = 0;
        /// <summary>
        /// 下行网关端口
        /// </summary>
        public static int DlPort
        {
            get { return Config.dlPort; }
        }

    }
}
