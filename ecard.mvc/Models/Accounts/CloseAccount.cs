using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class CloseAccount : EcardModel
    {
        public string AccountName { get; set; }

        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }

        [NoRender, Dependency]
        public Site HostSite { get; set; }

        [NoRender, Dependency]
        public IAccountTypeService AccountTypeService { get; set; }
        [NoRender, Dependency]
        public IShopService  ShopService { get; set; }

        [NoRender, Dependency]
        public ISystemDealLogService SystemDealLogService { get; set; }
        [NoRender, Dependency]
        public IDealLogService DealLogService { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }
        [NoRender, Dependency]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency, NoRender]
        public Site CurrentSite { get; set; }


        private class MyAccountServiceResponse : AccountServiceResponse
        {
            public MyAccountServiceResponse(int code, DealLog deallog, Shop accountShop, Account account, User user)
                : base(code, deallog, accountShop, account, user, null, null)
            {
            }

            public decimal DepositAmount { get; set; }
        }

        public AccountServiceResponse Save()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            var accountName = AccountName.TrimSafty();
            var account = AccountService.GetByName(accountName);
            if (account == null || (account.State != AccountStates.Normal && account.State != AccountStates.Invalid))
            {
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            }
            User owner = null;
            if (account.OwnerId.HasValue)
                owner = MembershipService.GetUserById(account.OwnerId.Value);

            var amount = account.Amount;        // Ó¦ÍË½ð¶î 
            var depositAmount = account.DepositAmount;

            account.Amount = 0;
            account.DepositAmount = 0;
            account.State = AccountStates.Closed;
            account.LastDealTime = DateTime.Now;
            account.ExpiredDate = DateTime.Now;
            account.OwnerId = null;
            // ÍË¿¨
            //
            var currentUser = SecurityHelper.GetCurrentUser().CurrentUser;
            var systemDealLog = new SystemDealLog(serialNo, currentUser) { Amount = -amount, DealType = SystemDealLogTypes.CloseCard };
            SystemDealLogService.Create(systemDealLog);
            CashDealLogService.Create(new CashDealLog(systemDealLog.Amount, 0, currentUser.UserId, systemDealLog.DealType));
            var dealLog1 = new DealLog(SerialNoHelper.Create()) { DealType = DealTypes.Close, AccountAmount = 0, AccountId = account.AccountId, AccountName = account.Name, Point = 0, Amount = amount, SubmitTime = DateTime.Now, State = DealLogStates.Normal };
            DealLogService.Create(dealLog1);

            // Ñº½ð
            //
            if (depositAmount != 0m)
            {
                var dealLog = new SystemDealLog(serialNo, currentUser) { Amount = -depositAmount, DealType = SystemDealLogTypes.CloseDeposit };
                SystemDealLogService.Create(dealLog);
                CashDealLogService.Create(new CashDealLog(dealLog.Amount, 0, currentUser.UserId, dealLog.DealType));
            }

            AccountService.Update(account);
            if (owner!=null)
            MembershipService.DeleteUser(owner);
            Logger.LogWithSerialNo(LogTypes.AccountClose, serialNo, account.AccountId, accountName);
            TransactionHelper.Commit();
            var response = new MyAccountServiceResponse(ResponseCode.Success, dealLog1, ShopService.GetById(account.ShopId), account, owner) { DepositAmount = depositAmount, Amount = amount };
            if (!string.IsNullOrWhiteSpace(CurrentSite.TicketTemplateOfClose))
            {
                var message = MessageFormator.Format(CurrentSite.TicketTemplateOfClose, account);
                message = MessageFormator.Format(message, amount);
                message = MessageFormator.Format(message, CurrentSite);
                message = message.Replace("#deposit-amount#", depositAmount.ToString());
                message = message.Replace("#total-amount#", (depositAmount + amount).ToString());
                message = MessageFormator.Format(message, owner);
                response.CodeText = message.FormatForJavascript();
            }

            return response;
        }
    }
}