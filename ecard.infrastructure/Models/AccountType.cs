using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class AccountType
    {
        [Key]
        public int AccountTypeId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 能否充值
        /// </summary>
        public bool IsRecharging { get; set; }
        /// <summary>
        /// 购买金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 初始积分
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public int ExpiredMonths { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// 每次可带小孩数
        /// </summary>
        public int NumberOfPeople { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }


        /// <summary>
        /// 是否积分
        /// </summary>
        public bool IsPointable{ get; set; }

        /// <summary>
        /// 每次续卡月份
        /// </summary>
        public int RenewMonths { get; set; }
        /// <summary>
        /// 能否续卡
        /// </summary>
        public bool IsRenew { get; set; }
        /// <summary>
        /// 押金
        /// </summary>
        public decimal DepositAmount { get; set; }
        /// <summary>
        /// 是否发送短信
        /// </summary>
        public bool IsMessageOfDeal { get; set; }


        [Bounded(typeof(AccountTypeStates))]
        public int State { get; set; }
        /// <summary>
        /// 是否发送交易短信
        /// </summary>
        public bool IsSmsDeal { get; set; }
        /// <summary>
        /// 是否发送会员生日短信
        /// </summary>
        public bool IsSmsAccountBirthday { get; set; }
        /// <summary>
        /// 是否发送充值短信
        /// </summary>
        public bool IsSmsRecharge { get; set; }
        /// <summary>
        /// 是否发送转账短信
        /// </summary>
        public bool IsSmsTransfer { get; set; }
        /// <summary>
        /// 是否发送卡停用短信
        /// </summary>
        public bool IsSmsSuspend { get; set; }
        /// <summary>
        /// 是否发送卡启用短信
        /// </summary>
        public bool IsSmsResume { get; set; }
        /// <summary>
        /// 是否发送卡延期短信
        /// </summary>
        public bool IsSmsRenew { get; set; }
        /// <summary>
        /// 是否发送退卡短信
        /// </summary>
        public bool IsSmsClose { get; set; }
        /// <summary>
        /// 是否发送验证码短信
        /// </summary>
        public bool IsSmsCode { get; set; }
        /// <summary>
        /// 是否返送换卡短信
        /// </summary>
        public bool IsSmsChangeName { get; set; }
    }
}
