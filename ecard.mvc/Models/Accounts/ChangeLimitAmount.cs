using System;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.Tasks;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Accounts
{
    public class ChangeLimitAmount : ViewModelBase
    {
        private string _accountName;

        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }

        private string _accountToken;

        public string AccountToken
        {
            get { return _accountToken.TrimSafty(); }
            set { _accountToken = value; }
        }

        public decimal LimitAmount { get; set; }

        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public ITaskService TaskService { get; set; }
        public SimpleAjaxResult Ready()
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                var account = AccountService.GetByName(AccountName);
                if (account == null || !string.Equals(account.AccountToken, AccountToken, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(string.Format("会员 '{0}' 未找到", AccountName));
                }
                using (var tran = TransactionHelper.BeginTransaction())
                {
                    account.LimiteAmount = LimitAmount;
                    LimitAmountCommand cmd = new LimitAmountCommand(account.AccountId, LimitAmount);
                    UnityContainer.BuildUp(cmd);
                    Helper h = new Helper(cmd.Validate());
                    if (h.Code != 0)
                    {
                        this.AddError(LogTypes.AccountLimit, "error", ModelHelper.GetBoundText(h, x => x.Code));
                        return new SimpleAjaxResult(ModelHelper.GetBoundText(h, x => x.Code));
                    }
                    if (HostSite.IsLimiteAmountApprove)
                    {
                       //var tasks= TaskService.Query(new TaskRequest()
                       //                       {
                       //                           CommandTypeName = Task.GetCommandType(typeof (LimitAmountCommand)),
                       //                           State = TaskStates.Normal,
                       //                           AccountId = account.AccountId,
                       //                       }).ToList();
                        //foreach (var task in tasks)
                        //{
                        //    TaskService.Delete(task);
                        //}
                        TaskService.Create(new Task(cmd, SecurityHelper.GetCurrentUser().CurrentUser.UserId) { Amount = LimitAmount, AccountId = account.AccountId });
                    }
                    else
                        cmd.Execute(SecurityHelper.GetCurrentUser().CurrentUser);
                    Logger.LogWithSerialNo(LogTypes.AccountLimit, serialNo, account.AccountId, account.Name, LimitAmount);
                    tran.Commit();
                    return new SimpleAjaxResult();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AccountLimit, ex);
                return new SimpleAjaxResult(string.Format("内部错误: '{0}'", ex.Message));
            }
        }
    }
}