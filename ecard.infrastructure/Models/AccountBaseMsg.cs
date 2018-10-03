using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ecard.Models.GoodandOrder
{
    /// <summary>
    /// 会员资料
    /// </summary>
    public class ClientAccount
    {
        /// <summary>
        /// 会员id
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// 性别:1、男 ，2、女
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机1
        /// </summary>
        public string Mobile1 { get; set; }
        /// <summary>
        /// 手机2 
        /// </summary>
        public string Mobile2 { get; set; }
        /// <summary>
        /// 电话1
        /// </summary>
        public string Phone1 { get; set; }
        /// <summary>
        /// 电话2
        /// </summary>
        public string Phone2 { get; set; }

    }
}
