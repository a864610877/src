using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Requests
{
    public class CouponsRequest:PageRequest
    {
        public string name { get; set; }

        public string code { get; set; }

        public int? couponsType { get; set; }

        public int? state { get; set; }

        public DateTime? startTime { get; set; }

        public DateTime? endTime { get; set; }
    }
}
