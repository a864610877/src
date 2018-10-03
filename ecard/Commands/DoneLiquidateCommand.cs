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
    public class DoneLiquidateCommand
    {
        [DataMember]
        public int LiquidateId { get; set; }

        [DataMember]
        public int ShopId { get; set; }

        [DataMember]
        public int DealWayId { get; set; }

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
        public IDealWayService DealWayService { get; set; }
        [Dependency]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency]
        public Site HostSite { get; set; }

        public DoneLiquidateCommand()
        {

        }

        public DoneLiquidateCommand(int liquidateId, int shopId, int dealWayId)
        {
            LiquidateId = liquidateId;
            ShopId = shopId;
            DealWayId = dealWayId;
        }

        public void Execute(User currentUser)
        {
            var serialNo = SerialNoHelper.Create();
            using (var tran = TransactionHelper.BeginTransaction())
            {
                var liquidate = this.LiquidateService.GetById(LiquidateId);
                if (liquidate == null || liquidate.State != LiquidateStates.Processing)
                {
                    throw new Exception("没有找到相关的清算记录");
                }
                liquidate.State = LiquidateStates.Done;
                LiquidateService.Update(liquidate);

                var shop = ShopService.GetById(ShopId);
                var rate = shop.ShopDealLogChargeRate ?? HostSite.ShopDealLogChargeRate;
                var rateAmount = (liquidate.DealAmount * rate);
                var amount = liquidate.DealAmount - liquidate.CancelAmount;

                var systemDealLog = new SystemDealLog(serialNo, currentUser)
                                        {
                                            Addin = liquidate.LiquidateId.ToString(),
                                            Amount = -amount,
                                            DealType = SystemDealLogTypes.ShopDealLogDone,
                                            DealWayId = DealWayId,
                                        };
                SystemDealLogService.Create(systemDealLog);

                shop.Amount -= amount;
                var shopDealLog = new ShopDealLog(serialNo, DealTypes.ShopDealLogDone, -amount, null, null, null, shop,
                                                  systemDealLog.SystemDealLogId);
                ShopDealLogService.Create(shopDealLog);

                systemDealLog = new SystemDealLog(serialNo, currentUser)
                                        {
                                            Addin = liquidate.LiquidateId.ToString(),
                                            Amount = rateAmount,
                                            DealType = SystemDealLogTypes.ShopDealLogCharging,
                                            DealWayId = DealWayId,
                                        };
                SystemDealLogService.Create(systemDealLog);

                shop.RechargingAmount += rateAmount;
                shopDealLog = new ShopDealLog(serialNo, DealTypes.ShopDealLogDone, rateAmount, null, null, null, shop,
                                                  systemDealLog.SystemDealLogId);
                ShopDealLogService.Create(shopDealLog);

                var dealWay = DealWayService.GetById(this.DealWayId);
                if (dealWay.IsCash)
                {
                    CashDealLogService.Create(new CashDealLog(amount - rateAmount, currentUser.UserId, currentUser.UserId, CashDealLogTypes.ShopDealLogDone));
                }
                ShopService.Update(shop);
                tran.Commit();
            }
        }

        public int Validate()
        {
            return ResponseCode.Success;
        }
    }
}