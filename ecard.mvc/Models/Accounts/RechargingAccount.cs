using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.DealWays;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Ecard.Mvc.ActionFilters;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Accounts
{
    public class RechargingAccount : ViewModelBase
    {
        public int HowToDeal { get; set; }
        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }
        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        [NoRender, Dependency]
        public IMembershipService _membershipService { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        [NoRender, Dependency]
        public IAccountDealService AccountDealService { get; set; }
        [NoRender, Dependency]
        public SerialNoHelper SerialNoHelper { get; set; }
        [NoRender, Dependency]
        public IRechargingLogService RechargingLogService { get; set; }
        public List<DealWay> DealWays { get; set; }
        [NoRender, Dependency]
        public ISiteService SiteService { get; set; }
        private string _accountName;

        public int StartNum { get; set; }
        public int EndNum { get; set; }
        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }
        public decimal Amount { get; set; }
        public bool HasReceipt { get; set; }
        public int? pageIndex { get; set; }
        public int? pageSize { get; set; }
        public AccountServiceResponse Save()
        {
            try
            {
                Account account = null;
                var accountUser = (AccountUser)_membershipService.GetByMobile(AccountName.Trim());
                if (accountUser != null)
                {
                    account = AccountService.QueryByOwnerId(accountUser).FirstOrDefault();
                    //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
                }
                else
                {
                    account = AccountService.GetByName(AccountName.Trim());
                }
                this.AccountName = account.Name;
                var serialNo = SerialNoHelper.Create();
                //var site = SiteService.Query(null).FirstOrDefault();
                
                User operatorUser = SecurityHelper.GetCurrentUser().CurrentUser;
                RechargingCommand command = new RechargingCommand(serialNo, this.AccountName, this.Amount, this.HasReceipt, HowToDeal, operatorUser.UserId, null);
                
                UnityContainer.BuildUp(command);
                int code = command.Validate();
                if (code != ResponseCode.Success)
                    return new AccountServiceResponse(code);

                //ApplyToModel apply = new ApplyToModel(command.DealWay.ApplyTo);
                //if (!apply.EnabledRecharging)
                //    return new AccountServiceResponse(ResponseCode.InvalidateDealWay);

                TransactionHelper.BeginTransaction();
                Logger.LogWithSerialNo(LogTypes.AccountRecharge, serialNo, command.Account.AccountId, AccountName, Amount);

               
                //if (command.DealWay.IsCash)
                //{
                //    CashDealLogService.Create(new CashDealLog(Amount, 0, SecurityHelper.GetCurrentUser().CurrentUser.UserId, SystemDealLogTypes.Recharge));
                //    command.IsCash = true;
                //}
                //if (!HostSite.IsRechargingApprove)
                    command.Execute(operatorUser);
                var response = new AccountServiceResponse(ResponseCode.Success, command.CreateDealLog(), ShopService.GetById(command.Account.ShopId), command.Account, command.Owner);
                //else
                //    TaskService.Create(new Task(command, operatorUser.UserId) { AccountId = command.Account.AccountId, Amount = Amount });
               
                var dealLog = command.CreateDealLog();
                //if (!string.IsNullOrWhiteSpace(CurrentSite.TicketTemplateOfRecharge))
                //{
                //    var message = MessageFormator.FormatTickForRecharging(CurrentSite.TicketTemplateOfRecharge,
                //                                                          CurrentSite, this.HasReceipt, Amount,
                //                                                          command.DealWay.DisplayName, dealLog, command.Account, command.AccountType,
                //                                                          command.Owner, command.OperatorUser);
                //    PrintTicketService.Create(new PrintTicket(LogTypes.AccountRecharge, serialNo, message, command.Account));
                //    response.CodeText = message.FormatForJavascript();
                //}
                return TransactionHelper.CommitAndReturn(response);
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AccountRecharge, ex);
                throw;
            }
        }
        public AccountServiceResponse DoRecharge()
        {
            var user = SecurityHelper.GetCurrentUser();
            if (user is ShopUserModel)
            {
                var shopUser = user.CurrentUser as ShopUser;
                var shop = ShopService.GetById(shopUser.ShopId);
                if (shop == null)
                    return new AccountServiceResponse(-1) { CodeText = "无效商户" };
                Account account = null;
                var accountUser = (AccountUser)_membershipService.GetByMobile(AccountName.Trim());
                if (accountUser != null)
                {
                    account = AccountService.QueryByOwnerId(accountUser).FirstOrDefault();
                    //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
                }
                else
                {
                    account = AccountService.GetByName(AccountName.Trim());
                }
                // var account = AccountService.GetByName(AccountName.Trim());
                if (account == null || account.State != AccountStates.Normal)
                    return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
                var sn = SerialNoHelper.Create();
                AccountServiceResponse rsp = new AccountServiceResponse(0);
                rsp = AccountDealService.Recharge(new PayRequest(account.Name, "", "", Amount, sn, "", shop.Name, shop.Name),true);
                if (rsp.Code == ResponseCode.Success)
                {
                    //做日志
                    // Logger.LogWithSerialNo(LogTypes.Deal, sn, account.AccountId, rsp.AccountType, account.Name);
                }
                return rsp;
            }
            else if (user is AdminUserModel)
            {
                 
                return Save();
            }
            else
            {
                return new AccountServiceResponse(-1) { CodeText = "无效商户" };
            }
        }
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency, NoRender]
        public ITaskService TaskService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public Site CurrentSite { get; set; }
        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }

        public void Ready()
        {
            DealWays = DealWayService.Query().Where(x => x.State == DealWayStates.Normal && new ApplyToModel(x.ApplyTo).EnabledRecharging).ToList();
        }
          
        public List<RechargingLog> AreaRecharges(out string pageHtml, out int tatolCount)
        {
            var user = SecurityHelper.GetCurrentUser();
            pageHtml = string.Empty;
            List<RechargingLog> listResp = new List<RechargingLog>();
            tatolCount = 0;
            if (user is AdminUserModel)
            {

                if (Amount > 0)
                {
                    string Name = string.Empty;
                    List<Account> accountNames = new List<Account>();
                    for (int i = StartNum; i <= EndNum; i += 1)
                    {
                        Name = string.Format("{0:" + AccountName + "}", i);
                        var account = AccountService.GetAccountByName(Name);
                        if (account != null)
                        {
                            accountNames.Add(account);
                        }

                    }

                    try
                    {

                        if (accountNames != null && accountNames.Count > 0)
                        {
                            User operatorUser = SecurityHelper.GetCurrentUser().CurrentUser;
                            TransactionHelper.BeginTransaction();
                            // AccountServiceResponse response = null;
                            var serialNoAll = DateTime.Now.ToString("yyyyMMddHHmmss");
                            foreach (var item in accountNames)
                            {
                                RechargingLog log = new RechargingLog(1) { SubmitTime = DateTime.Now };
                                var serialNo = SerialNoHelper.Create();
                                var oldAmount = item.Amount;
                                RechargingCommand command = new RechargingCommand(serialNo, item.Name, this.Amount, this.HasReceipt, HowToDeal, operatorUser.UserId, item);
                                UnityContainer.BuildUp(command);
                                int code = command.Validates();
                                if (code != ResponseCode.Success)
                                    listResp.Add(new RechargingLog(code));
                                Logger.LogWithSerialNo(LogTypes.AccountRecharge, serialNo, command.Account.AccountId, item.Name, Amount);
                                command.Execute(operatorUser);
                                //response = new AccountServiceResponse(ResponseCode.Success) { AccountName = item.Name, Amount = item.Amount, DealAmount = Amount, DetainAmount = oldAmount, OwnerDisplayName = command.Owner == null ? "" : command.Owner.DisplayName };

                                log.AccountName = item.Name;
                                log.RechargAccountAmount = item.Amount;
                                log.Name = command.Owner == null ? "" : command.Owner.DisplayName;
                                log.serialNoAll = serialNoAll;
                                log.RechargingAmount = Amount;
                                log.AccountAmount = oldAmount;
                                listResp.Add(log);
                                RechargingLogService.Create(log);
                                var dealLog = command.CreateDealLog();

                            }
                            var list = listResp.Skip<RechargingLog>(0).Take<RechargingLog>(10).ToList<RechargingLog>();
                            tatolCount = listResp.Count;
                            pageHtml = MvcPage.AjaxPager(1, 10, tatolCount);
                            return TransactionHelper.CommitAndReturn(list);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(LogTypes.AccountRecharge, ex);
                        listResp.Add(new RechargingLog(ResponseCode.SystemError));

                    }
                }
                else
                {
                    listResp.Add(new RechargingLog(111));
                }
            }
            else
            {
                listResp.Add(new RechargingLog(11112));

            }
            return listResp;
        }
    }
}