using Ecard.Infrastructure;
using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface ITicketsService
    {
        DataTables<Ticketss> GetList(int userId, int pageIndex = 1, int pageSize = 10);
        Tickets GetById(int id);
        void Create(Tickets item);
        void Update(Tickets item);
        DataTables<Ticketss> GetList(TicketsRequest request);
        Ticketss GetByCode(string code);
        Tickets GetByCodeModel(string code);
    }

    public class TicketsRequest
    {
        public string ticketName { get; set; }
        public string orderNo { get; set; }
        public string mobile { get; set; }
        public string code { get; set; }
        public int? state { get; set; }
        public string useScope { get; set; }
        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }


}
