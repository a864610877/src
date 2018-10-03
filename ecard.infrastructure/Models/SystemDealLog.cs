using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a SystemDealItem.
    /// </summary>
    public class SystemDealLog
    {
        public SystemDealLog(string serialNo, User user)
            : this()
        {
            if (user != null)
            {
                UserId = user.UserId;
                UserName = user.Name;
            }
            SerialNo = serialNo;
            State = SystemDealLogStates.Normal;
        }

        public SystemDealLog()
        {
            SubmitTime = DateTime.Now;
        }

        [Key]
        public int SystemDealLogId { get; set; }

        [Bounded(typeof(SystemDealLogTypes))]
        public int DealType { get; set; }

        public decimal Amount { get; set; }
        public decimal SiteAmount { get; set; }
        public string Addin { get; set; }
        public string SerialNo { get; set; }
        public DateTime SubmitTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string HowToDeal { get; set; }
        public int DealWayId { get; set; }
        public bool HasReceipt { get; set; }
        [Bounded(typeof(SystemDealLogStates))]
        public int State { get; set; }

        public string ShopName { get; set; }

        public string Operator { get; set; }

        public bool CanCancel(int dealType, Site site)
        {
            return DealType == dealType && State == SystemDealLogStates.Normal && (DateTime.Now - SubmitTime).TotalMinutes < site.TimeOutOfCancelSystemDeal;
        }
         
    }
}