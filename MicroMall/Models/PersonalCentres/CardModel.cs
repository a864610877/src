using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.PersonalCentres
{
    public class CardModel
    {

        public CardModel()
        {

        }

        public CardModel(AccountWithOwner item)
        {
          this.cardName = item.AccountTypeName;
          this.name = item.OwnerDisplayName;
          this.mobile = item.OwnerMobileNumber;
          this.babyName = item.BabyName;
          this.babySex = item.BabySex == 1 ? "男孩" : "女孩";
          this.expiredDate = item.ExpiredDate.ToString("yyyy-MM-dd");
            if (this.cardName == "铂金气球卡" || this.cardName == "黑金气球卡")
                this.frequency = -1;
            else
               this.frequency = item.Frequency;
          this.cardNo = item.Name;
        }
        /// <summary>
        /// 卡号
        /// </summary>
        public string cardNo { get; set; }
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