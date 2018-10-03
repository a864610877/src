using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class RechargingLog
    {
        public RechargingLog(int code)
        {
            this.State = code;
        }
        public RechargingLog()
        {

        }


        [Key]
        public int RechargingLogId { get; set; }

        public string AccountName { get; set; }
        public string serialNoAll { get; set; }

        public decimal AccountAmount { get; set; }
        public string Name { get; set; }
        public decimal RechargingAmount { get; set; }
        public decimal RechargAccountAmount { get; set; }
        public string Decs { get; set; }
        public int State { get; set; }
        public DateTime SubmitTime { get; set; }


    }
}
