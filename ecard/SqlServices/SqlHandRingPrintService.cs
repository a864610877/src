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
    public class SqlHandRingPrintService : IHandRingPrintService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "HandRingPrint";
        public SqlHandRingPrintService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public HandRingPrint GetById(int id)
        {
            return _databaseInstance.GetById<HandRingPrint>(TableName, id);
        }

        public void Insert(HandRingPrint item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public void Update(HandRingPrint item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public DataTables<HandRingPrint> GetList(HandRingPrintRequest request)
        {
            SqlParameter[] param = {
                                      new SqlParameter("@Bdate",request.Bdate),
                                      new SqlParameter("@code",request.code),
                                      new SqlParameter("@Edate",request.Edate),
                                      new SqlParameter("@mobile",request.mobile),
                                      new SqlParameter("@babyName",request.babyName),
                                      new SqlParameter("@state",request.state),
                                      new SqlParameter("@shopId",request.shopId),
                                      new SqlParameter("@ticketType",request.ticketType),
                                      new SqlParameter("@pageIndex",request.pageIndex),
                                      new SqlParameter("@pageSize",request.pageSize)
                                   };
            StoreProcedure sp = new StoreProcedure("P_getHandRingPrint", param);
            return _databaseInstance.GetTables<HandRingPrint>(sp);
        }
    }
}
