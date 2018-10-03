using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Distributors
{
    /// <summary>
    /// 提成比例，数据保存在DistributorAccountLevelPolicyRates
    /// </summary>
    public class DistributorAccountLevelRate
    {
        public string AccountLevelPolicyText { get; set; }
        public int AccountLevelPolicyId { get; set; }
        public decimal Rate { get; set; }
    }
}
