namespace Ecard.Infrastructure
{
    [Bounded(typeof(ResponseCode))]
    public class ResponseCode
    {

        /// <summary>
        /// 无效商户
        /// </summary>
        public const int InvalidateShop = 0x3;
        /// <summary>
        /// 无效POS
        /// </summary>
        public const int InvalidatePos = 0x97;
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 0x0;
        /// <summary>
        /// 需要重新签到
        /// </summary>
        public const int NeedSignIn = 0x77;
        /// <summary>
        /// 无此帐户
        /// </summary>
        public const int NonFoundAccount = 0x42;
        /// <summary>
        /// 过期卡
        /// </summary>
        public const int ValidityDate = 0x54;
        /// <summary>
        /// 密码错误 
        /// </summary>
        public const int InvalidatePassword = 0x55;
        /// <summary>
        /// Cracker 
        /// </summary>
        public const int Cracker = 0x59;
        /// <summary>
        /// Mac 错
        /// </summary>
        public const int MacError = 0xA0;
        /// <summary>
        /// 无效金额
        /// </summary>
        public const int InvalidateAmount = 0x13;
        /// <summary>
        /// 无消费记录
        /// </summary>
        public const int NonFoundDeal = 0x25;
        /// <summary>
        /// 系统异常
        /// </summary>
        public const int SystemError = 0x96;



        /// <summary>
        /// 此卡不能充值
        /// </summary>
        public const int NonRecharging = 0x10042;
        /// <summary>
        /// 此卡不能续卡
        /// </summary>
        public const int NonRenewal = 0x10043;
        /// <summary>
        /// 卡已停用
        /// </summary>
        public const int AccountStateInvalid = 0x10044;
        /// <summary>
        /// 卡出现数据冲突 
        /// </summary>
        public const int AccountConflict = 0x10045;
        /// <summary>
        /// 支付渠道不存在
        /// </summary>
        public const int InvalidateDealWay = 0x10046;
        /// <summary>
        /// 无效用户
        /// </summary>
        public const int InvalidateUser = 0x10047;
        /// <summary>
        /// 无效命令
        /// </summary>
        public const int InvalidateCommandType = 0x10048;
    }
}