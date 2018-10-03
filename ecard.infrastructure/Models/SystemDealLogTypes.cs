namespace Ecard.Models
{
    public class SystemDealLogTypes
    {

        /// <summary>
        /// 售卡
        /// </summary>
        public const int SaldCard = 1;

        /// <summary>
        /// 售卡手续费
        /// </summary>
        public const int SaldCardFee = 2;
        /// <summary>
        /// 充值
        /// </summary>
        public const int Recharge = 3;
        /// <summary>
        /// 储值卡押金
        /// </summary>
        public const int Deposit = 4;
        ///// <summary>
        ///// 储值卡售卡
        ///// </summary>
        //public const int SaldPetCard = 5;
        /// <summary>
        /// 清算支付
        /// </summary>
        public const int ShopDealLogDone = 6;
        /// <summary>
        /// 换卡
        /// </summary>
        public const int ChangeCard = 7;
        /// <summary>
        /// 清算支付
        /// </summary>
        public const int ShopDealLogCharging= 8;
        ///// <summary>
        ///// 会员消费手续费
        ///// </summary>
        //public const int AccountDealCharge = 9;
        /// <summary>
        /// 退卡
        /// </summary>
        public const int CloseCard = 11;
        /// <summary>
        /// 退储值卡押金
        /// </summary>
        public const int CloseDeposit = 14;
        /// <summary>
        /// 返利
        /// </summary>
        public const int Rebate = 15;
    }
}