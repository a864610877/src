using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.PosApi
{
    public class LoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    public class LoginRespone : ApiResponse
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string usercode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///  其他接口使用
        /// </summary>
        public string token { get; set; }
    }
}
