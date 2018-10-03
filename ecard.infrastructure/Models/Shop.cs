using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a Shop.
    /// </summary>
    public class Shop : INamedEntity, IRecordVersion
    {
        public Shop()
        {
            State = States.Normal;
            ShopType = ShopTypes.Normal;
        }

        [Bounded(typeof(ShopStates))]
        public int State { get; set; }

        [Key]
        public int ShopId { get; set; }
        public string Bank { get; set; }
        /// <summary>
        /// 银行二级网点
        /// </summary>
        public string BankPoint { get; set; }

        /// <summary>
        /// 商户余额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 商户手续费支出
        /// </summary>
        public decimal RechargingAmount { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话1
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 电话2
        /// </summary>
        public string PhoneNumber2 { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 电话号码2
        /// </summary>
        public string Mobile2 { get; set; }
        /// <summary>
        /// 清算手续比例
        /// </summary>
        public decimal? ShopDealLogChargeRate { get; set; }

        public bool EnableShopMakeCard { get; set; }
        /// <summary>
        /// 商户可充值额度
        /// </summary>
        public decimal RechargeAmount { get; set; }


        #region INamedEntity Members

        public string Name { get; set; }
        public string DisplayName { get; set; }

        public int ShopType { get; set; }
        /// <summary>
        /// 内置
        /// </summary>
        public bool BuildIn { get; set; }

        #endregion

        public int RecordVersion { get; set; }

        public int DealWayId { get; set; }
        public string Description { get; set; }
        public string BankAccountName { get; set; }
        public string BankUserName { get; set; }

        public string FormatedName
        {
            get { return string.Format("{0} - {1}", this.Name, this.DisplayName); }
        }

        public static Shop Default
        {
            get { return new Shop {DisplayName = "系统总部"}; } 
        }
    }
}