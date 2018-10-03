namespace Ecard.Models
{
    public class DealTypes
    {

        /// <summary>
        /// 交易（现金卡交易扣卡余额）
        /// </summary>
        public const int Deal = 1;
        /// <summary>
        /// 撤消（现金卡交易撤销）
        /// </summary>
        public const int CancelDeal = 2;
        /// <summary>
        /// 预授权
        /// </summary>
        public const int PrePay = 3;
        /// <summary>
        /// 交易
        /// </summary>
        public const int DonePrePay = 4;
        /// <summary>
        /// 撤销预授权
        /// </summary>
        public const int CancelPreDeal = 5;
        /// <summary>
        /// 撤消
        /// </summary>
        public const int CancelDonePrePay = 6;
        /// <summary>
        /// 冲正
        /// </summary>
        public const int Invalid = 7;
        /// <summary>
        /// 会员卡交易（输入金额积积分）
        /// </summary>
        public const int Integral = 8;
        /// <summary>
        /// 撤销会员卡交易
        /// </summary>
        public const int CancelIntegral = 9;
        /// <summary>
        ///积分交易
        /// </summary>
        public const int PayIntegral = 10;
        /// <summary>
        ///撤销积分交易
        /// </summary>
        public const int CancelPayIntegral = 11;
        /// <summary>
        /// 会员返利
        /// </summary>
        public const int PointRebate = 100;
        /// <summary>
        /// 利息
        /// </summary>
        public const int InterestSettlement = 101;
        /// <summary>
        /// 充值
        /// </summary>
        public const int Recharging = 102;
        /// <summary>
        /// 开户
        /// </summary>
        public const int Open = 103;
        /// <summary>
        /// 退卡
        /// </summary>
        public const int Close = 104;
        /// <summary>
        /// 取消充值（）
        /// </summary>
        public const int CancelRecharging = 105;
        /// <summary>
        /// 转帐转入
        /// </summary>
        public const int TransferIn = 106;
        /// <summary>
        /// 转帐转出
        /// </summary>
        public const int TransferOut = 107;
        /// <summary>
        /// 手续费
        /// </summary>
        public const int DealCharge = 108;
        /// <summary>
        /// 返利
        /// </summary>
        public const int Rebate = 109;
        /// <summary>
        /// 兑奖
        /// </summary>
        public const int Gift = 110;


        public const int ShopDealLogDone = 1000;
        public const int ShopDealLogCharging = 10010;
        /// <summary>
        /// 系统扣费
        /// </summary>
        public const int Receivable = 10000;

    }
}