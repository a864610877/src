using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.PosApi
{
    public class TicketRespone
    {
        /// <summary>
        /// 门票编码/卡号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 门票/卡 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public string buyTime { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpiredDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 剩余使用次数
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// 已使用次数
        /// </summary>
        public int FrequencyUsed { get; set; }
        /// <summary>
        /// 指定门店
        /// </summary>
        public string useScope { get; set; }
        /// <summary>
        /// 使用日期
        /// </summary>
        public string userTime { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 宝宝姓名
        /// </summary>
        public string babyName { get; set; }
        /// <summary>
        /// 宝宝性别
        /// </summary>
        public string babySex { get; set; }



    }
}
