using System;
using System.Linq;
using System.Runtime.Serialization;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class DeleteLiquidateCommand
    {
        [DataMember]
        public int LiquidateId { get; set; }

        [DataMember]
        public int ShopId { get; set; }

        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public ILiquidateService LiquidateService { get; set; }

        public DeleteLiquidateCommand()
        {

        }

        public DeleteLiquidateCommand(int liquidateId, int shopId)
        {
            LiquidateId = liquidateId;
            ShopId = shopId;
        }

        public void Execute(User currentUser)
        {
            using (var tran = TransactionHelper.BeginTransaction())
            {
                var liquidate = this.LiquidateService.GetById(LiquidateId);
                if (liquidate == null || liquidate.State != LiquidateStates.Processing)
                {
                    throw new Exception("没有找到相关的清算记录");
                }

                this.LiquidateService.Delete(liquidate);
                var ids = liquidate.DealIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToList();
                if (liquidate.Count != ShopDealLogService.UpdateLiquidateId(ids, 0, liquidate.LiquidateId, ShopId))
                {
                    throw new Exception("清算数据冲突");
                }
                var dealLogIds = ShopDealLogService.GetAddins(ids.ToArray());
                if (liquidate.Count != dealLogIds.Count)
                {
                    throw new Exception("清算数据冲突");
                }
                if (liquidate.Count != DealLogService.UpdateLiquidateId(dealLogIds, 0, liquidate.LiquidateId, ShopId))
                {
                    throw new Exception("清算数据冲突");
                }
                tran.Commit();
            }
        }

        public int Validate()
        {
            return ResponseCode.Success;
        }
    }
    [DataContract]
    public class DeleteRollbackCommand
    {
        [DataMember]
        public int RollbackId { get; set; }

        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }

        public DeleteRollbackCommand()
        {

        }

        public DeleteRollbackCommand(int rollbackId)
        {
            RollbackId = rollbackId;
        }

        public void Execute(User currentUser)
        {
            using (var tran = TransactionHelper.BeginTransaction())
            {
                var liquidate = this.RollbackShopDealLogService.GetById(this.RollbackId);
                if (liquidate == null || liquidate.State != LiquidateStates.Processing)
                {
                    throw new Exception("没有找到相关的申请记录");
                }

                this.RollbackShopDealLogService.Delete(liquidate);
                tran.Commit();
            }
        }

        public int Validate()
        {
            return ResponseCode.Success;
        }
    }
}