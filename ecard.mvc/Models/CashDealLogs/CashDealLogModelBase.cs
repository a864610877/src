using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.CashDealLogs
{
    public class CashDealLogModelBase : ViewModelBase
    {
        private CashDealLog _innerObject;

        public CashDealLogModelBase()
        {
            _innerObject = new CashDealLog();
        }

        public CashDealLogModelBase(CashDealLog shop)
        {
            _innerObject = shop;
        }

        [NoRender]
        public CashDealLog InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(CashDealLog item)
        {
            _innerObject = item;
        }

        [Dependency, NoRender]
        public ICashDealLogService CashDealLogService { get; set; }
        [Dependency, NoRender]
        public ICashDealLogSummaryService CashDealLogSummaryService { get; set; }

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }


    }
}