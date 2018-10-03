using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;

namespace Ecard.Mvc.Models.Accounts
{
   public class ShowPay
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeriaNo { get; set; }
        /// <summary>
        /// 实际交易金额（折扣后金额）
        /// </summary>
        public decimal DealAmount { get; set; }
        /// <summary>
        /// 折扣比例
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 应答码
        /// </summary>
        [Bounded(typeof(ResponseCode))]
        public int Code { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 帐户
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 当前积分
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// 获得积分
        /// </summary>
        public int PayPoint { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpiredDate { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string OwnerDisplayName { get; set; }
        /// <summary>
        /// 应答码文本
        /// </summary>
        public string CodeText { get; set; }
        /// <summary>
        /// 可用次数
        /// </summary>
        public int MeteringPayCount { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        public string AccountTypeName { get; set; }
        /// <summary>
        /// 原交易金额
        /// </summary>
        public decimal OldAmount { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayWay { get; set; }
    }
}
