using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AccountTypes
{
    public class GetAccountTypeById : ViewModelBase
    {
        private AccountType InnerObject { get; set; }
        public void Read(int id)
        {
            var obj = AccountTypeService.GetById(id);
            if (obj.State == AccountTypeStates.Normal)
                InnerObject = obj;
        }

        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }
        public int ExpiredMonths
        {
            get { return InnerObject.ExpiredMonths; }
            set { InnerObject.ExpiredMonths = value; }
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }

        public decimal DepositAmount
        {
            get { return InnerObject.DepositAmount; }
            set { InnerObject.DepositAmount = value; }
        }
        public int Point
        {
            get { return InnerObject.Point; }
            set { InnerObject.Point = value; }
        }
        public int RenewMonths
        {
            get { return InnerObject.RenewMonths; }
            set { InnerObject.RenewMonths = value; }
        }
        public bool IsRecharging
        {
            get { return InnerObject.IsRecharging; }
            set { InnerObject.IsRecharging = value; }
        }
        public bool IsRenew
        {
            get { return InnerObject.IsRenew; }
            set { InnerObject.IsRenew = value; }
        }

        [Dependency]
        [NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
    }
}