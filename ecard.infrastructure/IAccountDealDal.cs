using System;
using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IAccountDealDal
    {
        PosAndShop GetPosAndShopByName(string posName, string shopName);
        Account GetAccountByName(string accountName);
        int Save(DealLog dealLog);
        int Save(Account account);
        DealLog GetDealLog(string posName, string shopName, string serialNo);
        DealLog GetDealLog(int dealLogId);
        DealLog GetDealLog(int dealType, int addIn);
        PrePay GetPrePay(int posEndPointId, int accountId, int state);
        int Save(PrePay prePay);
        void Delete(PrePay prePay);
        PrePay GetPrePay(int id);
        IEnumerable<PointPolicy> GetPointPolicies();
        IEnumerable<AmountRate> GetAmountRate();
        Site GetSite();
        IEnumerable<AccountLevelPolicy> GetAccountLevelPolicies();
        IEnumerable<AccountType> GetAccounTypes();
        AccountUser GetUserById(Account account);
        Shop GetShop(string shopToName);
        Shop GetShop(int shopId);
        ShopUser GetShopUser(Shop shop);
        void Save(ShopDealLog shopDealLog);
        void Save(Shop shop);
        ShopDealLog GetShopDealLogByAddin(int addin);
        DealWay GetDealWay(int dealWayId);
        void Save(SystemDealLog systemDealLog);
        AdminUser GetUserByName(string userName);
        void UpdatePosOwnerId(int posEndPointId, int userId);
        void DeleteUser(int userId);
        void SystemDealLogSave(SystemDealLog item);
    }
    public class PosAndShop
    {
        public PosEndPoint  PosEndPoint  { get; set; }
        public Shop Shop { get; set; }
    }

    public class PointAmount
    {
        /// <summary>
        /// 本次消费积分
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// 本次消费折扣后金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 本次消费折扣
        /// </summary>
        public decimal DiscountRate { get; set; }
    }
}