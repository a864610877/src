using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit;

namespace Ecard.Mvc.Models.CashDealLogs
{
    public class DoneCashDealLog : CashDealLogModelBase
    {
        [Hidden]
        public int OwnerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; internal set; }
        public DoneCashDealLog()
        {
            InnerObject.State = CashDealLogStates.Normal;
        }

        public IMessageProvider Done()
        {
            this.Ready();
            if (this.ShouldPayAmount < this.Amount)
            {
                AddError(LogTypes.CashDealLogDone, "amountInvalidate", ShouldPayAmount, Amount);
                return this;
            }
            var serialNo = SerialNoHelper.Create();
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            var amount = Amount;
            using (var tran = TransactionHelper.BeginTransaction())
            {
                // InnerObject.AccountLevel = AccountLevel;
                InnerObject.DealType = CashDealLogTypes.EmployeeDeposit;
                InnerObject.UserId = user.UserId;
                InnerObject.OwnerId = OwnerId;
                InnerObject.Amount = -amount;
                InnerObject.SubmitTime = DateTime.Now;
                InnerObject.SubmitDate = this.SubmitDate;
                CashDealLogService.Create(InnerObject);
                AddMessage("success", ShouldPayAmount, amount);
                Logger.LogWithSerialNo(LogTypes.CashDealLogDone, serialNo, InnerObject.CashDealLogId /*, InnerObject.DisplayName*/);
                Amount = amount;
                tran.Commit();
                return this;
            }
        }

        public string OwnerName { get; internal set; }

        public string DealType
        {
            get { return "现金还款"; }
        }

        [Range(1, int.MaxValue, ErrorMessage = "至少要还1元钱")]
        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }

        public decimal ShouldPayAmount { get; internal set; }
        public void Ready()
        {
            var owner = this.MembershipService.GetUserById(this.OwnerId);

            this.OwnerName = owner == null ? "查无此人" : owner.DisplayName + "(" + owner.Name + ")";
            var items = this.CashDealLogSummaryService.Query(new CashDealLogRequest()
                                                         {
                                                             SubmitTimeMin = this.SubmitDate,
                                                             SubmitTimeMax = SubmitDate,
                                                             OwnerId = OwnerId
                                                         }).ToList();
            if (items.Count > 0)
            {
                var cashDealLogSummary = items.First();

                this.ShouldPayAmount = cashDealLogSummary.Amount;
            }
        }
    }
}