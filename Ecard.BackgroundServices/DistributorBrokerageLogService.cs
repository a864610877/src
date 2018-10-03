using System;
using System.Data;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Mvc.Models.ShopLiquidates;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Ecard.Models;

namespace Ecard.BackgroundServices
{
    public class DistributorBrokerageLogService : IBackgroundService
    {
        private readonly DatabaseInstance _databaseInstance;
        //[NoRender,Dependency]
        //public IDistributorService  DistributorService { get; set; }
         public DistributorBrokerageLogService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

         public void Execute()
         {
             _databaseInstance.BeginTransaction();

             //自动向一级经销商发起结算请求。
             //1、取得所有的一级经销商
             var distributors = _databaseInstance.Query<Distributor>("select * from Distributors where ParentId=0", null).ToList();
             DateTime? bdate = null;
             //2、上次结算的截止时间，取得上次截止时间到现在的消费。
             string sql = "select * from DistributorBrokerage where  Edate=(select MAX(edate) from  DistributorBrokerage)";
             var lastLog = _databaseInstance.Query<DistributorBrokerage>(sql, null).FirstOrDefault();
             if (lastLog != null)
                 bdate = lastLog.Edate;
             var dealLogs = _databaseInstance.Query<TempDistributorBrokerage>("select AccountId,Amount,DistributorId from V_DistributorRate where (@bdate is null or submitTime>@bdate)", new { bdate = bdate }).ToList();
             foreach (var item in distributors)
             {
                 //判断消费的会员卡所属的经销商是不是当前经销商的下级或者是当前经销商本身。，如果是，则系统向当前经销商发起结算请求。
                 for (int i = 0; i < dealLogs.Count; i++)
                 {
                     if (dealLogs[i].DistributorId == 0)
                         continue;
                     if (IsMyunderOrMyself(item.DistributorId, dealLogs[i].DistributorId))
                     {                         
                         var sql1 = @"select t.* from DistributorAccountLevelPolicyRates t where t.DistributorId = @DistributorId";
                         decimal myRate = 0m;
                         var rate = _databaseInstance.Query<DistributorAccountLevelRate>(sql1, new { DistributorId = item.DistributorId }).FirstOrDefault();
                         if (rate != null)
                             myRate = rate.Rate;
                         var distributorBrokerageItem = new DistributorBrokerage();
                         distributorBrokerageItem.Edate = DateTime.Now;
                         distributorBrokerageItem.DistributorId = dealLogs[i].DistributorId;
                         distributorBrokerageItem.settlementDistributorId = item.DistributorId;
                         distributorBrokerageItem.consume = dealLogs[i].Amount;
                         distributorBrokerageItem.brokerage = dealLogs[i].Amount * myRate;
                         distributorBrokerageItem.AccountId = dealLogs[i].AccountId;
                         distributorBrokerageItem.Rate = myRate;
                         distributorBrokerageItem.status = false;
                         if (bdate.HasValue)
                             distributorBrokerageItem.Bdate = bdate.Value;
                         else
                             distributorBrokerageItem.Bdate = DateTime.Now.AddDays(-1);//系统第一次生成的时候。开始时间从半年前开始。
                         _databaseInstance.Insert(distributorBrokerageItem, "DistributorBrokerage");
                         //已经发起请求的就把DistributorId改成-1
                         dealLogs[i].DistributorId = -1;
                     }
                 } 
             }
             //

             _databaseInstance.Commit();

         }
         /// <summary>
         /// 判断经销商是不是自己的下属
         /// </summary>
         /// <param name="parentId"></param>
         /// <param name="Id"></param>
         /// <returns></returns>
         public bool IsMyunderOrMyself(int parentId, int Id)
         {
             var item = _databaseInstance.Query<Distributor>("select * from Distributors where DistributorId=" + Id.ToString(), null).FirstOrDefault();
             if (item != null)
             { 
                 if (item.ParentId == parentId||item.DistributorId==parentId)
                     return true;
                 if (item.ParentId == 0)
                     return false;
                 else return IsMyunderOrMyself(parentId, item.ParentId);
             }
             else
                 return false;
         }
         class TempDistributorBrokerage
         {
             public int AccountId { get; set; }
             public decimal Amount { get; set; }
             public int DistributorId { get; set; }

         }
    }
}
