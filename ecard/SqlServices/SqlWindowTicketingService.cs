using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlWindowTicketingService : IWindowTicketingService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "WindowTicketings";
        public SqlWindowTicketingService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Insert(WindowTicketing item)
        {
            _databaseInstance.Insert(item,TableName);
        }
    }
}
