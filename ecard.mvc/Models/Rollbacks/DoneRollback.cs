using System;
using Ecard.Commands;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Rollbacks
{
    public class DoneRollback
    {
        public int Id { get; set; }
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
        public  IRollbackShopDealLogService RollbackService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser;
            try
            {
                var cmd = new DoneRollbackCommand(this.Id);
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