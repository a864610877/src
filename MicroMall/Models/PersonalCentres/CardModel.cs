using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.PersonalCentres
{
    public class CardModel
    {
        /// <summary>
        /// 卡片名称
        /// </summary>
        public string cardName { get; set; }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 宝宝名字
        /// </summary>
        public string babyName { get; set; }
        /// <summary>
        /// 宝宝性别
        /// </summary>
        public string babySex { get; set; }
        /// <summary>
        /// 剩余使用次数
        /// </summary>
        public int frequency { get; set; }
        /// <summary>
        /// 有效期 
        /// </summary>
        public string expiredDate { get; set; }


    }
}