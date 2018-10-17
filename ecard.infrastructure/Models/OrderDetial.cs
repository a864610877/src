using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class OrderDetial
    {
        [Key]
        public int id { get; set; }

        public string orderNo { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 源Id
        /// </summary>
        public int sourceId { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 充值卡号
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime subTime { get; set; }
    }
}
