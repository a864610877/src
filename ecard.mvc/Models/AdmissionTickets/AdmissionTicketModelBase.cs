using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.AdmissionTickets
{
    public class AdmissionTicketModelBase:ViewModelBase
    {
        private AdmissionTicket _innerObject;
        public AdmissionTicketModelBase()
        {
            _innerObject = new AdmissionTicket();
        }
        public AdmissionTicketModelBase(AdmissionTicket admissionTicket)
        {
            _innerObject = admissionTicket;
        }
        [NoRender]
        public AdmissionTicket InnerObject
        {
            get { return _innerObject; }
        }

        [NoRender, Dependency]
        public IAdmissionTicketService AdmissionTicketService { get; set; }

        /// <summary>
        /// 门票名称
        /// </summary>
        [Required(ErrorMessage = "请输入门票名称")]
        [StringLength(100)]
        public string name { get { return InnerObject.name; } set { InnerObject.name = value; } }
        /// <summary>
        /// 大人数量
        /// </summary>
        public int adultNum { get { return InnerObject.adultNum; } set { InnerObject.adultNum = value; } }
        /// <summary>
        /// 小孩数量
        /// </summary>
        public int childNum { get { return InnerObject.childNum; } set { InnerObject.childNum = value; } }
        /// <summary>
        /// 增加一位大人需要收取得金额
        /// </summary>
       // public decimal addAdultAmount { get { return InnerObject.addAdultAmount; } set { InnerObject.addAdultAmount = value; } }
        /// <summary>
        /// 平时价格
        /// </summary>
        [RegularExpression(@"\d{1,7}(\.\d{2})?", ErrorMessage = "输入的金额有误，必须大于0")]
        public decimal amount { get { return InnerObject.amount; } set { InnerObject.amount = value; } }
        /// <summary>
        /// 周末价格
        /// </summary>
        [RegularExpression(@"\d{1,7}(\.\d{2})?", ErrorMessage = "输入的金额有误，必须大于0")]
        public decimal weekendAmount { get { return InnerObject.weekendAmount; } set { InnerObject.weekendAmount = value; } }
        /// <summary>
        /// 介绍
        /// </summary>
        [UIHint("richtext")]
        public string introduce { get { return InnerObject.introduce; } set { InnerObject.introduce = value; } }

       
    }
}
