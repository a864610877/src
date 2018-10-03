using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Accounts
{
    public class RebateAccount : ViewModelBase
    {
        private Account _innerObject;

        public RebateAccount()
        {
            _innerObject = new Account();
        }

        public void Ready()
        {
            var accountLevel =
                AccountLevelPolicyService.Query().FirstOrDefault(
                    x => x.State == AccountLevelPolicyStates.Normal && x.Level == _innerObject.AccountLevel && x.AccountTypeId == _innerObject.AccountTypeId);

            var owner = (AccountUser)(_innerObject.OwnerId.HasValue ? this.MembershipService.GetUserById(_innerObject.OwnerId.Value) : null);
            var levels = PointRebateService.Query().Where(x => x.IsFor(_innerObject, owner, accountLevel, DateTime.Now) && x.Point < _innerObject.Point)
                .Select(x => new IdNamePair() { Key = x.PointRebateId, Name = x.DisplayName });
            PointRebate.Bind(levels);
        }

        [NoRender]
        public Account InnerObject
        {
            get { return _innerObject; }
        }

        public string AccountName
        {
            get { return InnerObject.Name; }
        }
        public decimal TotalAmount
        {
            get { return InnerObject.TotalAmount; }
        }
        public decimal Point
        {
            get { return InnerObject.Point; }
        }
        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }
        public string ExpiredDate
        {
            get { return InnerObject.ExpiredDate.ToColumnDate(); }
        }
        private Bounded _pointRebateBounded;

        public Bounded PointRebate
        {
            get
            {
                if (_pointRebateBounded == null)
                {
                    _pointRebateBounded = Bounded.CreateEmpty("PointRebateId", 0);
                }
                return _pointRebateBounded;
            }
            set { _pointRebateBounded = value; }
        }
        protected void SetInnerObject(Account item)
        {
            _innerObject = item;
        }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IPointRebateLogService PointRebateLogService { get; set; }
        [Hidden]
        public int AccountId
        {
            get { return InnerObject.AccountId; }
            set { InnerObject.AccountId = value; }
        }
        [Dependency, NoRender]
        public IPointRebateService PointRebateService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; } 
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Read(int id)
        {
            SetInnerObject(AccountService.GetById(id));
        }

        public IMessageProvider Save()
        {
            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            var pointRebate = PointRebateService.GetById(this.PointRebate);
            if (pointRebate == null)
            {
                AddError(LogTypes.AccountRebate, "NoPointRebate");
                return this;
            }
            var account = AccountService.GetById(AccountId);
            if (account == null || account.Point < pointRebate.Point)
            {
                AddError(LogTypes.AccountRebate, "NoPointRebate");
                return this;
            }
            
            var accountLevel =
                 AccountLevelPolicyService.Query().FirstOrDefault(
                     x => x.State == AccountLevelPolicyStates.Normal && x.Level == account.AccountLevel && x.AccountTypeId == account.AccountTypeId);

            var owner = (AccountUser)(_innerObject.OwnerId.HasValue ? this.MembershipService.GetUserById(_innerObject.OwnerId.Value) : null);
            var pointRebates = PointRebateService.Query().Where(x => x.IsFor(account, owner, accountLevel, DateTime.Now) && x.Point < account.Point).ToList();

            if (pointRebates.Count == 0)
            {
                AddError(LogTypes.AccountRebate, "NoPointRebate");
                return this;
            }

            if (!pointRebates.Any(x => x.PointRebateId == pointRebate.PointRebateId))
            {
                AddError(LogTypes.AccountRebate, "NoPointRebate");
                return this;
            }

            account.Amount += pointRebate.Amount;
            account.Point -= pointRebate.Point;

            AccountService.Update(account);
            var dealLog = new DealLog(serialNo, DealTypes.Rebate, -pointRebate.Amount, -pointRebate.Point, null, null, account, null, pointRebate.PointRebateId);
            DealLogService.Create(dealLog);
            SystemDealLogService.Create(new SystemDealLog(serialNo, SecurityHelper.GetCurrentUser()) { Amount = pointRebate.Amount, DealType = SystemDealLogTypes.Rebate, Addin = dealLog.DealLogId.ToString() });
            Logger.LogWithSerialNo(LogTypes.AccountRebate, serialNo, account.AccountId, account.Name, pointRebate.DisplayName);
            PointRebateLogService.Create(new PointRebateLog(serialNo, account, SecurityHelper.GetCurrentUser().CurrentUser, pointRebate));
            AddMessage("success", pointRebate.Amount, pointRebate.Point);
            return TransactionHelper.CommitAndReturn(this);
        }
    }
}