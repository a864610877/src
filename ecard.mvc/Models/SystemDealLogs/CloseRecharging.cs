using System;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.SystemDealLogs
{
    public class CloseRecharging : ViewModelBase
    {
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        [Dependency, NoRender]
        public Site CurrentSite { get; set; } 
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public LogHelper LogHelper { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public object Save()
        {
            try
            {
                var serialNo = SerialNoHelper.Create();
                SystemDealLog dealLog = SystemDealLogService.GetById(Id);
                if (dealLog != null && dealLog.CanCancel(SystemDealLogTypes.Recharge, CurrentSite))
                {
                    var account = AccountService.GetById(Convert.ToInt32(dealLog.Addin));
                    if (account == null || account.State != AccountStates.Normal)
                        return new DataAjaxResult(Localize("NoAccount", "会员信息未找到"));

                    var passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                    var password = passwordService.Decrypto(Password);

                    if (User.SaltAndHash(password, account.PasswordSalt) != account.Password)
                        return new DataAjaxResult(Localize("error.Password", "密码错误"));

                    dealLog.State = SystemDealLogStates.Closed;
                    account.Amount -= dealLog.Amount;
                    var logItem = DealLogService.GetByAddin(dealLog.SystemDealLogId);
                    TransactionHelper.BeginTransaction();
                    if (logItem != null)
                    {
                        logItem.State = DealLogStates.Cancel;
                        DealLogService.Update(logItem);
                        DealLogService.Create(new DealLog(serialNo)
                                                  {
                                                      Account = account,
                                                      Amount = dealLog.Amount,
                                                      SubmitTime = DateTime.Now,
                                                      State = DealLogStates.Normal,
                                                      DealType = DealTypes.CancelRecharging,
                                                      Addin = logItem.DealLogId,
                                                  });
                    }

                    SystemDealLogService.Update(dealLog);
                    AccountService.Update(account);
                    LogHelper.LogWithSerialNo(LogTypes.SystemDealLogCloseRecharging, serialNo, dealLog.SystemDealLogId, dealLog.SystemDealLogId, account.Name);
                    AddMessage(Localize("CloseRecharging.success"), dealLog.SystemDealLogId);

                    return TransactionHelper.CommitAndReturn(new SimpleAjaxResult());
                }
                return new SimpleAjaxResult(Localize("OpenReceipt.failed", "原交易不存在"));
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.SystemDealLogCloseRecharging, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }
    }
}