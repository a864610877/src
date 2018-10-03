using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class RollbackShopDealLog
    {
        public RollbackShopDealLog()
        {
            this.SubmitTime = DateTime.Now;
        }
        [Key]
        public int RollbackShopDealLogId { get; set; }

        public int ShopId { get; set; }
        public int ShopDealLogId { get; set; }
        [Bounded(typeof(RollbackShopDealLogState))]
        public int State { get; set; }
        public DateTime SubmitTime { get; set; }
    }
}