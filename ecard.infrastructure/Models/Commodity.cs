using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class Commodity
    {
        [Key]
        public int CommodityId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public decimal Price { get; set; }
        [Bounded(typeof(CommodityStates))]
        public int State { get; set; }
    }
    public class CommodityStates : States
    {

    }
}