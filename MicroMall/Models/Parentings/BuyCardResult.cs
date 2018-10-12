using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.Parentings
{
    public class BuyCardResult
    {
        public List<AccountType> accountTypes { get; set; }
        public List<UseCoupons> ListCoupons { get; set; }
    }
}