using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.Sites
{
    public class ListMessageAccount
    {
        private readonly Account InnerObject;

        public ListMessageAccount(Account account)
        {
            InnerObject = account;
        }
        [NoRender]
        public int AccountId
        {
            get { return InnerObject.AccountId; }
            set { InnerObject.AccountId = value; }
        }
        public string Name
        {
            get { return InnerObject.Name; }
            set { InnerObject.Name = value; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        public DateTime ExpiredDate
        {
            get { return InnerObject.ExpiredDate; }
            set { InnerObject.ExpiredDate = value; }
        }


        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }

    }
}