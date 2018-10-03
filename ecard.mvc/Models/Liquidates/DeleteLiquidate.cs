using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Liquidates
{
    public class DeleteLiquidate
    {
        public int Id { get; set; }

        [Dependency, NoRender]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public ILiquidateService LiquidateService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public SimpleAjaxResult Ready()
        {
            var user = this.SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            var liquidate = LiquidateService.GetById(Id);
            try
            {
                var cmd = new DeleteLiquidateCommand(this.Id, liquidate.ShopId);
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