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
    public class SqlAdmissionTicketService : IAdmissionTicketService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "AdmissionTicket";
        public SqlAdmissionTicketService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public Models.AdmissionTicket GetById(int id)
        {
            return _databaseInstance.GetById<AdmissionTicket>(TableName, id);
        }

        public void Update(Models.AdmissionTicket item)
        {
            _databaseInstance.Update(item, TableName);
        }


        public AdmissionTicket GetFirst()
        {
          return  _databaseInstance.Query<AdmissionTicket>("select * from AdmissionTicket", new { }).FirstOrDefault();
        }



        public DataTables<AdmissionTicket> Query(Requests.AdmissionTicketRequest request)
        {
            SqlParameter[] param = { 
                                       new SqlParameter("@name",request.name),
                                       new SqlParameter("@state",request.state),
                                       new SqlParameter("@startTime",request.startTime),
                                       new SqlParameter("@endTime",request.endTime),
                                       new SqlParameter("@pageIndex",request.PageIndex),
                                       new SqlParameter("@pageSize",request.PageSize),

                                   };
            StoreProcedure sp = new StoreProcedure("P_getAdmissionTickets", param);
            return _databaseInstance.GetTables<AdmissionTicket>(sp);
        }


        public void Create(AdmissionTicket item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public void Delete(AdmissionTicket item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public List<AdmissionTicket> GetNormalALL()
        {
            string sql = "select * from AdmissionTicket where state=@state";
            return new QueryObject<AdmissionTicket>(_databaseInstance, sql, new { state = AdmissionTicketState.Normal }).ToList();
        }
    }
}
