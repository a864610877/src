using Ecard.Models;

namespace Ecard.Mvc.Models.AccountTypes
{
    public class ListAccountType
    {
        private readonly AccountType _innerObject;

        [NoRender]
        public AccountType InnerObject
        {
            get { return _innerObject; }
        }

        public ListAccountType()
        {
            _innerObject = new AccountType();
        }

        public ListAccountType(AccountType innerObject)
        {
            _innerObject = innerObject;
        }

        [NoRender]
        public int AccountTypeId
        {
            get { return InnerObject.AccountTypeId; }
        }
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        //public bool IsRecharging
        //{
        //    get { return InnerObject.IsRecharging; }
        //}


        //public bool IsRenew
        //{
        //    get { return InnerObject.IsRenew; }
        //}


        //public bool IsPointable
        //{
        //    get { return InnerObject.IsPointable; }
        //} 


        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }


        //public decimal DepositAmount
        //{
        //    get { return InnerObject.DepositAmount; }
        //}

        //public int Point
        //{
        //    get { return InnerObject.Point; }
        //}

        public int ExpiredMonths
        {
            get { return InnerObject.ExpiredMonths; }
        }

        public int Frequency
        {
            get { return InnerObject.Frequency; }
        }

        public int NumberOfPeople
        {
            get { return InnerObject.NumberOfPeople; }
        }
        
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}