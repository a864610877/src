using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit;

namespace Ecard.Mvc.Models.CashDealLogs
{
    public class CreateCashDealLog : CashDealLogModelBase
    {
        public CreateCashDealLog()
        {
            InnerObject.State = CashDealLogStates.Normal;
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            using (var tran = TransactionHelper.BeginTransaction())
            {
                // InnerObject.AccountLevel = AccountLevel;
                InnerObject.DealType = CashDealLogTypes.EmployeeLoan;
                InnerObject.UserId = user.UserId;
                InnerObject.OwnerId = this.Owner;
                CashDealLogService.Create(InnerObject);
                AddMessage("success" /*, InnerObject.DisplayName*/);
                Logger.LogWithSerialNo(LogTypes.CashDealLogCreate, serialNo, InnerObject.CashDealLogId /*, InnerObject.DisplayName*/);
                tran.Commit();
                return this;
            }
        }

        private Bounded _ownerBounded;

        public Bounded Owner
        {
            get
            {
                if (_ownerBounded == null)
                {
                    _ownerBounded = Bounded.CreateEmpty("OwnerId", Globals.All);
                }
                return _ownerBounded;
            }
            set { _ownerBounded = value; }
        }

        public string DealType
        {
            get { return "现金领取"; }
        }

        [Range(1, int.MaxValue, ErrorMessage = "至少要领1元钱")]
        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }

        public void Ready()
        {
            var adminUsers = this.MembershipService.QueryUsers<AdminUser>(new UserRequest() { State = UserStates.Normal }).ToList();

            this.Owner.Bind(adminUsers.Select(x => new IdNamePair() { Key = x.UserId, Name = string.Format("{0} ({1})", x.DisplayName, x.Name) }));
        }
    }
}