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
    public class DealLogController : Controller
    {

        [DashboardItem]
        [CheckPermission(Permissions.DealLog)]
        public virtual ActionResult List(ListDealLogs request)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                request.Query();
            }
            return View("List", request);
        }
       
        [HttpPost]
        [CheckPermission(Permissions.DealLogRollback)]
        public virtual ActionResult Rollback(ListDealLogs request)
        {
            var ids = request.CheckItems.GetCheckedIds();
            int Count = 1;
            foreach (var id in ids)
            {
                var rsp = request.Rollback(id);
                if (rsp.Code == 0)
                    request.LogRollbackSuccess(string.Format("第{0}笔交易冲正成功", Count));
                   // Message += string.Format("第{0}笔交易冲正成功", Count); //"第" + Count + "笔交易冲正成功；";
                else
                {
                    request.LogRollbackError(string.Format("第{0}笔交易冲正失败,{1}", Count, rsp.CodeText));
                   // Message += string.Format("第{0}笔交易冲正失败,{1}", Count, rsp.CodeText); // "第" + Count + "笔交易冲正失败，" + rsp.CodeText + ""; ;
                }
                //switch (request.Rollback(id))
                //{
                //    case RollbackType.None:
                //        break;
                //    case RollbackType.Cancel:
                //        cancelCount++;
                //        break;
                //    case RollbackType.Undo:
                //        undoCount++;
                //        break;
                //    default:
                //        throw new ArgumentOutOfRangeException();
                //}
                Count++;
            }
            return List(request);
        }
    }
}
