using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlTicketsService : ITicketsService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Tickets";
        public SqlTicketsService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public Tickets GetById(int id)
        {
            return _databaseInstance.GetById<Tickets>(TableName, id);
        }

        public DataTables<Ticketss> GetList(int userId, int pageIndex = 1, int pageSize = 10)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@userId",userId),
                                      new SqlParameter("@pageIndex",pageIndex),
                                      new SqlParameter("@pageSize",pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getTickets", param);
            return _databaseInstance.GetTables<Ticketss>(sp);
        }

        public void Create(Tickets item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public void Update(Tickets item)
        {
            _databaseInstance.Update(item, TableName);
        }
    }
}
