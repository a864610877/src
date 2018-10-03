using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// ������.��Ӧ����Distributors��
    /// </summary>
    public class Distributor : INamedEntity, IRecordVersion
    {
        /// <summary>
        ///  State = States.Normal;
        /// </summary>
        public Distributor()
        {
            State = States.Normal;
        }
        /// <summary>
        /// ������״̬
        /// </summary>

        [Bounded(typeof(DistributorStates))]
        public int State { get; set; }

        [Key]
        public int DistributorId { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// ���������
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// �ϼ�
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// �����̼���
        /// </summary>
        public int DistributorLevel { get; set; }
        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public int DealWayId  { get; set; }
        /// <summary>
        /// �����˻�������
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// ֧������
        /// </summary>
        public string BankPoint  { get; set; }
        /// <summary>
        /// �����˻� 
        /// </summary>
        public string BankAccountName  { get; set; }
        /// <summary>
        /// �����˻�����
        /// </summary>
        public string BankUserName { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Description { get; set; }
        #region IRecordVersion Members
        /// <summary>
        /// 
        /// </summary>
        public int RecordVersion { get; set; }

        #endregion

        public string Name
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }
        public string FormatedName
        {
            get { return string.Format("{0} - {1}", this.Name, this.DisplayName); }
        }

        public static Distributor Default
        {
            get { return new Distributor { DisplayName = "ϵͳ�ܲ�" }; }
        }
    }
}