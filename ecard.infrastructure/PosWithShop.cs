using System.Runtime.Serialization;

namespace Ecard.Infrastructure
{
    [DataContract]
    public class PosWithShop
    {
        /// <summary>
        /// 8λ��������Կ�����ַ�����ʾ������ 3131313131313131
        /// </summary>
        [DataMember]
        public string DataKey { get; set; }
        /// <summary>
        /// �̻�����
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }
        [DataMember]
        public bool Authenticated{ get; set; }
    }
}