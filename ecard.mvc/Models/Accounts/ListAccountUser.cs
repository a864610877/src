using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class ListAccountUser
    {
        private readonly AccountUser _innerObject;

        [NoRender]
        public AccountUser InnerObject
        {
            get { return _innerObject; }
        }

        public ListAccountUser()
        {
            _innerObject = new AccountUser();
        }

        public ListAccountUser(AccountUser account)
        {
            _innerObject = account;
        }
        [NoRender]
        public int UserId
        {
            get { return InnerObject.UserId; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        public string Mobile
        {
            get { return InnerObject.Mobile; }
        }
        public string babyName
        {
            get { return InnerObject.babyName; }
        }
        public string babySex
        {
            get { return InnerObject.babySex==Genders.Male?"男":InnerObject.babySex== Genders .Female?"女":""; }
        }
        public DateTime? BirthDate
        {
            get { return InnerObject.BirthDate; }
        }
    }
}
