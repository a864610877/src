using Ecard.Models;
using Ecard.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.AdmissionTickets
{
    public class AdmissionTicketCreate : AdmissionTicketModelBase, IValidator
    {
        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
        public IMessageProvider Save()
        {
            var model = new AdmissionTicket();
            model.introduce = this.introduce;
            model.name = this.name;
            model.state = AdmissionTicketState.Invalid;
            model.weekendAmount = this.weekendAmount;
            model.adultNum = this.adultNum;
            model.amount = this.amount;
            model.childNum = this.childNum;
            model.crateTime = DateTime.Now;
            AdmissionTicketService.Create(model);
            AddMessage("success", model.name);
            Logger.LogWithSerialNo(LogTypes.AddAdmissionTicketCreate, model.name, InnerObject.id, model);
            return this;
        }
    }
}
