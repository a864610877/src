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
        /// ���ж�������
        /// </summary>
        public string BankPoint { get; set; }

        /// <summary>
        /// �̻����
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// �̻�������֧��
        /// </summary>
        public decimal RechargingAmount { get; set; }

        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// �绰1
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// �绰2
        /// </summary>
        public string PhoneNumber2 { get; set; }
        /// <summary>
        /// �绰����
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// �绰����2
        /// </summary>
        public string Mobile2 { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        public decimal? ShopDealLogChargeRate { get; set; }

        public bool EnableShopMakeCard { get; set; }
        /// <summary>
        /// �̻��ɳ�ֵ���
        /// </summary>
        public decimal RechargeAmount { get; set; }


        #region INamedEntity Members

        public string Name { get; set; }
        public string DisplayName { get; set; }

        public int ShopType { get; set; }
        /// <summary>
        /// ����
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
            get { return new Shop {DisplayName = "ϵͳ�ܲ�"}; } 
        }
    }
}