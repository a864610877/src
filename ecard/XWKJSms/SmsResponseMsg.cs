using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.XWKJSms
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public class SmsResponseMsg
    {
        /// <summary>
        /// 批次号
        /// </summary>
        public Guid Uuid { get; set; }
        /// <summary>
        /// 返回处理结果状态码
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 详细消息
        /// </summary>
        public string Attributes { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        public int UseMillisecond { get; set; }
    }
}
