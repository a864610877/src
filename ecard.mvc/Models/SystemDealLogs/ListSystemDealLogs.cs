using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.SystemDealLogs
{
    public class ListSystemDealLogs : EcardModelListRequest<ListSystemDealLog>
    {
        private Bounded _dealTypeBounded;
        private string _userName;

        public ListSystemDealLogs()
        {
            OrderBy = "SystemDealLogId desc";
        }

        [Dependency, NoRender]
        public ISystemDealLogService SystemDealLogService { get; set; }

        public string UserName
        {
            get { return _userName.TrimSafty(); }
            set { _userName = value; }
        }

        [DataType(DataType.Date)]
        public DateTime? StartTime { get; set; }

        [NoRender, Dependency]
        public SmsHelper SmsHelper { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }

        public string SerialNo { get; set; }

        public Bounded DealType
        {
            get
            {
                if (_dealTypeBounded == null)
                {
                    _dealTypeBounded = Bounded.Create<SystemDealLog>("DealType", Globals.All);
                }
                return _dealTypeBounded;
            }
            set { _dealTypeBounded = value; }
        }

        [NoRender, Dependency]
        public Site CurrentSite { get; set; }

        [Dependency, NoRender]
        public LogHelper LogHelper { get; set; }

        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }

        public void Ready()
        {
            DealType.Bind(null, true);
        }

        public void Query()
        {
            var request = new SystemDealLogRequest();
            if (DealType != Globals.All)
                request.DealType = DealType;
            if (State != SystemDealLogStates.All)
                request.State = State;
            request.SubmitTimeMin = StartTime;
            request.SubmitTimeMax = EndTime;
            request.UserName = UserName;
            request.SerialNo = SerialNo;
            QueryObject<SystemDealLog> query = SystemDealLogService.Query(request);
            List = query.ToList(this, x => new ListSystemDealLog(x));
            var dealways = DealWayService.Query().ToList();
            foreach (var dealLog in List)
            {
                var dealWay = dealways.FirstOrDefault(x => x.DealWayId == dealLog.InnerObject.DealWayId);
                dealLog.HowToDeal = dealWay == null ? dealLog.InnerObject.HowToDeal : dealWay.DisplayName;
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("export", null, new { export = "excel" }) { IsPost = true };
        }

        private Bounded _stateBounded;

        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.Create<SystemDealLog>("State", SystemDealLogStates.All);
                }
                return _stateBounded;
            }
            set { _stateBounded = value; }
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListSystemDealLog item)
        {
            if (item.HasReceipt != true && item.InnerObject.Amount > 0 && item.InnerObject.DealType == SystemDealLogTypes.Recharge && item.InnerObject.State == SystemDealLogStates.Normal)
                yield return new ActionMethodDescriptor("OpenReceipt", null, new { id = item.SystemDealLogId });
            if (item.InnerObject.CanCancel(SystemDealLogTypes.Recharge, CurrentSite))
                yield return new ActionMethodDescriptor("CloseRecharging", null, new { id = item.SystemDealLogId });
        }

        public object OpenReceipt(int id)
        {
            try
            {
                SystemDealLog dealLog = SystemDealLogService.GetById(id);
                if (dealLog != null && !dealLog.HasReceipt && dealLog.DealType == SystemDealLogTypes.Recharge && dealLog.Amount > 0 && dealLog.State == SystemDealLogStates.Normal)
                {
                    dealLog.HasReceipt = true;

                    TransactionHelper.BeginTransaction();

                    SystemDealLogService.Update(dealLog);
                    LogHelper.LogWithSerialNo(LogTypes.SystemDealLogOpenReceipt, SerialNoHelper.Create(), dealLog.SystemDealLogId, dealLog.SerialNo);
                    AddMessage(Localize("OpenReceipt.success"), dealLog.SystemDealLogId);

                    SendMessage(dealLog);
                    return TransactionHelper.CommitAndReturn(new SimpleAjaxResult());
                }
                return new SimpleAjaxResult(Localize("OpenReceipt.failed", "原交易不存在"));
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.SystemDealLogOpenReceipt, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }

        private void SendMessage(SystemDealLog dealLog)
        {
            if (string.IsNullOrWhiteSpace(CurrentSite.MessageTemplateOfOpenReceipt))
                return;

            int accountId = Convert.ToInt32(dealLog.Addin);
            Account account = AccountService.GetById(accountId);
            if (account.OwnerId == null || !account.IsMessageOfDeal) return;
            var user = MembershipService.GetUserById(account.OwnerId.Value) as AccountUser;
            if (user == null || !user.IsMobileAvailable) return;

            string msg = MessageFormator.Format(CurrentSite.MessageTemplateOfOpenReceipt, user);
            msg = MessageFormator.Format(msg, account);
            msg = MessageFormator.Format(msg, CurrentSite);
            SmsHelper.Send(user.Mobile, msg);
        }
    }
}