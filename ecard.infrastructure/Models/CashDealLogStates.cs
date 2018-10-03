namespace Ecard.Models
{
    public class CashDealLogSummaryStates
    {
        /// <summary>
        /// 正常的
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// 无效的，有故障的, 黑名单的, 暂时停使用的
        /// </summary>
        public const int Invalid = 2;
        /// <summary>
        /// 全部
        /// </summary>
        public const int All = 100000;
    }
    public class CashDealLogStates : States
    {
    }
}