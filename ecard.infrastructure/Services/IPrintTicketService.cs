using Ecard.Models;

namespace Ecard.Services
{
    public interface IPrintTicketService
    {
        QueryObject<PrintTicket> Query(PrintTicketRequest request);
        void Create(PrintTicket item);
        PrintTicket GetById(int id);
        void Update(PrintTicket item);
        void Delete(PrintTicket item);
    }
}