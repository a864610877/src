using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.AdmissionTickets
{
    public class AdmissionTicketEdit:ViewModelBase,IValidator
    {
        private AdmissionTicket _innerObject;
        [NoRender]
        public AdmissionTicket InnerObject
        {
            get { return _innerObject; }
        }
        public AdmissionTicketEdit()
        {
            _innerObject = new AdmissionTicket();
        }

        public AdmissionTicketEdit(AdmissionTicket shop)
        {
            _innerObject = shop;
        }
        [Dependency, NoRender]
        public IAdmissionTicketService AdmissionTicketService { get; set; }

        [Hidden]
        public int Id
        {
            get { return InnerObject.id; }
            set { InnerObject.id = value; }
        }

        public string Name
        {
            get { return InnerObject.name; }
            set { InnerObject.name = value; }
        }

        public int AdultNum
        {
            get { return InnerObject.adultNum; }
            set { InnerObject.adultNum = value; }
        }
        public int ChildNum
        {
            get { return InnerObject.childNum; }
            set { InnerObject.childNum = value; }
        }
        //public decimal AddAdultAmount
        //{
        //    get { return InnerObject.addAdultAmount; }
        //    set { InnerObject.addAdultAmount = value; }
        //}
        public decimal Amount
        {
            get { return InnerObject.amount; }
            set { InnerObject.amount = value; }
        }
        public decimal WeekendAmount
        {
            get { return InnerObject.weekendAmount; }
            set { InnerObject.weekendAmount = value; }
        }
        [UIHint("richtext")]
        public string introduce
        {
            get { return InnerObject.introduce; }
            set { InnerObject.introduce = value; }
        }
        public void Read(int id)
        {
            var item = AdmissionTicketService.GetById(id);
            SetItem(item);
        }
        protected void SetInnerObject(AdmissionTicket item)
        {
            _innerObject = item;
        }
        private void SetItem(AdmissionTicket item)
        {
            this.SetInnerObject(item);
        }

        public IMessageProvider Save()
        {
            var item = AdmissionTicketService.GetById(Id);
            if (item != null)
            {
                //item.addAdultAmount = AddAdultAmount;
                item.adultNum = AdultNum;
                item.amount = Amount;
                item.childNum = ChildNum;
                item.introduce = introduce;
                item.name = Name;
                item.weekendAmount = WeekendAmount;
                AdmissionTicketService.Update(item);
                Logger.LogWithSerialNo(LogTypes.AdmissionTicketEdit, item.name, InnerObject.id, item);
            }
            AddMessage("success", this.Name);
            SetItem(item);
            return this;
        }
        //public string details
        //{
        //    get { return InnerObject.details; }
        //    set { InnerObject.details = value; }
        //}

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
    }
}
