using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class AccountLevelPolicy : IKeySetter, IKeyObject
    {
        [Key]
        public int AccountLevelPolicyId { get; set; }

        public string DisplayName { get; set; }
        public decimal TotalPointStart { get; set; }
        public int Level { get; set; }
        public int AccountTypeId { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountRate { get; set; }

        [Bounded(typeof(AccountLevelPolicyStates))]
        public int State { get; set; }

        int IKeySetter.Id
        {
            get { return AccountLevelPolicyId; }
            set { AccountLevelPolicyId = value; }
        }

        int IKeyObject.Id
        {
            get { return AccountLevelPolicyId; }
        }
    } 
}