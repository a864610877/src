using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class UseCouponslog
    {
        [Key]
        public int id { get; set; }

        public int userId { get; set; }

        public int couponsId { get; set; }

        public string orderNo { get; set; }
        /// <summary>
        /// 抵扣的金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal discount { get; set; }
        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime useTime { get; set; }
    }
}
