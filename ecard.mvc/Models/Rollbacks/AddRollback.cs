using System;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Rollbacks
{
    public class AddRollback : ViewModelBase, ICommandProvider
    {
        public string SerialServerNo { get; set; }
        public decimal Amount { get; private set; }
        public string AccountName { get; private set; }
        public string AccountShopName { get; private set; }
        public string PosName { get; private set; }
        public string OwnerDisplayName { get; private set; }
        public string OwnerMobile { get; private set; }
        public string ShopName { get; private set; }
        public string ShopDisplayName { get; private set; }
        public DateTime? SubmitTime { get; private set; }
        public string DealType { get; private set; }
        public string State { get; private set; }

        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IRollbackShopDealLogService RollbackService { get; set; }
        [NoRender]
        public ShopDealLog ShopDealLog { get; set; }

        [NoRender]
        public bool DisableApply { get; private set; }
        public void Ready()
        {
            DisableApply = true;
            var id = 0;
            if (!int.TryParse(this.SerialServerNo, out id))
            {
                this.AddError(0, "nonfound", SerialServerNo);
                DisableApply = true;
                return;
            }
            ShopDealLog = ShopDealLogService.GetByAddin(id);
            if (ShopDealLog == null)
            {
                this.AddError(0, "nonfound", SerialServerNo);
                DisableApply = true;
                return;
            }
            State = ModelHelper.GetBoundText(ShopDealLog, x => x.State);

            Amount = ShopDealLog.Amount;
            PosName = ShopDealLog.SourcePosName;
            SubmitTime = ShopDealLog.SubmitTime;
            AccountName = ShopDealLog.AccountName;
            DealType = ModelHelper.GetBoundText(ShopDealLog, x => x.DealType);

            var shop = ShopService.GetById(ShopDealLog.ShopId);
            ShopDisplayName = shop.DisplayName;
            ShopName = shop.Name;

            var account = AccountService.GetById(ShopDealLog.AccountId);
            if (account.OwnerId.HasValue)
            {
                var owner = MembershipService.GetUserById(account.OwnerId.Value);
                if (owner != null)
                {
                    OwnerMobile = owner.Mobile;
                    OwnerDisplayName = owner.DisplayName;
                }
            }
            var accountShop = ShopService.GetById(account.ShopId);
            if(accountShop!= null)
            {
                AccountShopName = accountShop.DisplayName;
            }
            var rollbackItem = RollbackService.GetByShopDealLogId(ShopDealLog.ShopDealLogId);
            if (rollbackItem != null)
            {
                if (rollbackItem.State == RollbackShopDealLogState.Processing)
                    ApplyState = "已经申请";
                else
                    ApplyState = "已申请成功";
            }
            else
            {
                if (ShopDealLog.State != DealLogStates.Normal_)
                    ApplyState = "未申请";
                DisableApply = false;
            }
        }

        public string ApplyState { get; private set; }
        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            if (!DisableApply)
            {
                yield return new ActionMethodDescriptor("add");
                if (this.ShopDealLog.State != DealLogStates.Normal_)
                {
                    yield return new ActionMethodDescriptor("apply");
                }
            }
        }

        public void Apply()
        {
            var id = 0;
            if (!int.TryParse(this.SerialServerNo, out id))
            {
                this.AddError(0, "nonfound", SerialServerNo);
                DisableApply = true;
                return;
            }
            ShopDealLog = ShopDealLogService.GetByAddin(id);
            if (ShopDealLog == null)
            {
                this.AddError(0, "nonfound", SerialServerNo);
                DisableApply = true;
                return;
            }
            if (ShopDealLog.State == DealLogStates.Normal_)
            {
                DisableApply = true;
                return;
            }
            State = ModelHelper.GetBoundText(ShopDealLog, x => x.State);

            var rollbackItem = RollbackService.GetByShopDealLogId(ShopDealLog.ShopDealLogId);
            if (rollbackItem != null)
            {
                AddError(0, "alreadyExisting", SerialServerNo);
            }
            else
            {
                RollbackShopDealLog item = new RollbackShopDealLog()
                                               {
                                                   ShopDealLogId = ShopDealLog.ShopDealLogId,
                                                   State = RollbackShopDealLogState.Processing,
                                                   ShopId = ShopDealLog.ShopId,
                                                   SubmitTime = DateTime.Now,
                                               };

                RollbackService.Create(item);
                this.AddMessage("success", SerialServerNo);

                Amount = 0;
                AccountName = "";
                PosName = "";
                ShopName = "";
                ShopDisplayName = "";
                SubmitTime = null;
                DealType = "";
                State = "";
                SerialServerNo = "";
            }
        }
    }
}