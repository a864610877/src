using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.DealWays;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Accounts
{
    public class OpenAccount : EcardModel
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public int HowToDeal { get; set; }
        public string Identify { get; set; }
        public string Remark1 { get; set; }
        public int SaleId { get; set; }
        public int Gender { get; set; }
        public string Code { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsActived { get; set; }

        public List<AdminUser> Sales { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        public List<DealWay> DealWays { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }

        [NoRender, Dependency]
        public IAccountTypeService AccountTypeService { get; set; }

        [NoRender, Dependency]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        [NoRender, Dependency]
        public IPrintTicketService PrintTicketService { get; set; }

        [NoRender, Dependency]
        public Site HostSite { get; set; }

        [NoRender, Dependency]
        public ISystemDealLogService SystemDealLogService { get; set; }
        [NoRender, Dependency]
        public ICashDealLogService CashDealLogService { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [Dependency]
        public RandomCodeHelper CodeHelper { get; set; }
        [Dependency]
        public IDealWayService DealWayService { get; set; }
        [NoRender, Dependency]
        public IDealLogService DealLogService { get; set; }
        [NoRender, Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public AccountServiceResponse Save()
        {
            var serialNo = SerialNoHelper.Create();
            string password1 = "";
            string password2 = "";

            var passSvc = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
            passSvc.Decrypto(Password, PasswordConfirm, out password1, out password2);

            if (!string.IsNullOrEmpty(password1) || !string.IsNullOrEmpty(password2))
            {
                if (password1 != password2)
                {
                    return new OpenAccountServiceResponse(ResponseCode.SystemError)
                               {
                                   CodeText = "两次密码输入不一致"
                               };
                }
            }
            string accountName = AccountName.TrimSafty();
            var operatorUser = SecurityHelper.GetCurrentUser().CurrentUser;
            //try
            //{
            //    if (CodeHelper.GetObject<string>("sms") != Code || CodeHelper.GetObject<string>("sms_mobile") != Mobile)
            //    {
            //        Mobile = "";
            //    }
            //}
            //catch
            //{

            //}
            if (!string.IsNullOrWhiteSpace(Mobile))
            {
                User u = MembershipService.GetByMobile(Mobile);
                if (u != null)
                {
                    return new AccountServiceResponse(-1) { CodeText = "手机号已绑定" };
                }
            }

            var command = new OpenAccountCommand(serialNo, accountName, password1, DisplayName, BirthDate, IsActived, HowToDeal, Identify, Remark1, operatorUser.UserId, SaleId, Gender, Mobile);
            UnityContainer.BuildUp(command);
            int code = command.Validate();
            if (code != ResponseCode.Success)
                return new AccountServiceResponse(code);

            TransactionHelper.BeginTransaction();

            command.Execute(operatorUser);
            decimal? saleFee = 0m;
            if (command.AccountType != null)
            {
                saleFee = HostSite.SaleCardFee;
                // 手续费
                //
                if (saleFee != null && saleFee.Value != 0m)
                {
                    var account = AccountService.GetByName(accountName);
                    account.ChargingAmount += saleFee.Value;
                    AccountService.Update(account);
                    var d = DealWayService.Query().FirstOrDefault(x => x.State == DealWayStates.Normal);
                    var systemDealLog = new SystemDealLog(serialNo, operatorUser) { Amount = saleFee.Value, DealWayId = (d == null ? 0 : d.DealWayId), DealType = SystemDealLogTypes.SaldCardFee, Addin = account.AccountId.ToString() };
                    SystemDealLogService.Create(systemDealLog);
                    if (d.IsCash)
                        CashDealLogService.Create(new CashDealLog(systemDealLog.Amount, 0, operatorUser.UserId, systemDealLog.DealType));
                }
            }

            // sale Id
            //
            var accountShop = ShopService.GetById(command.Account.ShopId);
            Logger.LogWithSerialNo(LogTypes.AccountOpen, serialNo, command.Account.AccountId, command.Account.Name);
            var r = new OpenAccountServiceResponse(ResponseCode.Success, command.DealLog, accountShop, command.Account, command.Owner)
            {
                SaleFee = saleFee == null ? 0m : saleFee.Value,
                DepositAmount = command.Account.DepositAmount
            };
            if (command.AccountType != null)
                r.AccountType = command.AccountType.DisplayName;

            if (!string.IsNullOrEmpty(this.HostSite.TicketTemplateOfOpen))
            {
                var dealLog = command.DealLog;
                var msg = this.HostSite.TicketTemplateOfOpen;
                msg = MessageFormator.FormatForOperator(msg, SecurityHelper.GetCurrentUser());
                msg = MessageFormator.Format(msg, dealLog);
                msg = MessageFormator.FormatHowToDeal(msg, command.DealWay.DisplayName);
                msg = MessageFormator.Format(msg, command.DealLog);
                msg = MessageFormator.Format(msg, command.AccountType);
                msg = MessageFormator.Format(msg, command.Owner);
                msg = MessageFormator.Format(msg, HostSite);
                r.CodeText = msg;
                PrintTicketService.Create(new PrintTicket(LogTypes.AccountOpen, serialNo, msg, command.Account));
            }
            return TransactionHelper.CommitAndReturn(r);
        }

        #region Nested type: OpenAccountServiceResponse

        private class OpenAccountServiceResponse : AccountServiceResponse
        {
            public OpenAccountServiceResponse(int code, DealLog dealLog, Shop accountShop, Account account, User user)
                : base(code, dealLog, accountShop, account, user, null, null)
            {
            }

            public OpenAccountServiceResponse(int code)
                : base(code)
            {
            }

            public string AccountType { get; set; }
            public decimal DepositAmount { get; set; }

            public decimal SaleFee { get; set; }
        }

        #endregion

        public void Ready()
        {
            Sales = MembershipService.GetSales().ToList();
            DealWays = DealWayService.Query().Where(x => x.State == DealWayStates.Normal && new ApplyToModel(x.ApplyTo).EnabledOpenAccount).ToList();
        }
    }
}