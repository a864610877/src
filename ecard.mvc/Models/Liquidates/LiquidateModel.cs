using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Liquidates
{
    public class LiquidateModel : ICommandProvider
    {
        [Hidden]
        public int LiquidateId { get; set; }
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal DealAmount { get; set; }
        public decimal RateAmount { get; set; }
        public decimal CancelAmount { get; set; }
        public decimal Count { get; set; }
        public decimal CashAmount { get; set; }

        private Bounded _dealWayBounded;

        public Bounded DealWay
        {
            get
            {
                if (_dealWayBounded == null)
                {
                    _dealWayBounded = Bounded.CreateEmpty("DealWayId", 0);
                }
                return _dealWayBounded;
            }
            set { _dealWayBounded = value; }
        }
        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency, NoRender]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency, NoRender]
        public Site HostSite { get; set; }
        [NoRender]
        public Liquidate Liquidate { get; set; }

        public string State { get; set; }
        public string Formula { get; set; }
        [UIHint("ItemList")]
        public ItemList<ListShopDealLog> Items { get; set; }

        private bool _hasError;
        public void Ready()
        {
            LiquidateId = Id;
            _hasError = true;
            Liquidate = LiquidateService.GetById(Id);
            State = ModelHelper.GetBoundText(Liquidate, x => x.State);
            var currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
            var shop = ShopService.GetById(Liquidate.ShopId);

            var rate = shop.ShopDealLogChargeRate ?? HostSite.ShopDealLogChargeRate;
            RateAmount = (Liquidate.DealAmount * rate);
            Amount = (Liquidate.DealAmount - Liquidate.CancelAmount) - RateAmount;
            Formula = string.Format("({0} - {1}) - {0} * {2}", Liquidate.DealAmount, Liquidate.CancelAmount, rate.ToString("P"));
            DealAmount = Liquidate.DealAmount;
            CancelAmount = Liquidate.CancelAmount;
            Count = Liquidate.Count;

            var ids = Liquidate.DealIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
            Items = new ItemList<ListShopDealLog>(this.ShopDealLogService.GetByIds(ids).Select(x => new ListShopDealLog(x)));

            CashAmount = CashDealLogService.GetSummary(currentUser.UserId);


            var dealways = from x in DealWayService.Query()
                           where new ApplyToModel(x.ApplyTo).EnabledShopDealAccount
                           select new IdNamePair { Key = x.DealWayId, Name = x.DisplayName };
            this.DealWay.Bind(dealways);
            this.DealWay.IsReadOnly = this.Liquidate.State == LiquidateStates.Done;
            _hasError = false;
        }

        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            if (!_hasError)
            {
                if (this.Liquidate.State == LiquidateStates.Processing)
                {
                    yield return new ActionMethodDescriptor("done", null, new { id = this.Id });
                    yield return new ActionMethodDescriptor("delete", null, new { id = this.Id });
                }
            }
        }
    }
}