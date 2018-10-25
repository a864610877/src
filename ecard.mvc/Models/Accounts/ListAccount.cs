using System;
using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.Accounts
{
    public class ListAccount
    {
        private readonly AccountWithOwner _innerObject;

        [NoRender]
        public AccountWithOwner InnerObject
        {
            get { return _innerObject; }
        }

        public ListAccount()
        {
            _innerObject = new AccountWithOwner();
        }

        public ListAccount(AccountWithOwner account)
        {
            _innerObject = account;
        }

        public string Name
        {
            get { return InnerObject.Name; }
        }

        public string DisplayName
        {
            get { return InnerObject.OwnerDisplayName; }
        }

        public string Mobile
        {
            get { return InnerObject.OwnerMobileNumber; }
        }
        public int TotalTimes
        {
            get { return InnerObject.TotalTimes; }
        }
        public int Frequency
        {
            get { return InnerObject.Frequency; }
        }
        public int FrequencyUsed
        {
            get { return InnerObject.FrequencyUsed; }
        }
        public decimal SinglePrice
        {
            get { return InnerObject.SinglePrice; }
        }
        public decimal SaleAmount
        {
            get { return InnerObject.SaleAmount; }
        }
        public DateTime babyBirthDate
        {
            get { return InnerObject.babyBirthDate; }
        }
        public string BabyName
        {
            get { return InnerObject.BabyName; }
        }
        public string BabySex
        {
            get { return InnerObject.BabySex==1?"ÄÐ":"Å®"; }
        }
        public string shopName
        {
            get { return InnerObject.shopName; }
        }

        //[CheckPermission(Ecard.Permissions.AccountInitPassword)]
        //public string InitPassword
        //{
        //    get { return InnerObject.InitPassword; }
        //}
        //public decimal Amount
        //{
        //    get { return InnerObject.Amount; }
        //}
        //[NoRender]
        //public string ShopName { get; set; }
        //public string DistributorName { get; set; }

        //public decimal TotalAmount
        //{
        //    get { return InnerObject.TotalAmount; }
        //}

        //public decimal DepositAmount
        //{
        //    get { return InnerObject.DepositAmount; }
        //}

        //public string AccountLevelName
        //{
        //    get { return InnerObject.AccountLevelName; }
        //}

        public DateTime? OpenTime
        {
            get { return InnerObject.OpenTime; }
        }
        //[CheckPermission(Ecard.Permissions.AccountToken)]
        //public string AccountToken
        //{
        //    get { return InnerObject.AccountToken; }
        //}
        //public int Point
        //{
        //    get { return InnerObject.Point; }
        //}
        [Sort("AccountTypeId")]
        public string AccountType { get; set; }
        public string ExpiredDate
        {
            get
            {
                if (InnerObject.State == AccountStates.Normal || InnerObject.State == AccountStates.Invalid)
                    return InnerObject.ExpiredDate.ToColumnDate();
                return null;
            }
        }
        public string LastDealTime
        {
            get
            {
                return (InnerObject.State == AccountStates.Normal || InnerObject.State == AccountStates.Invalid || InnerObject.State == AccountStates.Closed)
                    ? InnerObject.LastDealTime.FormatForTime() : "";
            }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
        [NoRender]
        public int AccountId
        {
            get { return InnerObject.AccountId; }
        }
        [NoRender]
        public string boor { get; set; }
        
    }
}