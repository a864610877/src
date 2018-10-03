using System;
using System.Runtime.Serialization;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class RechargingCommand : Command, ICommand
    {
        private Account _account;
        private AccountType _accountType;
        private AccountUser _owner;
        private DealWay _dealWay;
        private User _operatorUser;

        public RechargingCommand()
        {
        }

        public RechargingCommand(string serialNo, string accountName, decimal amount, bool hasReceipt, int dealWay, int operatorId,Account account)
        {
            SerialNo = serialNo;
            AccountName = accountName;
            Amount = amount;
            HasReceipt = hasReceipt;
            OperatorUserId = operatorId;
            HowToDeal = dealWay;
            _account = account;
        }

        public User OperatorUser
        {
            get { return _operatorUser; }
        }

        [DataMember]
        public string SerialNo { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public bool HasReceipt { get; set; }

        [DataMember]
        public int OperatorUserId { get; set; }

        [DataMember]
        public int HowToDeal { get; set; }

        [DataMember]
        public bool IsCash { get; set; }

        #region Dependencies

        [Dependency, NoRender]
        public IAccountService AccountService { get; set; } 

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }

        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }

        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }

        public Account Account
        {
            get { return _account; }
        }

        public AccountUser Owner
        {
            get { return _owner; }
        }

        public AccountType AccountType
        {
            get { return _accountType; }
        }

        public DealWay DealWay
        {
            get
            {
                return _dealWay;
            }
        }

        #endregion

        public int Execute(User @operatorUser)
        {
            _operatorUser = @operatorUser ?? _operatorUser;
            _account.Amount += Amount;
            AccountService.Update(_account);
            var systemDealLog = new SystemDealLog(SerialNo, _operatorUser)
            {
                Amount = Amount,
                DealType = SystemDealLogTypes.Recharge,
                HasReceipt = HasReceipt,
                Addin = _account.AccountId.ToString(),
                DealWayId = this.HowToDeal
            };
            SystemDealLogService.Create(systemDealLog);

            DealLog dealLog = CreateDealLog();
            dealLog.Addin = systemDealLog.SystemDealLogId;
            DealLogService.Create(dealLog);
            if (!string.IsNullOrWhiteSpace(CurrentSite.MessageTemplateOfRecharge))
            {
                if (_owner != null && _accountType.IsSmsRecharge && _owner.IsMobileAvailable)
                {
                    string message = MessageFormator.FormatTickForRecharging(CurrentSite.MessageTemplateOfRecharge,
                                                                             CurrentSite, HasReceipt, Amount,
                                                                             _dealWay!=null?_dealWay.DisplayName:"", dealLog, _account, AccountType,
                                                                             _owner, _operatorUser);
                    SmsHelper.Send(_owner.Mobile, message);
                }
            }
            return ResponseCode.Success;
        }

        public DealLog CreateDealLog()
        {
            var dealLog = new DealLog(SerialNo)
                              {
                                  Account = _account,
                                  Addin = 0,
                                  Amount = -Amount,
                                  DealType = DealTypes.Recharging,
                                  Point = 0,
                                  SourcePosId = 0,
                                  SourcePosName = "",
                                  SourceShopDisplayName = "",
                                  State = DealLogStates.Normal,
                                  SourceShopId = 0,
                                  SourceShopName = "",
                                  SubmitTime = DateTime.Now,
                              };
            return dealLog;
        }

        public int Validate()
        {
            if (Amount <= 0)
                return ResponseCode.InvalidateAmount;

            _account = AccountService.GetByName(AccountName);
            if (_account == null)
                return ResponseCode.NonFoundAccount;
            if (_account.State != States.Normal)
                return ResponseCode.AccountStateInvalid;

            if (_account.OwnerId.HasValue)
                _owner = MembershipService.GetUserById(_account.OwnerId.Value) as AccountUser;
            _accountType = AccountTypeService.GetById(_account.AccountTypeId);

            if (AccountType != null && !AccountType.IsRecharging)
            {
                return ResponseCode.NonRecharging;
            }

            _operatorUser = MembershipService.GetUserById(OperatorUserId);
            if (_operatorUser == null || !(_operatorUser is AdminUser))
                return ResponseCode.InvalidateUser;

            //_dealWay = DealWayService.GetById(this.HowToDeal);
            //if (_dealWay == null || _dealWay.State != DealWayStates.Normal)
            //    return ResponseCode.InvalidateDealWay;
            return ResponseCode.Success;
        }
        public int Validates()
        {
            if (_account.OwnerId.HasValue)
                _owner = MembershipService.GetUserById(_account.OwnerId.Value) as AccountUser;
            _operatorUser = MembershipService.GetUserById(OperatorUserId);
            if (_operatorUser == null || !(_operatorUser is AdminUser))
                return ResponseCode.InvalidateUser;
            _dealWay = DealWayService.GetById(this.HowToDeal);
            if (_dealWay == null || _dealWay.State != DealWayStates.Normal)
                return ResponseCode.InvalidateDealWay;
            return ResponseCode.Success;
        } 
    }
}