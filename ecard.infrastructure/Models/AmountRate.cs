using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class TokenKeyTypes
    {
        public const int RecoveryPassword = 1;
    }
    public class TemporaryTokenKey
    {
        [Key]
        public int TemporaryTokenKeyId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int TokenKeyType { get; set; }
    }
    public class AmountRate : IKeySetter
    {
        [Key]
        public int AmountRateId { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// ӳ�䵽 account level policy id
        /// </summary>
        public int AccountLevel { get; set; }
        public int Days { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        [Bounded(typeof(AmountRateStates))]
        public int State { get; set; }

        int IKeySetter.Id
        {
            get
            {
                return AmountRateId;
            }
            set { AmountRateId = value; }
        }
    }
    public class AccountDependencyTypes
    {
        public const int Manunal = 0;
        public const int BirthDate = 1;
        public const int Weekly = 2;
        public const int Day = 4;
        public const int EveryDay = 8;
    }
    public interface IAccountDependency
    {
        int DependencyType { get; set; }
        string WeekDays { get; set; }
        string Days { get; set; }
        string AccountLevels { get; set; }
    }
}