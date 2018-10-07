using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Models.GoodandOrder;

namespace Ecard.Services
{
    public interface IOrder1Service
    {
        ClientAccount GetAccountByPhone(string phone);
        QueryObject<Order1> QueryOrder(OrderRequest request);
        QueryObject<Good> QueryGood(GoodRequest request);
        Good GetById(int id);
        /// <summary>
        /// 取得订单明细。
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        QueryObject<OrderDetial1> GetByorderId(string orderId);
        /// <summary>
        /// 取得指定商品
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        QueryObject<Good> GetGoodsByIds(int[] ids);
        /// <summary>
        /// 取得会员的所有订单。
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        QueryObject<Order1> GetOrderByAccountId(int accountId);
        /// <summary>
        /// 取得当天订单流水号
        /// </summary>
        /// <returns></returns>
        string GetOrderSerialnumber();
        void CreateGood(Good item);
        void CreateOrder(Order1 item);
        void AddOrderDetial(OrderDetial1 item);
        void UpdateGood(Good item);
        void UpdateOrder(Order1 item);
        void EditOrderDetial(OrderDetial1 item);
        void DeleteOrder(Order1 item);
        void DeleteGood(Good item);
        void DeleteOrderDetial(OrderDetial1 item);
        int DeleteOrderDetials(string OrderId);
    }
    public class GoodRequest
    {
        public string NameWith { get; set; }

    }
    public class OrderRequest
    {
        public string OrderId { get; set; }
        public int? Serialnumber { get; set; }
        public int? AccountId { get; set; }
        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }

    }
    public class SqlOrder1Service : IOrder1Service
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string OrderTable = "Orders";
        private const string OrderDetialTable = "OrderDetial";
        private const string GoodTable = "Goods";


        public SqlOrder1Service(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Order1> QueryOrder(OrderRequest request)
        {
            string sql = @"select * from Orders where 1=1
                         and (@AccountId is null or AccountId=@AccountId)
                         and (@OrderId is null or OrderId=@OrderId) and (@Serialnumber is null or Serialnumber=@Serialnumber)
                         and (@Bdate is null or SubmitTime >= @Bdate) and (@Edate is null or SubmitTime < @Edate)";
            return new QueryObject<Order1>(this._databaseInstance, sql, request);
        }

        public QueryObject<Good> QueryGood(GoodRequest request)
        {
            string sql = @"select * from Goods where 1=1
                         and (@NameWith is null or GoodName like '%@NameWith%')";
            return new QueryObject<Good>(this._databaseInstance, sql, request);
        }

        public Good GetById(int id)
        {
            var arg = new { id = id };
            var sql = "select * from Goods where 1=1 and (@id is null or GoodId=@id)";
            return new QueryObject<Good>(_databaseInstance, sql, arg).FirstOrDefault();
        }

        public QueryObject<OrderDetial1> GetByorderId(string orderId)
        {
            var arg = new { OrderId = orderId };
            string sql = @"select * from OrderDetial where 1=1
                           and (@orderId is null or OrderId=@orderId)";
            return new QueryObject<OrderDetial1>(this._databaseInstance, sql, arg);
        }

        public QueryObject<Good> GetGoodsByIds(int[] ids)
        {
            return new QueryObject<Good>(_databaseInstance, "select * from Goods where Goodid in (@ids)", new { ids = ids });
        }

        public QueryObject<Order1> GetOrderByAccountId(int accountId)
        {
            return new QueryObject<Order1>(_databaseInstance, "select * from Orders where AccountId =@accountId", new { AccountId = accountId });
        }

        public void CreateGood(Good item)
        {
            _databaseInstance.Insert(item, GoodTable);
        }

        public void CreateOrder(Order1 item)
        {
            _databaseInstance.Insert(item, OrderTable);
        }

        public void AddOrderDetial(OrderDetial1 item)
        {
            _databaseInstance.Insert(item, OrderDetialTable);
        }

        public void UpdateGood(Good item)
        {
            _databaseInstance.Update(item, GoodTable);
        }

        public void UpdateOrder(Order1 item)
        {
            _databaseInstance.Update(item, OrderTable);
        }

        public void EditOrderDetial(OrderDetial1 item)
        {
            _databaseInstance.Update(item, OrderDetialTable);
        }

        public void DeleteOrder(Order1 item)
        {
            _databaseInstance.Delete(item, OrderTable);
        }

        public void DeleteGood(Good item)
        {
            _databaseInstance.Delete(item, GoodTable);
        }

        public void DeleteOrderDetial(OrderDetial1 item)
        {
            _databaseInstance.Delete(item, OrderDetialTable);
        }


        public string GetOrderSerialnumber()
        {
            string sql = @"select isnull(max(substring(Orderid,11,5))+1,1) from Orders 
                        where substring(Orderid,2,8)is null or substring(Orderid,2,8)=Convert(varchar(10),Getdate(),112)";
            int num=(int)_databaseInstance.ExecuteScalar(sql,null);
            string snoTemp = num.ToString();
            for (int i = 0; i < 5 - num.ToString().Length; i++)
            {
                snoTemp = "0" + snoTemp;
            }
            return snoTemp;
        }


        public int DeleteOrderDetials(string OrderId)
        {
            string sql = string.Format(@"delete Orderdetial where OrderId='{0}'", OrderId);
            return _databaseInstance.ExecuteNonQuery(sql,null);
        }

        public ClientAccount GetAccountByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new ClientAccount();
            string sql = string.Format(@"select a.accountId,b.birthDate,b.Mobile as Mobile1,b.Mobile2,b.displayname as AccountName ,b.gender, b.address,
                            b.PhoneNumber as Phone1,b.phonenumber2 as Phone2 from Accounts as a,users as b where a.ownerId=b.userId
	                        and ((b.Mobile like '%{0}%')or( b.Mobile2 like '%{0}%')
                            or(b.phonenumber like '%{0}%') or (b.phonenumber2 like '%{0}%'))", phone);
            var result = _databaseInstance.Query<ClientAccount>(sql, null).FirstOrDefault();
            return result;
        }
    }

}
