using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class DealWay : IKeySetter
    {
        [Key]
        public int DealWayId { get; set; }
        public string DisplayName { get; set; }
        public int ApplyTo { get; set; } 
        [Bounded(typeof(DealWayStates))]
        public int State { get; set; }
        /// <summary>
        /// ÊÇ·ñÏÖ½ð
        /// </summary>
        public bool IsCash { get; set; }

        int IKeySetter.Id
        {
            get { return DealWayId; }
            set { DealWayId = value; }
        }
    }
}