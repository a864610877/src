using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.CashDealLogSummarys
{

    public class ListCashDealLogSummarys : EcardModelListRequest<ListCashDealLogSummary>
    {
        public ListCashDealLogSummarys()
        {
            OrderBy = "SubmitDate";
            this.SubmitTimeMax = DateTime.Now.Date;
            this.SubmitTimeMin = DateTime.Now.Date;
        }

        [DataType(DataType.Date)]
        public DateTime? SubmitTimeMax { get; set; }
        [DataType(DataType.Date)]
        public DateTime? SubmitTimeMin { get; set; }

        [Dependency, NoRender]
        public ICashDealLogSummaryService CashDealLogSummaryService { get; set; }

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        public void Ready()
        {
        }

        public void Query()
        {
            var request = new CashDealLogRequest(); 
            request.SubmitTimeMax = SubmitTimeMax;
            request.SubmitTimeMin = SubmitTimeMin;
            var query = this.CashDealLogSummaryService.Query(request);

            // fill condition
            List = query.ToList(this, x => new ListCashDealLogSummary(x));

            var ownerIds = List.Select(x => x.InnerObject.OwnerId).Distinct().ToArray();

            var users = this.MembershipService.QueryUsers<AdminUser>(new UserRequest { UserIds = ownerIds.Distinct().ToArray() }).ToList();
            foreach (var item in List)
            {
                var user = users.FirstOrDefault(x => x.UserId == item.InnerObject.OwnerId);
                if (user != null)
                    item.OwnerName = user.DisplayName + "(" + user.Name + ")";
                if (item.Amount <= 0)
                    item.InnerObject.State = CashDealLogSummaryStates.Normal;
                else
                    item.InnerObject.State = CashDealLogSummaryStates.Invalid;
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield break;
        }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListCashDealLogSummary item)
        {
            if (item.InnerObject.State != CashDealLogSummaryStates.Normal)
                yield return new ActionMethodDescriptor("Done", "CashDeallog", new { submitDate = item.SubmitDate ,ownerId=item.InnerObject.OwnerId});
            yield break;
        }

        private Bounded _state;

        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<CashDealLogSummary>("State", CashDealLogSummaryStates.All);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}
