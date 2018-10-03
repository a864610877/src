using System;
using System.Linq;
using System.Runtime.Serialization;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class OpenAccountCommand : Command, ICommand
    {
        public OpenAccountCommand(string serialNo, string accountName, string password, string userDisplayName, DateTime? birthDate, bool isActived, int dealWayId, string identifyCard, string remark1, int operatorUserId, int saleId, int gender, string mobile)
        {
            SerialNo = serialNo;
            AccountName = accountName;
            Password = password;
            UserDisplayName = userDisplayName;
            BirthDate = birthDate;
            IsActived = isActived;
            DealWayId = dealWayId;
            Identify = identifyCard;
            Remark1 = remark1;
            OperatorUserId = operatorUserId;
            SaleId = saleId;
            Gender = gender;
            Mobile = mobile;
        }

        [DataMember]
        public string SerialNo { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public string UserDisplayName { get; set; }
        [DataMember]
        public int DealWayId { get; set; }
        [DataMember]
        public string Identify { get; set; }
        [DataMember]
        public string Remark1 { get; set; } 

        [DataMember]
        public int OperatorUserId { get; set; }

        [DataMember]
        public int SaleId { get; set; }
        [DataMember]
        public int Gender { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Mobile { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public DateTime? BirthDate { get; set; }
        [DataMember]
        public bool IsActived { get; set; }

        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        public int Execute(User user)
        {
            if (_dealWay == null)
                _dealWay = new DealWay() { IsCash = false };
            user = user ?? OperatorUser;
            if (!string.IsNullOrWhiteSpace(Password))
            {
                _account.SetPassword(Password);
            }
            _account.State = IsActived ? States.Normal : States.Invalid;
            _account.ExpiredDate = DateTime.Now.AddMonths(_account.ExpiredMonths).Date;
            _account.LastDealTime = DateTime.Now;
            _account.OpenTime = DateTime.Now;
            _account.Remark1 = Remark1;
             

            // 售卡
            //
            var systemDealLog = new SystemDealLog(SerialNo, _operator) { Amount = _account.Amount, DealWayId = DealWayId, DealType = SystemDealLogTypes.SaldCard, Addin = _account.AccountId.ToString() };
            SystemDealLogService.Create(systemDealLog);
           
            if (_dealWay.IsCash)
                CashDealLogService.Create(new CashDealLog(systemDealLog.Amount, 0, user.UserId, systemDealLog.DealType));

            // 押金
            //
            if (_account.DepositAmount != 0m)
            {
                systemDealLog = new SystemDealLog(SerialNo, _operator) { Amount = _account.DepositAmount, DealWayId = DealWayId, DealType = SystemDealLogTypes.Deposit, Addin = _account.AccountId.ToString() };
                SystemDealLogService.Create(systemDealLog);
                if (_dealWay.IsCash)
                    CashDealLogService.Create(new CashDealLog(systemDealLog.Amount, 0, user.UserId, systemDealLog.DealType));
            }

            AccountLevelPolicy accountLevel = AccountLevelPolicyService.Query().FirstOrDefault(x =>x.Level==0&& x.State == States.Normal && x.AccountTypeId == _account.AccountTypeId && _account.TotalPoint >= x.TotalPointStart);
            if (accountLevel != null)
            {
                _account.AccountLevelName = accountLevel.DisplayName;
                _account.AccountLevel = accountLevel.Level;
            }


            // 用户
            //if (_owner != null)
            //{
            //    _owner.Name = Guid.NewGuid().ToString("N");
            //    var roles = MembershipService.QueryRoles(new RoleRequest() { Name = RoleNames.Account }).ToList();
            //    MembershipService.CreateUser(_owner);
            //    MembershipService.AssignRoles(_owner, roles.Select(x => x.RoleId).ToArray());
            //    _account.OwnerId = _owner.UserId;
            //}

            // sale Id
            //
            if (SaleId > 0)
            {
                var sale = MembershipService.GetUserById(SaleId) as AdminUser;
                if (sale != null && sale.IsSale == true)
                {
                    _account.SalerId = sale.UserId;
                }
            }

            DealLogService.Create(_dealLog);
            AccountService.Update(_account);

            return ResponseCode.Success;
        }

        private Account _account;
        private AdminUser _operator;
        private AccountType _accountType;
        AccountLevelPolicy _accountLevel;
        DealLog _dealLog;
        private DealWay _dealWay;
        private AccountUser _owner;

        public int Validate()
        {
            _account = AccountService.GetByName(AccountName);

            if (_account == null || (_account.State != AccountStates.Ready && _account.State != AccountStates.Saled))
            {
                return ResponseCode.NonFoundAccount;
            }

            _operator = MembershipService.GetUserById(OperatorUserId) as AdminUser;
            if (_operator == null)
            {
                return ResponseCode.InvalidateUser;
            }
            _dealWay = DealWayService.GetById(DealWayId);

            _accountType = AccountTypeService.GetById(_account.AccountTypeId);
            _accountLevel = AccountLevelPolicyService.Query().FirstOrDefault(x => x.Level == 0 && x.State == States.Normal && x.AccountTypeId == _account.AccountTypeId);

            _dealLog = new DealLog(SerialNo, DealTypes.Open, -_account.Amount, 0, null, null, _account, null, 0);

            // 用户
            if (!string.IsNullOrEmpty(UserDisplayName))
            {
                _owner = new AccountUser();
                _owner.DisplayName = UserDisplayName;
                _owner.IdentityCard = Identify;
                _owner.BirthDate = BirthDate;
                _owner.Mobile = Mobile;
                if (!string.IsNullOrWhiteSpace(Mobile))
                {
                    _owner.IsMobileAvailable = true;
                }
                _owner.Gender = Gender;
            }

            return ResponseCode.Success;
        } 
        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }
        [NoRender, Dependency]
        public IDealWayService DealWayService { get; set; }

        [NoRender, Dependency]
        public IAccountTypeService AccountTypeService { get; set; }

        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }

        [NoRender, Dependency]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        [NoRender, Dependency]
        public IPrintTicketService PrintTicketService { get; set; }

        [NoRender, Dependency]
        public Site HostSite { get; set; }

        [NoRender, Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public DealWay DealWay
        {
            get { return _dealWay; }
        }

        public AccountType AccountType
        {
            get
            {
                return _accountType;
            }
        }

        public Account Account
        {
            get
            {
                return _account;
            }
        }

        public DealLog DealLog
        {
            get { return _dealLog; }
        }

        public AccountUser Owner
        {
            get
            {
                return _owner;
            }
        }

        public AdminUser OperatorUser
        {
            get
            {
                return _operator;
            }
        }
    }
}