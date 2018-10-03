namespace Ecard.Models
{
    public class DistributorAccountLevelRate
    {
        public int DistributorId { get; set; }
        public int AccountLevelPolicyId { get; set; }
        public decimal Rate { get; set; }
    }
}