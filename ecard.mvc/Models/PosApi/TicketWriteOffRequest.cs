using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.PosApi
{
    public class TicketWriteOffRequest
    {
        /// <summary>
        /// 门票代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 凭证
        /// </summary>
        public string token { get; set; }
    }

    public class TicketWriteOffResponse : TicketRespone
    {

        public TicketWriteOffResponse()
        {
            Code = "1";
            Msg = "核销成功";
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
    }
}
