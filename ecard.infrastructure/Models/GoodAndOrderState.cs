using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class GoodState : States
    {
 
    }
    public class OrderState : States
    {
        /// <summary>
        /// 派送中
        /// </summary>
        public const int Carry = 3;
        /// <summary>
        /// 部分完成
        /// </summary>
        public const int Partially = 4;
        /// <summary>
        /// 已完成
        /// </summary>
        public const int Completed = 5;
    }
}
