using Ecard.XWKJSms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.XWKJ
{
    /// <summary>
    /// 玄武科技短信发送
    /// </summary>
    public class XWKJ_Sms : ISendSms
    {
        public bool SendOne(string msg, string phone, string userName, string pwd, out SmsResponseMsg srm)
        {
            List<SmsPhoneMsg> pms = new List<SmsPhoneMsg>();
            pms.Add(new SmsPhoneMsg() { Phone = phone, Content = msg });
            return Sends(pms, userName, pwd, out srm, false);
        }

        public bool SendOne(string msg, string phone, out SmsResponseMsg srm)
        {
            List<SmsPhoneMsg> pms = new List<SmsPhoneMsg>();
            pms.Add(new SmsPhoneMsg() { Phone = phone, Content = msg });
            return Sends(pms, Config.UserName, Config.Pwd, out srm, false);
        }

        public bool Sends(List<SmsPhoneMsg> pms, string userName, string pwd, out SmsResponseMsg srm, bool isMass = false)
        {
            SmsBase sb = new SmsBase();
            Config.UserName = userName;
            Config.Pwd = pwd;
            SmsResponseMsg rm = new SmsResponseMsg();
            if (!sb.Init())
            {
                rm.Result = 0;
                rm.Message = "初始化失败";
                srm = rm;
                return false;
            }
            return sb.Send(pms, out srm, isMass);
        }

        public bool Sends(List<SmsPhoneMsg> pms, out SmsResponseMsg srm, bool isMass = false)
        {
            return Sends(pms, Config.UserName, Config.Pwd, out srm, isMass);
        }
    }
}
