using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class OrderDetial1
    {

		#region Model
        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        public int Serialnumber { get; set; }
		/// <summary>
		/// 
		/// </summary>
        public string OrderId { get; set; }
		/// <summary>
		/// 
		/// </summary>
        public int GoodId { get; set; }
		/// <summary>
		/// 
		/// </summary>
        [RegularExpression(@"^\d+$", ErrorMessage = "数量输入有误")]
        public int Amount { get; set; }
		/// <summary>
		/// 
		/// </summary>
        [RegularExpression(@"\d{1,7}(\.\d{2})?", ErrorMessage = "输入的金额有误,负数？超额？")]
        public decimal price { get; set; }
		#endregion Model
    }
}
