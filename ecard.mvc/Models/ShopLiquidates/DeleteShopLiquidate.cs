using System;
using System.Linq;
using Ecard.Commands;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class DeleteShopLiquidate
    {
        public int LiquidateId { get; set; }
        [Dependency, NoRender]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;

            try
            {
                var cmd = new DeleteLiquidateCommand(this.LiquidateId, user.ShopId);
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