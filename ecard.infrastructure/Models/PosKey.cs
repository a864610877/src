using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class PosKey
    {
        [Key]
        public int PosKeyId { get; set; }

        public string PosName { get; set; }

        public string ShopName { get; set; }

        public string Key1 { get; set; }

        public string Key2 { get; set; }
    }
}
