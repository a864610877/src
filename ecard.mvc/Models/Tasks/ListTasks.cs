using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Runtime.Serialization;

namespace Ecard.Mvc.Models.Tasks
{
    public class ListTasks : EcardModelListRequest<ListTask>
    {
        public ListTasks()
        {
            OrderBy = "TaskId desc";
        }


        [Dependency, NoRender]
        public ITaskService TaskService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }

        public void Ready()
        {
        }

        public void Query()
        {
            var request = new TaskRequest();
            if (this.State != TaskStates.All)
                request.State = State;

            request.CommandTypeName = this.CommandType;
            var query = this.TaskService.Query(request);

            // fill condition
            List = query.ToList(this, x => new ListTask(x));

            // commandName, descripton object
            foreach (var task in List)
            {
                var type = Type.GetType(task.InnerObject.CommandTypeName, false, true);
                if (type != null)
                {
                    var desc = ViewModelDescriptor.GetTypeDescriptor(type);
                    task.CommandName = desc.Name;
                }
            }

            // users
            var userids =
                List.Select(x => x.InnerObject.CreatorId).Union(List.Select(x => x.InnerObject.EditorId)).ToArray();

            var users = MembershipService.GetByIds(userids).ToList();
            foreach (var task in List)
            {
                var user = users.FirstOrDefault(x => x.UserId == task.InnerObject.CreatorId);
                if (user != null)
                    task.CreatorUserName = user.DisplayName;
                user = users.FirstOrDefault(x => x.UserId == task.InnerObject.EditorId);
                if (user != null)
                    task.EditorUserName = user.DisplayName;
            }

            var accountIds =
               List.Select(x => x.InnerObject.AccountId).ToArray();
            var accounts = AccountService.GetByIds(accountIds);
            foreach (var task in List)
            {
                var account = accounts.FirstOrDefault(x => x.AccountId == task.InnerObject.AccountId);
                if (account != null)
                    task.AccountName = account.Name;
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield break;
        }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListTask item)
        {
            if (string.Equals(item.InnerObject.CommandTypeName, Task.GetCommandType(typeof(RechargingCommand))))
            {
                if (item.InnerObject.State == TaskStates.Normal)
                {
                    yield return new ActionMethodDescriptor("ApproveRecharging", null, new { id = item.TaskId });
                    yield return new ActionMethodDescriptor("RefuseRecharging", null, new { id = item.TaskId });
                }
            }
            if (string.Equals(item.InnerObject.CommandTypeName, Task.GetCommandType(typeof(LimitAmountCommand))))
            {
                if (item.InnerObject.State == TaskStates.Normal)
                {
                    yield return new ActionMethodDescriptor("ApproveLimitAmount", null, new { id = item.TaskId });
                    yield return new ActionMethodDescriptor("RefuseLimitAmount", null, new { id = item.TaskId });
                }
            }

        }

        private Bounded _state;
        private string _commandType;

        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Task>("State", TaskStates.All);
                }
                return _state;
            }
            set { _state = value; }
        }
        [NoRender]
        public string CommandType
        {
            get
            {
                return _commandType;
            }
            set
            {
                _commandType = value;
            }
        }

        public void Approve<T>(int id, int logType)
            where T : ICommand
        {
            Approve(id, typeof(T), logType);
        }
        public void Approve(int id, Type commandType, int logType)
        {
            var serialNo = SerialNoHelper.Create();
            var item = TaskService.GetById(id);
            if (item != null && item.State == TaskStates.Normal)
            {
                if (!string.Equals(item.CommandTypeName, Task.GetCommandType(commandType), StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                try
                {
                    using (var tran = TransactionHelper.BeginTransaction())
                    {
                        ICommand command = (ICommand)item.CommandParameter.DeserializeAsDataContractJson(commandType);
                        this.UnityContainer.BuildUp(commandType, command);
                        var rsp = command.Validate();
                        if (rsp != 0)
                        {
                            item.Error = ModelHelper.GetBoundText(new Helper(rsp), x => x.Code);
                            Logger.Error(logType, item.Error);
                            return;
                        }
                        var helper = new Helper(command.Execute(SecurityHelper.GetCurrentUser().CurrentUser));
                        if (helper.Code != 0)
                        {
                            item.Error = ModelHelper.GetBoundText(new Helper(rsp), x => x.Code);
                            Logger.Error(logType, item.Error);
                            return;
                        }
                        item.State = TaskStates.Approved;
                        Logger.LogWithSerialNo(logType, serialNo, item.TaskId);
                        TaskService.Update(item);
                        AddMessage("approve.success", serialNo, item.TaskId);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(logType, ex);
                    throw;
                }
            }
            else
            {
                AddError("noexisting");
            }

        }
        public void Reject<T, TRefuseCmd>(int id, int logType)
            where T : ICommand
            where TRefuseCmd : RefuseCommand
        {
            Reject(id, typeof(T), typeof(TRefuseCmd), logType);
        }
        public void Reject(int id, Type commandType, Type refuseCommandType, int logType)
        {
            var serialNo = SerialNoHelper.Create();
            var item = TaskService.GetById(id);
            if (item != null && item.State == TaskStates.Normal)
            {
                if (!string.Equals(item.CommandTypeName, Task.GetCommandType(commandType), StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                try
                {
                    using (var tran = TransactionHelper.BeginTransaction())
                    {
                        ICommand command = (ICommand)item.CommandParameter.DeserializeAsDataContractJson(commandType);
                        this.UnityContainer.BuildUp(commandType, command);
                        RefuseCommand refuseCommand = (RefuseCommand) UnityContainer.Resolve(refuseCommandType);
       
                        var helper = new Helper(refuseCommand.Reject(item, command, SecurityHelper.GetCurrentUser().CurrentUser));
                        if (helper.Code != 0)
                        {
                            item.Error = ModelHelper.GetBoundText(helper, x => x.Code);
                            Logger.Error(logType, item.Error);
                            return;
                        }
                        item.State = TaskStates.Refused;
                        Logger.LogWithSerialNo(logType, serialNo, item.TaskId);
                        TaskService.Update(item);
                        AddMessage("reject.success", serialNo, item.TaskId);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(logType, ex);
                    throw;
                }
            }
            else
            {
                AddError("noexisting");
            }

        }
    }
    internal class Helper
    {
        public Helper(int code)
        {
            this.Code = code;
        }
        [Bounded(typeof(TaskStates))]
        public int Code { get; set; }
    }
}
