using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class PointRebateLog
    {
        public PointRebateLog()
        {
            SubmitTime = DateTime.Now;
        }

        public PointRebateLog(string serialNo, Account account, User currentUser, PointRebate pointRebate)
        {
            this.SerialNo = serialNo;
            SubmitTime = DateTime.Now;
            this.AccountId = account.AccountId;
            UserId = currentUser.UserId;
            Amount = pointRebate.Amount;
            Point = pointRebate.Point;
            PointRebateId = pointRebate.PointRebateId;
        }


        [Key]
        public int PointRebateLogId { get; set; }
        public DateTime SubmitTime { get; set; }
        public decimal Amount { get; set; }
        public int Point { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public int PointRebateId { get; set; }
        public string SerialNo { get; set; }
    }
    public class PointRebateLogRequest
    {

    }
}