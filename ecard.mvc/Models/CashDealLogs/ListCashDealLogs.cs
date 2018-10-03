using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.CashDealLogs
{
    public class ListCashDealLogs : EcardModelListRequest<ListCashDealLog>
    {
        public ListCashDealLogs()
        {
            OrderBy = "CashDealLogId desc";
            this.SubmitTimeMax = DateTime.Now.Date;
            this.SubmitTimeMin = DateTime.Now.Date;
        }
         
        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }

        public void Ready()
        {
        }
        [DataType(DataType.Date)]
        public DateTime? SubmitTimeMax { get; set; }
        [DataType(DataType.Date)]
        public DateTime? SubmitTimeMin { get; set; }
        public void Query()
        {
            CashDealLogRequest req = new CashDealLogRequest();
            req.SubmitTimeMax = SubmitTimeMax;
            req.SubmitTimeMin = SubmitTimeMin;
            var query = this.CashDealLogService.Query(req);
            // fill condition
            List = query.ToList(this,  x => new ListCashDealLog(x));
            var ownerIds = List.Select(x => x.InnerObject.OwnerId).Distinct().ToArray();
            var userIds = List.Select(x => x.InnerObject.UserId).Distinct().ToArray();

            var users = this.MembershipService.QueryUsers<AdminUser>(new UserRequest{UserIds = ownerIds.Union(userIds).Distinct().ToArray()}).ToList();
            foreach (var item in List)
            {
                var user = users.FirstOrDefault(x => x.UserId== item.InnerObject.OwnerId);
                if (user != null)
                    item.OwnerName = user.DisplayName + "(" + user.Name + ")";

                user = users.FirstOrDefault(x => x.UserId == item.InnerObject.UserId);
                if (user != null)
                    item.UserName = user.DisplayName + "(" + user.Name + ")";
            }
        }

        public void Suspend(int id)
        {
            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            var item = this.CashDealLogService.GetById(id);
            if (item != null)
            {
                item.State = CashDealLogStates.Invalid;
                CashDealLogService.Update(item);
                Logger.LogWithSerialNo(LogTypes.CashDealLogSuspend, serialNo, item.CashDealLogId, serialNo);
                AddMessage("suspend.success", serialNo);
            }
            TransactionHelper.Commit();
        } 

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
        }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListCashDealLog item)
        {
            if (item.InnerObject.State == CashDealLogStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new {id = item.CashDealLogId});
        }
         
    }
}
