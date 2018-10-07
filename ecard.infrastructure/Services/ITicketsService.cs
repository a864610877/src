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
        DataTables<Tickets> GetList(int userId, int pageIndex = 1, int pageSize = 10);
        Tickets GetById(int id);
        void Create(Tickets item);
        void Update(Tickets item);
    }
}
