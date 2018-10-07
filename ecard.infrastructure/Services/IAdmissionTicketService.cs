using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IAdmissionTicketService
    {
        AdmissionTicket GetById(int id);

        AdmissionTicket GetFirst();

        void Update(AdmissionTicket item);

        DataTables<AdmissionTicket> Query(AdmissionTicketRequest request);

        void Create(AdmissionTicket item);
        void Delete(AdmissionTicket item);

        List<AdmissionTicket> GetNormalALL();

    }
}
