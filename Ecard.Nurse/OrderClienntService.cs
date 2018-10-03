using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Nurse
{
    public interface IOrderClientService
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        /// <returns></returns>
        List<ClientCommdity> GetCommodities();
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool AddOrder(ClientOrder order);
        /// <summary>
        /// 通过电话、手机号码取得会员资料。
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        ClientAccount GetAccountByPhone(string phone);

        
    }
    /// <summary>
    /// 商品
    /// </summary>
    public class ClientCommdity
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string CommodityName { get; set; }

    }
    /// <summary>
    /// 订单
    /// </summary>
    public class ClientOrder
    {
        /// <summary>
        /// 会员卡
        /// </summary>
        public int AccountId { set; get; }
        /// <summary>
        /// 派送地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 下单人
        /// </summary>
        public string Creater { set; get; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<ClientOrderDetial> Detials { get; set; }
    }
    /// <summary>
    /// 订单明细
    /// </summary>
    public class ClientOrderDetial
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { get; set; }
    }
    /// <summary>
    /// 会员资料
    /// </summary>
    public class ClientAccount
    {
        /// <summary>
        /// 会员id
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// 性别:1、男 ，2、女
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机1
        /// </summary>
        public string Mobile1 { get; set; }
        /// <summary>
        /// 手机2 
        /// </summary>
        public string Mobile2 { get; set; }
        /// <summary>
        /// 电话1
        /// </summary>
        public string Phone1 { get; set; }
        /// <summary>
        /// 电话2
        /// </summary>
        public string Phone2 { get; set; }

    }

    public class SqlOrderClientService : IOrderClientService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string OrderTable = "Orders";
        private const string OrderDetialTable = "OrderDetial";
        private const string CommodityTable = "Commodities";

        private TransactionHelper transactionHelper;

        public SqlOrderClientService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
            transactionHelper = new TransactionHelper(databaseInstance);
        }

        public List<ClientCommdity> GetCommodities()
        {
            var commodities = new QueryObject<Commodity>(_databaseInstance, "Commodity.query", null).Where(x=>x.State==CommodityStates.Normal);
            List<ClientCommdity> listCommodities = new List<ClientCommdity>();
            foreach (var item in commodities)
            {
                ClientCommdity model = new ClientCommdity();
                model.CommodityId = item.CommodityId;
                model.Price = item.Price;
                model.CommodityName = item.DisplayName;
                listCommodities.Add(model);
            }
            return listCommodities;
        }

        public bool AddOrder(ClientOrder order)
        {
            if (order.Detials.Any(x => x.Amount < 1) || order.Detials.Any(y => y.Price <= 0))
                return false;
            try
            {
                OrderBase createOrder = new OrderBase();
                createOrder.AccountId = order.AccountId;
                createOrder.Address = order.Address;
                createOrder.Creater = order.Creater;
                createOrder.OrderId = GetOrderSerialnumber();
                createOrder.State = OrderState.Normal;
                createOrder.SubmitTime = DateTime.Now;
                createOrder.TotalMoney = order.Detials.Sum(x => x.Amount * x.Price);
                transactionHelper.BeginTransaction();
                foreach (var item in order.Detials)
                {
                    OrderDetialBase detial = new OrderDetialBase();
                    detial.Amount = item.Amount;
                    detial.GoodId = item.CommodityId;
                    detial.OrderId = createOrder.OrderId;
                    detial.price = item.Price;
                    _databaseInstance.Insert(detial, OrderTable);
                }
                _databaseInstance.Insert(createOrder, OrderDetialTable);
                return transactionHelper.CommitAndReturn(true);
            }
            catch
            { return false; }
        }

        public ClientAccount GetAccountByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new ClientAccount();
            string sql = @"select a.accountId,b.birthDate,b.Mobile as Mobile1,b.Mobile2,b.displayname as AccountName ,b.gender, b.address,
                            b.PhoneNumber as Phone1,b.phonenumber2 as Phone2 from Accounts as a,users as b where a.ownerId=b.userId
	                        and b.discriminator='AccountUser' and ((b.Mobile like '%@phone%')or( b.Mobile2 like '%@phone%')
                            or(b.phonenumber like '%@phone%') or (b.phonenumber2 like '%@phone%'))";
            var result = _databaseInstance.Query<ClientAccount>(sql, new { phone = phone }).FirstOrDefault();
            return result;
        }

        public string GetOrderSerialnumber()
        {
            string sql = @"select isnull(max(substring(Orderid,11,5))+1,1) from Orders 
                        where substring(Orderid,2,8)is null or substring(Orderid,2,8)=Convert(varchar(10),Getdate(),112)";
            int num = (int)_databaseInstance.ExecuteScalar(sql, null);
            string snoTemp = num.ToString();
            for (int i = 0; i < 5 - num.ToString().Length; i++)
            {
                snoTemp = "0" + snoTemp;
            }
            return snoTemp;
        }
    }

    public class OrderBase
    {
        #region Model
        /// <summary>
        /// 订单号规则：SyyyyMMdd-00001
        /// </summary>
        //[Key]
        public string OrderId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int AccountId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Creater { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime SubmitTime { set; get; }
        public int State { set; get; }

        #endregion Model
    }
    public class OrderDetialBase
    {

        #region Model
        /// <summary>
        /// 
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GoodId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }
        #endregion Model
    }
}
