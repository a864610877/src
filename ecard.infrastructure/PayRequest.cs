using System.Runtime.Serialization;

namespace Ecard
{
    /// <summary>
    /// 交易请求
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
        /// 会员卡号
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }
        /// <summary>
        /// 会员密码
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
        /// 流水号
        /// </summary>
        [DataMember]
        public string SeriaNo { get; set; }
        /// <summary>
        /// 会员卡暗码
        /// </summary>
        [DataMember]
        public string UserToken { get; set; }
        /// <summary>
        /// 发起方商户编号
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }
        /// <summary>
        /// 是否系统强制发起（可不考虑）
        /// </summary>
        [DataMember]
        public bool IsForce { get; set; }
        /// <summary>
        /// 交易金额接受方
        /// </summary>
        [DataMember]
        public string ShopNameTo { get; set; }

        public string Operator { get; set; }
    }
}