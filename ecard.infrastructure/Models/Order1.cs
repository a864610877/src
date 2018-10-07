using System;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;
using System.Collections.Generic;

namespace Ecard.Models
{
    /// <summary>
    /// Orders:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class Order1
    {
        #region Model
        /// <summary>
        /// 订单号规则：SyyyyMMdd-00001
        /// </summary>
        //[Key]
        public string OrderId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int AccountId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Serialnumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { set; get; }
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "输入的必须是手机号码和电话号码")]
        public string Phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Creater { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime SubmitTime { set; get; }

        public int? SenderId { get; set; }
        public string Sender { get; set; }
        public int CreaterId { get; set; }
        public DateTime createDate { get; set; }
        public string Demo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        
        [Bounded(typeof(OrderState))]
        public int State { set; get; }

        #endregion Model
    }
}