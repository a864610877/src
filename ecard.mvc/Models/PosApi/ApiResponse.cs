using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.PosApi
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ApiResponse
    {
        public ApiResponse()
        {
            Code = "-1001";
            Msg = "校验错误";
        }

        public ApiResponse(string errorMsg)
        {
            Code = "0";
            Msg = errorMsg;
        }


        public ApiResponse(string code, string msg)
        {
            Code = code;
            Msg = msg;
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

    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            Code = "-1001";
            Msg = "校验错误";
        }

        public ApiResponse(string errorMsg)
        {
            Code = "0";
            Msg = errorMsg;
        }


        public ApiResponse(string code, string msg)
        {
            Code = code;
            Msg = msg;
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        public T data { get; set; }
    }
}
