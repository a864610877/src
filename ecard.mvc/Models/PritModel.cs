using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models
{
    public class PritModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        public string mobile { get; set; }
        /// <summary>
        /// 入场人数
        /// </summary>
        public int people { get; set; }
        /// <summary>
        /// 入场时间
        /// </summary>
        public string einlass { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public string effectiveTime { get; set; }

    }
}
