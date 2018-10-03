using System;
using Ecard.Commands;
using Ecard.Models;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopLiquidates
{
    public class DeleteRollback
    {
        public int Id { get; set; }
        [Dependency, NoRender]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;

            try
            {
                var cmd = new DeleteRollbackCommand(this.Id);
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