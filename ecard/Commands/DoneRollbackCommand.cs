using System;
using System.Runtime.Serialization;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class DoneRollbackCommand
    {
        [DataMember]
        public int RollbackId { get; set; }
        [Dependency]
        public TransactionHelper TransactionHelper { get; set; }
        [Dependency]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public ISystemDealLogService SystemDealLogService { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IRollbackShopDealLogService RollbackShopDealLogService { get; set; }
        [Dependency]
        public Site HostSite { get; set; }

        public DoneRollbackCommand()
        {

        }

        public DoneRollbackCommand(int rollbackId)
        {
            this.RollbackId = rollbackId;
        }

        public void Execute(User currentUser)
        {
            var serialNo = SerialNoHelper.Create();
            using (var proxy = new ServiceProxy<IAccountDealService>())
            {
                var rollback = this.RollbackShopDealLogService.GetById(RollbackId);
                if (rollback == null || rollback.State != RollbackShopDealLogState.Processing)
                    throw new Exception("³åÕýÇëÇóÎ´ÕÒµ½");
                var shopDealLog = this.ShopDealLogService.GetById(rollback.ShopDealLogId);
                var dealLog = this.DealLogService.GetById(shopDealLog.Addin);
                var account = this.AccountService.GetById(dealLog.AccountId);
                var request = new PayRequest_(dealLog.AccountName, "", dealLog.SourcePosName, Math.Abs(dealLog.Amount), serialNo, dealLog.SerialNo, account.AccountToken, dealLog.SourceShopName) { IsForce = true };
                var rsp = proxy.Proxy.Roolback(request);
                if (rsp.Code != ResponseCode.Success)
                    throw new Exception(ModelHelper.GetBoundText(rsp, x => x.Code));
                rollback.State = RollbackShopDealLogState.Done;
                RollbackShopDealLogService.Update(rollback);
            }
        }

        public int Validate()
        {
            return ResponseCode.Success;
        }
    }
}