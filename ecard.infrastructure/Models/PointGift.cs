using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class PointGift : IAccountDependency, IKeySetter
    {
        [Key]
        public int PointGiftId { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }
        [Bounded(typeof(PointGiftStates))]
        public int State { get; set; }
        public int Priority { get; set; }

        public int DependencyType { get; set; }
        public string WeekDays { get; set; }
        public string Days { get; set; }
        public string AccountLevels { get; set; }

        int IKeySetter.Id
        {
            get { return PointGiftId; }
            set { PointGiftId = value; }
        }
    }
}