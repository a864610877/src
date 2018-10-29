using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.XWKJSms
{
    /// <summary>
    ///  手机号码和短信内容
    /// </summary>
    public class SmsPhoneMsg
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 讯息内容
        /// </summary>
        public string Content { get; set; }
    }
}
