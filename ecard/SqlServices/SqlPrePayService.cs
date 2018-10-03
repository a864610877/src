using System;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlPrePayService : IPrePayService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "PrePays";

        public SqlPrePayService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<PrePay> Query(PrePayRequest request)
        {
            return new QueryObject<PrePay>(_databaseInstance, "PrePay.query", request);
        }

        public void Create(PrePay item)
        {
            item.UpdateTime = item.SubmitTime;
            item.PrePayId = _databaseInstance.Insert(item, TableName);
        }

        public PrePay GetById(int id)
        {
            return _databaseInstance.GetById<PrePay>(TableName, id);
        }

        public void Update(PrePay item)
        { 
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(PrePay item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    } 
}