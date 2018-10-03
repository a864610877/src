using System;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit.Runtime.Serialization;

namespace Ecard.Mvc.Models.Tasks
{
    public class RefuseCommand : ViewModelBase
    {
        [Dependency]
        public ITaskService TaskService { get; set; }
        public int Reject(Task item, ICommand command, User currentUser)
        {
            var rsp = OnReject(command);
            if (rsp != 0)
            {
                return rsp;
            }
            item.State = TaskStates.Refused;
            return ResponseCode.Success;
        }

        protected virtual int OnReject(ICommand command)
        {
            return ResponseCode.Success;
        }
    }

    public class RefuseRechargeCommand : RefuseCommand
    {
        [Dependency]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
        protected override int OnReject(ICommand command)
        {
            RechargingCommand c = (RechargingCommand)command;
            if (c.IsCash)
            {
                var user = MembershipService.GetUserById(c.OperatorUserId);
                CashDealLogService.Create(new CashDealLog(-c.Amount, 0, user.UserId, SystemDealLogTypes.Recharge));
            }
            return base.OnReject(command);
        }
    }
}