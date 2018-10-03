using System.Runtime.Serialization;

namespace Ecard
{
    /// <summary>
    /// ��������
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    [DataContract]
    public class PayRequest
    {
        public PayRequest(string accountName, string password, string posName, decimal amount, string seriaNo, string userToken, string shopName, string shopNameTo)
        {
            AccountName = accountName;
            Password = password;
            PosName = posName;
            Amount = amount;
            SeriaNo = seriaNo;
            UserToken = userToken;
            ShopName = shopName;
            ShopNameTo = (shopNameTo??"").Trim();
        }
        /// <summary>
        /// ��Ա����
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }
        /// <summary>
        /// ��Ա����
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// �����ն˺�
        /// </summary>
        [DataMember]
        public string PosName { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }
        /// <summary>
        /// ��ˮ��
        /// </summary>
        [DataMember]
        public string SeriaNo { get; set; }
        /// <summary>
        /// ��Ա������
        /// </summary>
        [DataMember]
        public string UserToken { get; set; }
        /// <summary>
        /// �����̻����
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }
        /// <summary>
        /// �Ƿ�ϵͳǿ�Ʒ��𣨿ɲ����ǣ�
        /// </summary>
        [DataMember]
        public bool IsForce { get; set; }
        /// <summary>
        /// ���׽����ܷ�
        /// </summary>
        [DataMember]
        public string ShopNameTo { get; set; }

        public string Operator { get; set; }
    }
}