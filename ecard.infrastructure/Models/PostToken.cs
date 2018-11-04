using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class PostToken
    {
        public int id { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>
        public string posName { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
    }
}
