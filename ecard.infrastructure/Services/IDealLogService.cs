using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit;
using Moonlit.Data;

namespace Ecard.Services
{
    public interface IDealLogService
    {
        QueryObject<DealLog> Query(DealLogRequest request);
        void Create(DealLog item);
        DealLog GetById(int id);
        void Update(DealLog item);
        void Delete(DealLog item);
        DealLog GetByAddin(int addIn);
        QueryObject<DealLog> GetByIds(int[] ids);
        DealLog GetBySerialNo(string serialNo, string shopName, string posName);
        decimal SumNoLiquidate(int shopId);
        int UpdateLiquidateId(List<int> ids, int liquidateId, int originalId, int shopid);
        //ÐÂÔö
        void CreateDistributorBrokerage(DistributorBrokerage item);
        void UpdateDistributorBrokerage(DistributorBrokerage item);
        DistributorBrokerage GetDistributorBrokerageById(int id);
        QueryObject<DistributorBrokerage> QueryBrokerage(DistributorBrokerageRequest request);
    }
    public class DealLogQueryTypes
    {
        public const int All = Globals.All;
        public const int ShopDealType = 1;
    }
    public class DealLogRequest
    {
        [Bounded(typeof(DealLogQueryTypes))]
        public int? DealLogQueryType { get; set; }
        public int? AccountId { get; set; }
        private string _accountName;
        public string AccountName
        {
            get
            {
                return _accountName.NullIfEmpty();
            }
            set
            {
                _accountName = value;
            }
        }
        public int? ShopId { get; set; }

        public DateTime? SubmitTimeMax { get; set; }

        public int? State { get; set; }

        public DateTime? SubmitTimeMin { get; set; }

        private string _shopName;
        public string ShopName
        {
            get { return _shopName.NullIfEmpty(); }
            set { _shopName = value; }
        }

        public int? PosId { get; set; }

        public int? AccountShopId { get; set; }

        public int? DealType { get; set; }
    }

    public class DistributorBrokerageRequest
    {
        public string NameWith { get; set; }

        public int? DistributorId { get; set; }

        public DateTime? SubmitTimeMax { get; set; }

        public bool? Status { get; set; }

        public DateTime? SubmitTimeMin { get; set; }
    }

    public class SqlDealLogService : IDealLogService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "DealLogs";
        private const string TableName1 = "DistributorBrokerage";

        public SqlDealLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<DealLog> Query(DealLogRequest request)
        {
            return new QueryObject<DealLog>(_databaseInstance, "DealLog.query", request);
        }

        public void Create(DealLog item)
        {
            item.DealLogId = _databaseInstance.Insert(item, TableName);
        }

        public DealLog GetById(int id)
        {
            return _databaseInstance.GetById<DealLog>(TableName, id);
        }

        public void Update(DealLog item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(DealLog item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public DealLog GetByAddin(int addIn)
        {
            return new QueryObject<DealLog>(_databaseInstance, "DealLog.GetByAddin", new { addIn = addIn }).FirstOrDefault();
        }
         
        public QueryObject<DealLog> GetByIds(int[] ids)
        {
            return new QueryObject<DealLog>(_databaseInstance, "select * from DealLogs where dealLogid in (@ids)", new{ids = ids});
        }

        public DealLog GetBySerialNo(string serialNo, string shopName, string posName)
        {
            var arg = new { serialNo = serialNo, shopName = shopName , posName = posName};
            var sql = "select * from DealLogs where serialNo = @serialNo and shopName = @shopName and sourceposName = @posName";
            return new QueryObject<DealLog>(_databaseInstance, sql, arg).FirstOrDefault();
        }

        public decimal SumNoLiquidate(int shopId)
        {
            var sql = "select sum(amount) from DealLogs where state <> 3 and dealtype in (1,4,8) and LiquidateDealLogId = 0 and shopId = @shopId";
            return (decimal) (_databaseInstance.ExecuteScalar(sql, new {shopId = shopId})??0m);
        }

        public int UpdateLiquidateId(List<int> ids, int liquidateId, int originalId, int shopId)
        {
            var sql = @"update deallogs set LiquidateDealLogId = @liquidateId where state <> @state and deallogid in (@ids) and dealtype in (1,2,4,6,8) and shopid = @shopid and LiquidateDealLogId = @originalId";
            return _databaseInstance.ExecuteNonQuery(sql, new { shopId = shopId, state = DealLogStates.Normal_, liquidateId = liquidateId, ids = ids, originalId = originalId });
        }


        public void CreateDistributorBrokerage(DistributorBrokerage item)
        {
            _databaseInstance.Insert(item, TableName1);
        }

        public void UpdateDistributorBrokerage(DistributorBrokerage item)
        {
            _databaseInstance.Update(item, TableName1);
        }

        public QueryObject<DistributorBrokerage> QueryBrokerage(DistributorBrokerageRequest request)
        {
            string sql = @"select * from DistributorBrokerage where 1=1
                         and (@status is null or status = @status) and (@SubmitTimeMax is null or Edate < @SubmitTimeMax) 
                         and (@NameWith is null or AccountId in (select AccountId from Accounts where Name like '%' + @NameWith + '%'))
                         and (@SubmitTimeMin is null or Bdate >= @SubmitTimeMin) and (@DistributorId is null or DistributorId = @DistributorId)";
            return new QueryObject<DistributorBrokerage>(_databaseInstance, sql, request);
        }


        public DistributorBrokerage GetDistributorBrokerageById(int id)
        {
            return _databaseInstance.GetById<DistributorBrokerage>(TableName1, id);
        }
    }
}