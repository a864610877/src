using System;
using Ecard.Commands;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Liquidates
{
    public class DoneLiquidate
    {
        public int LiquidateId { get; set; }
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
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser;
            var liquidate = LiquidateService.GetById(LiquidateId);
            try
            {
                var cmd = new DoneLiquidateCommand(this.LiquidateId, liquidate.ShopId, DealWay);
                UnityContainer.BuildUp(cmd);
                cmd.Execute(user);
                return new SimpleAjaxResult();
            }
            catch (Exception ex)
            {
                return new SimpleAjaxResult(ex.Message);
            }
        }
    }
}