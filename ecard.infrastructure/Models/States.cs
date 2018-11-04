namespace Ecard.Models
{
    public class States
    { 
        /// <summary>
        /// 正常的 1
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// 无效的，有故障的, 黑名单的, 暂时停使用的 2
        /// </summary>
        public const int Invalid = 2;
        /// <summary>
        /// 已使用完成
        /// </summary>
        public const int UseComplete = 3;

        /// <summary>
        /// 全部 100000
        /// </summary>
        public const int All = 100000;
    }
}
