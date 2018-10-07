using Ecard.Models;
using Ecard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.SqlServices
{
    public class SqlUserCouponsService : IUserCouponsService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "UserCoupons";
        public SqlUserCouponsService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }
        public void Create(UserCoupons item)
        {
            item.id = _databaseInstance.Insert(item, TableName);
        }
        public UserCoupons GetById(int id)
        {
            return _databaseInstance.GetById<UserCoupons>(TableName, id);
        }
        public List<UserCouponss> GetUserId(int userId)
        {
            string sql = @"select uc.id,c.code,c.couponsType,c.name,(select DisplayName from Shops where Name=c.useScope) as shopName,
                           c.discount,c.deductibleAmount,c.fullAmount,c.reduceAmount,c.validity from UserCoupons uc join Coupons c on uc.couponsId=c.id
                           where validity<=@validity and c.[state]=1 and uc.state=1 and userId=@userId";
            return new QueryObject<UserCouponss>(_databaseInstance, sql, new { validity = DateTime.Now.Date, userId = userId }).ToList();
        }
        public void Update(UserCoupons item)
        {
            _databaseInstance.Update(item, TableName);
        }
    }
}
