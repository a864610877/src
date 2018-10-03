namespace Ecard.Models
{
    public class DealTypes
    {

        /// <summary>
        /// ���ף��ֽ𿨽��׿ۿ���
        /// </summary>
        public const int Deal = 1;
        /// <summary>
        /// �������ֽ𿨽��׳�����
        /// </summary>
        public const int CancelDeal = 2;
        /// <summary>
        /// Ԥ��Ȩ
        /// </summary>
        public const int PrePay = 3;
        /// <summary>
        /// ����
        /// </summary>
        public const int DonePrePay = 4;
        /// <summary>
        /// ����Ԥ��Ȩ
        /// </summary>
        public const int CancelPreDeal = 5;
        /// <summary>
        /// ����
        /// </summary>
        public const int CancelDonePrePay = 6;
        /// <summary>
        /// ����
        /// </summary>
        public const int Invalid = 7;
        /// <summary>
        /// ��Ա�����ף�����������֣�
        /// </summary>
        public const int Integral = 8;
        /// <summary>
        /// ������Ա������
        /// </summary>
        public const int CancelIntegral = 9;
        /// <summary>
        ///���ֽ���
        /// </summary>
        public const int PayIntegral = 10;
        /// <summary>
        ///�������ֽ���
        /// </summary>
        public const int CancelPayIntegral = 11;
        /// <summary>
        /// ��Ա����
        /// </summary>
        public const int PointRebate = 100;
        /// <summary>
        /// ��Ϣ
        /// </summary>
        public const int InterestSettlement = 101;
        /// <summary>
        /// ��ֵ
        /// </summary>
        public const int Recharging = 102;
        /// <summary>
        /// ����
        /// </summary>
        public const int Open = 103;
        /// <summary>
        /// �˿�
        /// </summary>
        public const int Close = 104;
        /// <summary>
        /// ȡ����ֵ����
        /// </summary>
        public const int CancelRecharging = 105;
        /// <summary>
        /// ת��ת��
        /// </summary>
        public const int TransferIn = 106;
        /// <summary>
        /// ת��ת��
        /// </summary>
        public const int TransferOut = 107;
        /// <summary>
        /// ������
        /// </summary>
        public const int DealCharge = 108;
        /// <summary>
        /// ����
        /// </summary>
        public const int Rebate = 109;
        /// <summary>
        /// �ҽ�
        /// </summary>
        public const int Gift = 110;


        public const int ShopDealLogDone = 1000;
        public const int ShopDealLogCharging = 10010;
        /// <summary>
        /// ϵͳ�۷�
        /// </summary>
        public const int Receivable = 10000;

    }
}