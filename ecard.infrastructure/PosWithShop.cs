using System.Runtime.Serialization;

namespace Ecard.Infrastructure
{
    [DataContract]
    public class PosWithShop
    {
        /// <summary>
        /// 8位二进制密钥，用字符串表示，比如 3131313131313131
        /// </summary>
        [DataMember]
        public string DataKey { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }
        [DataMember]
        public bool Authenticated{ get; set; }
    }
}