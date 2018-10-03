using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.Registers
{
    public class register
    {
        public string code { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// 宝宝姓名
        /// </summary>
        public string babyName { get; set; }
        /// <summary>
        /// 宝宝性别    
        /// </summary>
        public int babySex { get; set; }
        /// <summary>
        /// 宝宝出生年月
        /// </summary>
        public DateTime babyBirthDate { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string verifiCode { get; set; }
    }
}