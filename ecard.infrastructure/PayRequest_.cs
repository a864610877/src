using System.Runtime.Serialization;

namespace Ecard.Infrastructure
{
    /// <summary>
    /// 冲正请求
    /// </summary>
    [DataContract]
    public class PayRequest_
    {
        public PayRequest_(string accountName, string password, string posName, decimal amount, string serialNo, string oldSerialNo, string userToken, string shopName)
        {
            AccountName = accountName;
            Password = password;
            PosName = posName;
            Amount = amount;
            OldSerialNo = oldSerialNo;
            SerialNo = serialNo;
            UserToken = userToken;
            ShopName = shopName;
        }
        /// <summary>
        /// 会员卡号
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }
        /// <summary>
        /// 输入密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 发起方终端号
        /// </summary>
        [DataMember]
        public string PosName { get; set; }
        /// <summary>
        /// 操作金额
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }
        /// <summary>
        /// 上一次的流水号
        /// </summary>
        [DataMember]
        public string OldSerialNo { get; set; }
        /// <summary>
        /// 本次流水号
        /// </summary>
        [DataMember]
        public string SerialNo { get; set; }
        /// <summary>
        /// 会员卡暗码
        /// </summary>
        [DataMember]
        public string UserToken { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }
        /// <summary>
        /// 是否系统发起的强制操作（可不考虑）
        /// </summary>
        [DataMember]
        public bool IsForce { get; set; }
    }
}