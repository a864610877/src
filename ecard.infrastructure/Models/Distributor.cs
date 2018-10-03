using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// 经销商.对应的是Distributors表
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
        /// 经销商状态
        /// </summary>

        [Bounded(typeof(DistributorStates))]
        public int State { get; set; }

        [Key]
        public int DistributorId { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// 经销商余额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 上级
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 经销商级别
        /// </summary>
        public int DistributorLevel { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public int DealWayId  { get; set; }
        /// <summary>
        /// 银行账户开户行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 支行名称
        /// </summary>
        public string BankPoint  { get; set; }
        /// <summary>
        /// 银行账户 
        /// </summary>
        public string BankAccountName  { get; set; }
        /// <summary>
        /// 银行账户姓名
        /// </summary>
        public string BankUserName { get; set; }
        /// <summary>
        /// 描述
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
            get { return new Distributor { DisplayName = "系统总部" }; }
        }
    }
}