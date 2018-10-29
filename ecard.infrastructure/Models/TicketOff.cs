using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class TicketOff
    {
        [Key]
        public int id { get; set; }

        public int userId { get; set; }
        [Bounded(typeof(OffTypes))]

        public int offType { get; set; }
        /// <summary>
        /// 门票代码/卡号
        /// </summary>
        public string code { get; set; }
        public int shopId { get; set; }
        /// <summary>
        /// 核销次数
        /// </summary>
        public int timers { get; set; }
        /// <summary>
        /// 卡名称/门票名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 核销员
        /// </summary>
        public string offOp { get; set; }
        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime subTime { get; set; }
    }

    public class OffTypes
    {
        /// <summary>
        /// 门票
        /// </summary>
        public const int ALL = -10001;
        /// <summary>
        /// 门票
        /// </summary>
        public const int TicKet = 1;
        /// <summary>
        /// 卡
        /// </summary>
        public const int Card = 2;
    }

   
}
