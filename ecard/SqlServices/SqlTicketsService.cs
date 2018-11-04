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

        public DataTables<Ticketss> GetList(TicketsRequest request)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@Bdate",request.Bdate),
                                      new SqlParameter("@code",request.code),
                                      new SqlParameter("@Edate",request.Edate),
                                      new SqlParameter("@mobile",request.mobile),
                                      new SqlParameter("@orderNo",request.orderNo),
                                      new SqlParameter("@state",request.state),
                                      new SqlParameter("@ticketName",request.ticketName),
                                      new SqlParameter("@useScope",request.useScope),
                                      new SqlParameter("@pageIndex",request.pageIndex),
                                      new SqlParameter("@pageSize",request.pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getTicketss", param);
            return _databaseInstance.GetTables<Ticketss>(sp);
        }

        public Ticketss GetByCode(string code)
        {
            string sql = @"select t.*,a.name as TicketName,a.introduce,(select top 1 s.DisplayName as shopName from Shops s where s.Name=t.useScope) as shopName,u.DisplayName as UserDisplayName,u.Mobile,u.babyName,u.babySex 
from  Tickets t left join AdmissionTicket a on t.AdmissionTicketId=a.id  left join Users u on t.userId=u.UserId where code = @code";
            return new QueryObject<Ticketss>(_databaseInstance, sql, new { code = code }).FirstOrDefault();
        }

        public Tickets GetByCodeModel(string code)
        {
            return new QueryObject<Tickets>(_databaseInstance, string.Format("select * from  {0} where code = @code", TableName), new { code = code }).FirstOrDefault();
        }
    }
}
