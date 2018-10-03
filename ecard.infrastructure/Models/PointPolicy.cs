using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class PointPolicy : IAccountDependency, IKeySetter
    {
        [Key]
        public int PointPolicyId { get; set; }

        public string DisplayName { get; set; }

        [Bounded(typeof(PointPolicyStates))]
        public int State { get; set; }
        public int Priority { get; set; }

        public decimal Point { get; set; }

        #region IAccountDependency Members
        [Bounded(typeof(AccountDependencyTypes))]
        public int DependencyType { get; set; }
        public string WeekDays { get; set; }
        public string Days { get; set; }
        public string AccountLevels { get; set; }

        #endregion

        int IKeySetter.Id
        {
            get { return PointPolicyId; }
            set { PointPolicyId = value; }
        }
    }
}