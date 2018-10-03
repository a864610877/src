using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit;
using Moonlit.Data;

namespace Ecard.Services
{
    public class CachedSqlAccountDealDal : IAccountDealDal
    {
        private readonly DatabaseInstance _databaseInstance;
        private static object PosAndShopsLocker = new object();
        private static object PointPoliciesLocker = new object();
        private static object AmountRateLocker = new object();
        private static object SiteLocker = new object();

        public CachedSqlAccountDealDal(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public IEnumerable<PointPolicy> GetPointPolicies()
        {
            string key = CacheKeys.PointPolicyKey;
            var items = (Point_Level_Type)CachePools.GetData(key);
            if (items == null)
            {
                lock (PointPoliciesLocker)
                {
                    items = (Point_Level_Type)CachePools.GetData(key);
                    if (items == null)
                    {
                        items = InnerGetPointPolicies();
                        CachePools.AddCache(key, items);
                    }
                }
            }
            return items.PointPolicies;
        }
        public IEnumerable<AccountLevelPolicy> GetAccountLevelPolicies()
        {
            string key = CacheKeys.PointPolicyKey;
            var items = (Point_Level_Type)CachePools.GetData(key);
            if (items == null)
            {
                lock (PointPoliciesLocker)
                {
                    items = (Point_Level_Type)CachePools.GetData(key);
                    if (items == null)
                    {
                        items = InnerGetPointPolicies();
                        CachePools.AddCache(key, items);
                    }
                }
            }
            return items.AccountLevelPolicies;
        }

        public IEnumerable<AccountType> GetAccounTypes()
        {
            string key = CacheKeys.PointPolicyKey;
            var items = (Point_Level_Type)CachePools.GetData(key);
            if (items == null)
            {
                lock (PointPoliciesLocker)
                {
                    items = (Point_Level_Type)CachePools.GetData(key);
                    if (items == null)
                    {
                        items = InnerGetPointPolicies();
                        CachePools.AddCache(key, items);
                    }
                }
            }
            return items.AccountTypes;
        }

        public IEnumerable<AmountRate> GetAmountRate()
        {
            string key = CacheKeys.AmountRateKey;

            List<AmountRate> items = (List<AmountRate>)CachePools.GetData(key);
            if (items == null)
            {
                lock (AmountRateLocker)
                {
                    items = (List<AmountRate>)CachePools.GetData(key);
                    if (items == null)
                    {
                        items = new List<AmountRate>(InnerGetAmountRates());
                        CachePools.AddCache(key, items);
                    }
                }
            }
            return items.AsEnumerable();
        }

        public Site GetSite()
        {
            string key = CacheKeys.SiteKey;

            Site site = (Site)CachePools.GetData(key);
            if (site == null)
            {
                lock (SiteLocker)
                {
                    site = (Site)CachePools.GetData(key);
                    if (site == null)
                    {
                        site = InnerGetSite();
                        CachePools.AddCache(key, site);
                    }
                }
            }
            return site;
        }

        public Site InnerGetSite()
        {
            string sql = @"select t.* from sites t";

            return _databaseInstance.Query<Site>(sql, null).FirstOrDefault();
        }


        Point_Level_Type InnerGetPointPolicies()
        {
            Point_Level_Type r = new Point_Level_Type();
            r.PointPolicies = _databaseInstance.Query<PointPolicy>(@"select policy.* from PointPolicies policy where state = 1", null).ToList();
            r.AccountTypes = _databaseInstance.Query<AccountType>(@"select * from AccountTypes   where state = 1", null).ToList();
            r.AccountLevelPolicies = _databaseInstance.Query<AccountLevelPolicy>(@"select * from AccountLevelPolicies   where state = 1", null).ToList();
            return r;
        }
        IEnumerable<AmountRate> InnerGetAmountRates()
        {
            string sql = @"select t.* from AmountRates t where state = 1";
            return _databaseInstance.Query<AmountRate>(sql, null);
        }


        public PosAndShop GetPosAndShopByName(string posName, string shopName)
        {
            var shop = _databaseInstance.Query<Shop>(@"select * from shops  where state = 1 and name = @name", new { name = shopName }).FirstOrDefault();
            if (shop == null) return null;
            var poses = EnsureShopAndPos();

            var pos = poses.FirstOrDefault(x => string.Equals(posName, x.Name, StringComparison.OrdinalIgnoreCase) && x.ShopId == shop.ShopId);
            if (pos != null)
            {
                return new PosAndShop { Shop = shop, PosEndPoint = pos };
            }
            return null;
        }

        private List<PosEndPoint> GetPosAndShops()
        {
            List<PosEndPoint> items = _databaseInstance.Query<PosEndPoint>(@"select	* from PosEndPoints where state = 1", null).ToList();
            return items;
        }

        class Point_Level_Type
        {
            internal List<PointPolicy> PointPolicies { get; set; }
            internal List<AccountLevelPolicy> AccountLevelPolicies { get; set; }
            internal List<AccountType> AccountTypes { get; set; }
        }
        public int Save(PrePay prePay)
        {
            if (prePay.PrePayId != 0)
            {
                prePay.UpdateTime = DateTime.Now;
                return _databaseInstance.Update(prePay, "PrePays");
            }
            else
            {
                prePay.UpdateTime = prePay.SubmitTime;
                prePay.PrePayId = _databaseInstance.Insert(prePay, "PrePays");
                return prePay.PrePayId;
            }
        }

        public void Delete(PrePay prePay)
        {
            var sql = "delete prepays where prepayId = @prepayId ";
            _databaseInstance.ExecuteNonQuery(sql, new { prepayId = prePay.PrePayId });
        }

        //public DealLog GetDealItemWithAddin(int addInId, int dealType)
        //{
        //    const string sql = "SELECT * FROM dealLogs WHERE AddIn = @AddIn and dealType = @dealType and state = 1";
        //    return _databaseInstance.Query<DealLog>(sql, new { AddIn = addInId, dealType = dealType }).FirstOrDefault();
        //}

        public int Save(DealLog dealLog)
        {
            if (dealLog.DealLogId != 0)
            {
                return _databaseInstance.Update(dealLog, "DealLogs");
            }
            else
            {
                dealLog.DealLogId = _databaseInstance.Insert(dealLog, "DealLogs");
                return dealLog.DealLogId;
            }
        }

        public int Save(Account account)
        {
            return _databaseInstance.Update(account, "Accounts");
        }

        public DealLog GetDealLog(string posName, string shopName, string serialNo)
        {
            string sql = @"select deal.* from deallogs deal where (@posName is null or sourceposName = @posName) and sourceshopName = @shopName and serialNo = @serialNo";
            return _databaseInstance.Query<DealLog>(sql, new { posName = posName, shopName = shopName, serialNo = serialNo }).FirstOrDefault();
        }

        public DealLog GetDealLog(int dealLogId)
        {
            string sql = @"select deal.* from deallogs deal where dealLogId = @dealLogId";
            return _databaseInstance.Query<DealLog>(sql, new { dealLogId = dealLogId }).FirstOrDefault();
        }

        public DealLog GetDealLog(int dealType, int addIn)
        {
            string sql = @"select deal.* from deallogs deal where addin = @addin and dealType = @dealType";
            return _databaseInstance.Query<DealLog>(sql, new { addIn = addIn, dealType = dealType }).FirstOrDefault();
        }

        public PrePay GetPrePay(int posId, int accountId, int state)
        {
            // posid == 0 means is operation in system
            string sql = @"select pay.* from PrePays pay where (PosId = @PosId or @PosId = 0) and AccountId = @AccountId and state = @state";
            return _databaseInstance.Query<PrePay>(sql, new { posId = posId, accountId = accountId, state = state }).FirstOrDefault();
        }


        public PrePay GetPrePay(int id)
        {
            string sql = @"select pay.* from PrePays pay where PrePayId = @PrePayId";
            return _databaseInstance.Query<PrePay>(sql, new { PrePayId = id }).FirstOrDefault();
        }


        public AccountUser GetUserById(Account account)
        {
            if (account != null && account.OwnerId.HasValue)
                return GetUser(account.OwnerId.Value);
            return null;
        }

        public Shop GetShop(string shopName)
        {
            if (string.IsNullOrWhiteSpace(shopName))
            {
                return null;
            }
            return _databaseInstance.Query<Shop>("select * from shops where state = 1 and (Name = @name or PhoneNumber = @name or Mobile= @name)", new { name = shopName }).FirstOrDefault();
        }


        public Shop GetShop(int shopId)
        {
            if (shopId == 0)
                return Shop.Default;
            return _databaseInstance.Query<Shop>("select * from shops where state = 1 and ShopID = @shopId", new { shopId = shopId }).FirstOrDefault();
        }


        private List<PosEndPoint> EnsureShopAndPos()
        {
            var poses = (List<PosEndPoint>)CachePools.GetData(CacheKeys.PosKey);
            if (poses == null)
            {
                lock (PosAndShopsLocker)
                {
                    poses = (List<PosEndPoint>)CachePools.GetData(CacheKeys.PosKey);
                    if (poses == null)
                    {
                        poses = GetPosAndShops();
                        CachePools.AddCache(CacheKeys.PosKey, poses);
                    }
                }
            }
            return poses;
        }


        public Account GetAccountByName(string accountName)
        {
            string sql = @"select acc.* from accounts acc where Name = @name";
            return _databaseInstance.Query<Account>(sql, new { name = accountName }).FirstOrDefault();
        }

        public ShopUser GetShopUser(Shop shop)
        {
            string sql = @"select t.* from users t where t.ShopId = @ShopId and ShopRole = @shopRole";

            return _databaseInstance.Query<ShopUser>(sql, new { ShopId = shop.ShopId, shopRole = ShopRoles.Owner }).FirstOrDefault();
        }

        public void Save(ShopDealLog shopDealLog)
        {
            if (shopDealLog.ShopDealLogId > 0)
                _databaseInstance.Update(shopDealLog, "ShopDealLogs");
            else
                _databaseInstance.Insert(shopDealLog, "ShopDealLogs");
        }

        public void Save(Shop shop)
        {
            _databaseInstance.Update(shop, "Shops");
        }

        public ShopDealLog GetShopDealLogByAddin(int addin)
        {
            string sql = @"select t.* from ShopDealLogs t where t.Addin = " + addin;
            return _databaseInstance.Query<ShopDealLog>(sql, null).FirstOrDefault();
        }

        public DealWay GetDealWay(int dealWayId)
        {
            string sql = @"select t.* from DealWays t where t.DealWayid = " + dealWayId;
            return _databaseInstance.Query<DealWay>(sql, null).FirstOrDefault();
        }

        public void Save(SystemDealLog item)
        {
            lock (SiteAccount.Locker)
            {
                if (!_databaseInstance.InTransaction)
                    throw new Exception("SystemDealLog have to run in transaction");
                _databaseInstance.ExecuteNonQuery("update siteaccounts set amount = amount + @amount",
                                                  new { amount = item.Amount });
                var siteAccount = _databaseInstance.Query<SiteAccount>("select * from siteaccounts", null).FirstOrDefault();
                if (siteAccount == null)
                    throw new Exception("siteaccounts table changed, please check it");
                item.SiteAmount = siteAccount.Amount;
                item.SystemDealLogId = _databaseInstance.Insert(item, "SystemDealLogs");
            }
        }

        public AdminUser GetUserByName(string userName)
        {
            string sql = @"select t.* from users t where t.Name = @userName";
            return _databaseInstance.Query<AdminUser>(sql, new { userName = userName }).FirstOrDefault();
        }

        public void UpdatePosOwnerId(int posEndPointId, int userId)
        {
            string sql = @"update posEndPoints set CurrentUserId = @userId, recordversion = recordversion + 1 where posEndPointId = @posEndPointId";
            _databaseInstance.ExecuteNonQuery(sql, new { userId = userId, posEndPointId = posEndPointId });
        }

        private AccountUser GetUser(int userId)
        {
            string sql = @"select t.* from users t where t.UserId = @userId";

            return _databaseInstance.Query<AccountUser>(sql, new { userId = userId }).FirstOrDefault();
        }


        public void DeleteUser(int userId)
        {
            string sql = @"Delete from users where UserId = @userId";
            _databaseInstance.ExecuteNonQuery(sql, new { userId = userId});
        }


        public int SystemDealLogSave(SystemDealLog item)
        {
            return _databaseInstance.Insert(item, "SystemDealLogs");
        }


        void IAccountDealDal.SystemDealLogSave(SystemDealLog item)
        {
            throw new NotImplementedException();
        }
    }
}