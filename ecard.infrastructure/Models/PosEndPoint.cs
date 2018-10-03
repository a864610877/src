using System.ComponentModel.DataAnnotations;
using Oxite.Model;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a Po.
    /// </summary>
    public class PosEndPoint : INamedEntity, ITokenKey, IRecordVersion
    {
        public PosEndPoint()
        {
            State = States.Normal;
            DataKey = "11111111";
            this.GenTokenKey();
        }

        [Key]
        public int PosEndPointId { get; set; }
        public int CurrentUserId { get; set; }

        public int ShopId { get; set; }

        [Bounded(typeof(PosEndPointStates))]
        public int State { get; set; }

        /// <summary>
        /// Ωª“◊√‹‘ø
        /// </summary>
        public string DataKey { get; set; }

        #region INamedEntity Members

        public string Name { get; set; }

        public string DisplayName { get; set; }

        #endregion

        #region ITokenKey Members

        /// <summary>
        /// Œ¥”√
        /// </summary>
        public string TokenKey { get; set; }

        #endregion

        public int RecordVersion { get; set; }
    }
}