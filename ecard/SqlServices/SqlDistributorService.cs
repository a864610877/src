using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard.Services
{
    public class SqlDistributorService : IDistributorService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Distributors";

        public SqlDistributorService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        static object Locker = new object();
        public IEnumerable<Distributor> Query()
        {
            string key = CacheKeys.DistributorKey;
            var items = (List<Distributor>)CachePools.GetData(key);
            if (items == null)
            {
                lock (Locker)
                {
                    items = (List<Distributor>)CachePools.GetData(key);
                    if (items == null)
                    {
                        items = InnerQuery();
                        CachePools.AddCache(key, items);
                    }
                }
            }
            return items;
        }

        public Distributor GetById(int id)
        {
            return Query().FirstOrDefault(x => x.DistributorId == id);
        }

        public List<DistributorAccountLevelRate> GetAccountLevelPolicyRates(int distributorId) //经销商提成比例
        {
            var sql = @"select t.* from DistributorAccountLevelPolicyRates t where t.DistributorId = @DistributorId";
            return new QueryObject<DistributorAccountLevelRate>(_databaseInstance, sql, new { DistributorId = distributorId }).ToList();
        }

        public Distributor GetByUserId(int userId)
        {
            var sql = @"select t.* from Distributors t where t.UserId = @UserId";
            return new QueryObject<Distributor>(_databaseInstance, sql, new { UserId = userId }).FirstOrDefault();

        }
        public List<Distributor> GetByParentId(int ParentId)
        {
            var sql = @"select t.* from Distributors t where t.ParentId = @ParentId";
            return new QueryObject<Distributor>(_databaseInstance, sql, new { ParentId = ParentId }).ToList();

        }

        List<Distributor> InnerQuery()
        {
            var sql = @"select t.* from distributors t";
            return new QueryObject<Distributor>(_databaseInstance, sql, new object()).ToList();
        }
        public DataTables<Distributor> New_InnerQuery()
        {

            StoreProcedure sp = new StoreProcedure("P_getDistributors", null);
            return _databaseInstance.GetTables<Distributor>(sp);
        }
        public void Create(Distributor item)
        {
            item.DistributorId = _databaseInstance.Insert(item, TableName);
            CachePools.Remove(CacheKeys.DistributorKey);
        }


        public void Update(Distributor item)
        {
            _databaseInstance.Update(item, TableName);
            CachePools.Remove(CacheKeys.DistributorKey);
        }

        public void UpdateAccountLevelPolicy(int distributorId, DistributorAccountLevelRate rates)
        {
            _databaseInstance.ExecuteNonQuery(
                "delete DistributorAccountLevelPolicyRates where DistributorId = @DistributorId",
                new { DistributorId = distributorId });
            _databaseInstance.Insert(rates, "DistributorAccountLevelPolicyRates");
        }

        public void Delete(Distributor item)
        {
            _databaseInstance.Delete(item, TableName);
            CachePools.Remove(CacheKeys.DistributorKey);
        }
    }
}