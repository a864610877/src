using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.AdmissionTickets
{
    public class ListAdmissionTicket
    {
        private readonly AdmissionTicket _innerObject;

        [NoRender]
        public AdmissionTicket InnerObject
        {
            get { return _innerObject; }
        }

        public ListAdmissionTicket()
        {
            _innerObject = new AdmissionTicket();
        }

        public ListAdmissionTicket(AdmissionTicket account)
        {
            _innerObject = account;
        }

        public int id { get { return InnerObject.id; } }
        /// <summary>
        /// 门票名称
        /// </summary>
        public string name { get { return InnerObject.name; } }
        /// <summary>
        /// 大人数量
        /// </summary>
        public int adultNum { get { return InnerObject.adultNum; } }
        /// <summary>
        /// 小孩数量
        /// </summary>
        public int childNum { get { return InnerObject.childNum; } }
        
        /// <summary>
        /// 平时价格
        /// </summary>
        public decimal amount { get { return InnerObject.amount; } }
        /// <summary>
        /// 周末价格
        /// </summary>
        public decimal weekendAmount { get { return InnerObject.weekendAmount; } }
        
        /// <summary>
        /// 状态
        /// </summary>
        public string state
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.state); }
        }
        public DateTime crateTime { get { return InnerObject.crateTime; } }

        [NoRender]
        public string boor { get; set; }
    }
}
