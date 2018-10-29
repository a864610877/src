using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.XWKJSms
{
    public interface ISendSms
    {
        /// <summary>
        /// 单发
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="phone">发送到的手机号</param>
        /// <param name="userName">服务端账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="srm"></param>
        /// <returns></returns>
        bool SendOne(string msg, string phone, string userName, string pwd, out SmsResponseMsg srm);

        /// <summary>
        /// 单发，账号密码在配置文件中获取
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="phone">发送到的手机号</param>
        /// <param name="srm"></param>
        /// <returns></returns>
        bool SendOne(string msg, string phone, out SmsResponseMsg srm);

        /// <summary>
        /// 多发
        /// </summary>
        /// <param name="pms">手机号和消息组</param>
        /// <param name="userName">服务端账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="isMass">是否群发</param>
        /// <param name="srm"></param>
        /// <returns></returns>
        bool Sends(List<SmsPhoneMsg> pms, string userName, string pwd, out SmsResponseMsg srm, bool isMass = false);

        /// <summary>
        /// 多发，账号密码在配置文件中获取
        /// </summary>
        /// <param name="pms">手机号和消息组</param>
        /// <param name="isMass">是否群发</param>
        /// <param name="srm"></param>
        /// <returns></returns>
        bool Sends(List<SmsPhoneMsg> pms, out SmsResponseMsg srm, bool isMass = false);
    }
}
