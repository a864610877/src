using Ecard.Models;

namespace Ecard.Services
{
    public class SqlPrintTicketService : IPrintTicketService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "PrintTickets";

        public SqlPrintTicketService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<PrintTicket> Query(PrintTicketRequest request)
        {
            return new QueryObject<PrintTicket>(_databaseInstance, "PrintTicket.query", request);
        }

        public void Create(PrintTicket item)
        {
            item.PrintTicketId = _databaseInstance.Insert(item, TableName);
        }

        public PrintTicket GetById(int id)
        {
            return _databaseInstance.GetById<PrintTicket>(TableName, id);
        }

        public void Update(PrintTicket item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(PrintTicket item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}