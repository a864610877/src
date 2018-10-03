﻿using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlCouponsService : ICouponsService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Coupons";
        public SqlCouponsService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public Models.Coupons GetById(int id)
        {
            return _databaseInstance.GetById<Coupons>(TableName, id);
        }

        public void Update(Models.Coupons item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public Infrastructure.DataTables<Models.Coupons> Query(Requests.CouponsRequest request)
        {
            SqlParameter[] param = { 
                                       new SqlParameter("@name",request.name),
                                       new SqlParameter("@code",request.code),
                                       new SqlParameter("@couponsType",request.couponsType),
                                       new SqlParameter("@state",request.state),
                                       new SqlParameter("@startTime",request.startTime),
                                       new SqlParameter("@endTime",request.endTime),
                                       new SqlParameter("@pageIndex",request.PageIndex),
                                       new SqlParameter("@pageSize",request.PageSize),

                                   };
            StoreProcedure sp = new StoreProcedure("P_getCoupons", param);
            return _databaseInstance.GetTables<Coupons>(sp);
        }

        public void Create(Models.Coupons item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }

        public void Delete(Models.Coupons item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}
