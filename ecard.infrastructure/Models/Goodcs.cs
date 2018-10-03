using System;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;

namespace Ecard.Models
{
   public  class Good
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
       public int GoodId { get; set; }
        /// <summary>
        /// 
        /// </summary>
       public string GoodName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Bounded(typeof(GoodState))]
       public int State { get; set; }
    }
}
