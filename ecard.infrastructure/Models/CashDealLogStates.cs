namespace Ecard.Models
{
    public class CashDealLogSummaryStates
    {
        /// <summary>
        /// ������
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// ��Ч�ģ��й��ϵ�, ��������, ��ʱͣʹ�õ�
        /// </summary>
        public const int Invalid = 2;
        /// <summary>
        /// ȫ��
        /// </summary>
        public const int All = 100000;
    }
    public class CashDealLogStates : States
    {
    }
}