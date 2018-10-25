using System;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;

namespace Ecard.Models
{
    public class MobileStates
    {
        public const int None = 1;
        public const int IsAvailable = 2;
        public const int IsUnavailable = 3;
    }
    public class SiteAccount : IRecordVersion
    {
        static SiteAccount()
        {
            Locker = new object();
        }

        public static readonly object Locker;
        [Key]
        public int SiteAccountId { get; set; }
        public DateTime SubmitTime { get; set; }

        public int RecordVersion { get; set; }
        public decimal Amount { get; set; }
    }
    public class Account : IRecordVersion
    {
        public Account()
        {
            ExpiredDate = DateTime.Now;
            LastDealTime = DateTime.Now;
            State = States.Normal;
            AccountToken = "11111111";
        }

        [Key]
        public int AccountId { get; set; }

        public int? OwnerId { get; set; }

        /// <summary>
        /// 当前金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 信用额度
        /// </summary>
        public decimal LimiteAmount { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        public decimal FreezeAmount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount
        {
            get { return Amount + FreezeAmount; }
        }

        /// <summary>
        /// 有效月份
        /// </summary>
        public int ExpiredMonths { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// 剩余使用次数
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// 已使用次数
        /// </summary>
        public int FrequencyUsed { get; set; }
        public DateTime LastDealTime { get; set; }
        public DateTime? OpenTime { get; set; }
        [Bounded(typeof(AccountStates))]
        public int State { get; set; }
        /// <summary>
        /// 是谁卖的这张卡（指营销人员）
        /// </summary>
        public int SalerId { get; set; }
        public int AccountTypeId { get; set; }
        public int ShopId { get; set; }
        public decimal PayAmount { get; set; }
        public int DistributorId { get; set; }
        /// <summary>
        /// 押金
        /// </summary>
        public decimal DepositAmount { get; set; }
        public string AccountToken { get; set; }
        public int Point { get; set; }
        public int TotalPoint { get; set; }
        public int AccountLevel { get; set; }
        public decimal ChargingAmount { get; set; }

        [Timestamp]
        public int RecordVersion { get; set; }

        public void SetPassword(string password)
        {
            this.PasswordSalt = Guid.NewGuid().ToString("N").Substring(0, 8);
            this.Password = User.SaltAndHash(password, PasswordSalt);
        }
        public string AccountLevelName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string InitPassword { get; set; }
        public string PasswordSalt { get; set; }
        /// <summary>
        /// 是否发送短信
        /// </summary>
        public bool IsMessageOfDeal { get; set; }

        public string Remark1 { get; set; }

        public string useScope { get; set; }


        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal SaleAmount { get; set; }
        /// <summary>
        /// 总次数
        /// </summary>
        public int TotalTimes { get; set; }
        /// <summary>
        /// 单次价格
        /// </summary>
        public decimal SinglePrice { get; set; }

        /// <summary>
        /// 现金交易，不需要从账户中扣减余额，增加消费总额，增加积分。
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public DealResult IntegralPay(decimal amount, int point)
        {
            //Amount -= (amount);
            PayAmount += (amount);
            Point += point;
            TotalPoint += point;
            LastDealTime = DateTime.Now;
            return new DealResult() { Amount = (amount), Point = point };
        }
        /// <summary>
        /// 积分交易，扣减积分
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public DealResult PayIntegralPay(decimal amount)
        {
            //Amount -= (amount);
            //PayAmount += (amount);
            Point -= (int)amount;
            TotalPoint -= (int)amount;
            LastDealTime = DateTime.Now;
            return new DealResult() { Amount = (amount), Point = (int)amount };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public DealResult Pay(decimal amount, int point)
        {
            Amount -= (amount);
            PayAmount += (amount);
            Point += point;
            TotalPoint += point;
            LastDealTime = DateTime.Now;
            return new DealResult() { Amount = (amount), Point = point };
        }

        public void PayCharge(decimal charge)
        {
            Amount -= (charge);
            PayAmount += (charge);
            ChargingAmount += charge;
            LastDealTime = DateTime.Now;
        }
        public void CancelPayCharge(decimal charge)
        {
            Amount += (charge);
            PayAmount -= (charge);
            ChargingAmount -= charge;
            LastDealTime = DateTime.Now;
            if (ChargingAmount < 0)
                throw new Exception("the ChargingAmount is less then zero!");
        }

        public void PayCancel(decimal amount, int point)
        {
            Amount += (amount);
            PayAmount -= (amount);
            Point -= point;
            TotalPoint -= point;
            LastDealTime = DateTime.Now;
        }

        public void RechargeCancel(decimal amount)
        {
            Amount -= (amount);
            LastDealTime = DateTime.Now;
        }

        public void PrePay(decimal amount)
        {
            Amount -= amount;
            FreezeAmount += amount;
            LastDealTime = DateTime.Now;
        }

        public void CancelPrePay(decimal amount)
        {
            Amount += amount;
            FreezeAmount -= amount;
            LastDealTime = DateTime.Now;
        }

        public void DonePrePay(PrePay prePay)
        {
            Amount += prePay.Amount;
            FreezeAmount -= prePay.Amount;
            LastDealTime = DateTime.Now;
        }

        public void CancelDonePrePay(PrePay prePay)
        {
            Amount -= prePay.Amount;
            FreezeAmount += prePay.Amount;
            LastDealTime = DateTime.Now;
        }

        public bool HasEnoughAmount(decimal amount)
        {
            return Amount - amount >= -LimiteAmount;
        }

        public void PayIntegralCancel(int point)
        {
            Point += point;
            TotalPoint += point;
            LastDealTime = DateTime.Now;
            //throw new NotImplementedException();
        }

        public void IntegralCancel(decimal amount, int point)
        {
            PayAmount -= (amount);
            Point -= point;
            TotalPoint -= point;
            LastDealTime = DateTime.Now;
        }

        public void Recharge(decimal amount)
        {
            Amount += amount;
            LastDealTime = DateTime.Now;
        }
    }

    public interface IRecordVersion
    {
        int RecordVersion { get; set; }
    }

    public class AccountWithOwner : Account
    {
        public string OwnerDisplayName { get; set; }
        public string OwnerMobileNumber { get; set; }
        public string BabyName { get; set; }
        public int BabySex { get; set; }
        public DateTime babyBirthDate { get; set; }
        public string AccountTypeName { get; set; }
        public string shopName { get; set; }
        
    }

    public class ShopWithOwner : Shop
    {
        public string OwnerDisplayName { get; set; }
        public string OwnerMobileNumber { get; set; }
    }
}