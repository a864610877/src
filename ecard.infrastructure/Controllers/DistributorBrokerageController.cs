using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.Models;
using Ecard.Mvc.Models.DealLogs;
using Ecard.Mvc.Models.Shops;
using Ecard.Services;
using Ecard.Mvc.Models.DistributorBrokerages;
using Ecard.Infrastructure;


namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class DistributorBrokerageController: Controller
    {
        private readonly LogHelper _logger;
        public DistributorBrokerageController( LogHelper logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 提成列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [CheckPermission(Permissions.DistributorBrokerage)]
        public virtual ActionResult ListDistributorBroekerage(ListDistributorBrokerages request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query();
            }
            request.Ready();
            return View("ListDistributorBrokerage", request);
        }
        /// <summary>
        /// 确认请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(Permissions.DistributorBrokerage)]
        public virtual ActionResult confirm(int Id, ListDistributorBrokerages request)
        {
            //确认……
            request.Close(Id);
            return ListDistributorBroekerage(request);
        }
        /// <summary>
        /// 确认请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(Permissions.DistributorBrokerage)]
        public virtual ActionResult confirms(ListDistributorBrokerages request)
        {
            //确认……

            var ids = request.CheckItems.GetCheckedIds();
            for (int i = 0; i < ids.Length; i++)
			{
                request.Close(ids[i]);
			}
            
            return ListDistributorBroekerage(request);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [CheckPermission(Permissions.DistributorBrokerage)]
        public ActionResult Export(ListDistributorBrokerages request)
        {
            //导出
            _logger.LogWithSerialNo(LogTypes.CashDealLogExport, SerialNoHelper.Create(), 0);
            return ListDistributorBroekerage(request);
        }
    }
}
