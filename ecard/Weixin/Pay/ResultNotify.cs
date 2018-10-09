using Ecard;
using Ecard.Services;
using Moonlit.Data;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Ecard.Models;
using System.Threading;
using Ecard.SqlServices;
using Microsoft.Practices.Unity;

namespace WxPayAPI
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify : Notify
    {
        private Database _database;
        private DatabaseInstance _databaseInstance;
        //var database = new Database("ecard");
        //DatabaseInstance _databaseInstance = new DatabaseInstance(database);

        private readonly IAccountService IAccountService;
        private readonly ISiteService ISiteService;
        private readonly ILog4netService ILog4netService;
        private readonly IOrdersService ordersService;
        private readonly IOrderDetialService orderDetialService;
        private readonly ITicketsService ticketsService;
        private readonly IAdmissionTicketService admissionTicketService;

        public ResultNotify(Page page)
            : base(page)
        {
            _database = new Database("ecard");
            ILog4netService = new SqlLog4netService();
            ISiteService = new SqlSiteService(_database.OpenInstance());
            IAccountService = new SqlAccountService(_database.OpenInstance());
            ordersService = new SqlOrdersService(_database.OpenInstance());
            ticketsService = new SqlTicketsService(_database.OpenInstance());
            admissionTicketService = new SqlAdmissionTicketService(_database.OpenInstance());
            orderDetialService = new SqlOrderDetialService(_database.OpenInstance());
        }

        public override void ProcessNotify()
        {

            WxPayData notifyData = GetNotifyData();

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            string transaction_id = notifyData.GetValue("transaction_id").ToString();
            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "订单查询失败 : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                try
                {
                    Log.Debug(this.GetType().ToString(), "订单状态 : " + notifyData.GetValue("result_code").ToString());
                    if (notifyData.GetValue("result_code").ToString() == "SUCCESS")
                    {
                        string orderNo = notifyData.GetValue("out_trade_no").ToString();
                        Log.Debug(this.GetType().ToString(), "订单号 : " + orderNo);
                        if (!string.IsNullOrWhiteSpace(orderNo))
                        {
                            using (_databaseInstance = new DatabaseInstance(_database))
                            {
                                _databaseInstance.BeginTransaction();
                                var sql = "select * from fz_Orders where orderNo=@orderNo";
                                var order = new QueryObject<Orders>(_databaseInstance, sql, new { orderNo = orderNo }).FirstOrDefault();
                                if (order != null && order.orderState == OrderStates.awaitPay)
                                {
                                    order.orderState = OrderStates.paid;
                                    order.payTime = DateTime.Now;
                                    _databaseInstance.Update(order, "fz_Orders");
                                    var list = orderDetialService.GetOrderNo(orderNo);
                                    if (list != null)
                                    {
                                        if (order.type == OrderTypes.ticket)
                                        {
                                            foreach (var item in list)
                                            {
                                                var admissionTicket = admissionTicketService.GetById(item.sourceId);
                                                for (var i = 0; i < item.num; i++)
                                                {
                                                    var ticket = new Tickets();
                                                    ticket.AdmissionTicketId = admissionTicket.id;
                                                    ticket.orderNo = orderNo;
                                                    ticket.Price = item.amount;
                                                    ticket.State = TicketsState.NotUse;
                                                    ticket.UserId = order.userId;
                                                    ticket.useScope = order.useScope;
                                                    ticket.adultNum = admissionTicket.adultNum;
                                                    ticket.BuyTime = DateTime.Now;
                                                    ticket.childNum = admissionTicket.childNum;
                                                    ticket.Code = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + i.ToString() + order.userId.ToString();
                                                    ticket.ExpiredDate = DateTime.Now.Date;
                                                    _databaseInstance.Insert(ticket, "Tickets");
                                                }
                                            }
                                        }
                                        else if (order.type == OrderTypes.card)
                                        {
                                            string sqlSite = "select * from Sites";
                                            var site = new QueryObject<Site>(_databaseInstance, sqlSite, null).FirstOrDefault();
                                            string cardNo = "";
                                            int minxCode =1;
                                            Int32.TryParse(site.MixCode, out minxCode);
                                            int i = 1;
                                            while (true)
                                            {
                                                minxCode = minxCode + i;
                                                cardNo = string.Format("60000000{0}", minxCode.ToString().PadLeft(8, '0'));
                                                string sqlCard = "select * from Accounts where Name=@Name";
                                                var card = new QueryObject<Account>(_databaseInstance, sqlCard, new { Name = cardNo }).FirstOrDefault();
                                                if (card == null)
                                                    break;
                                                i++;
                                            }
                                            int shopId = 0;
                                            if (!string.IsNullOrWhiteSpace(order.useScope))
                                            {
                                                string sqlShop = "select * from shops where Name=@Name";
                                                var shop = new QueryObject<Account>(_databaseInstance, sqlShop, new { Name = order.useScope }).FirstOrDefault();
                                                if (shop != null)
                                                    shopId = shop.ShopId;
                                            }
                                            var item = list.FirstOrDefault();
                                            var accountType = _databaseInstance.GetById<AccountType>("AccountTypes", item.sourceId);
                                            var account = new Account();
                                            account.AccountLevel = 0;
                                            account.AccountToken = "11111111";
                                            account.AccountTypeId = accountType.AccountTypeId;
                                            account.Amount = 0;
                                            account.ExpiredDate = DateTime.Now.AddMonths(accountType.ExpiredMonths);
                                            account.Frequency = accountType.Frequency;
                                            account.FrequencyUsed = 0;
                                            account.LastDealTime = DateTime.Now;
                                            account.Name = cardNo;
                                            account.OpenTime = DateTime.Now;
                                            account.OwnerId = order.userId;
                                            account.ShopId = shopId;
                                            account.useScope = order.useScope;
                                            account.State = AccountStates.Normal;
                                            _databaseInstance.Insert(account, "Accounts");
                                            site.MixCode = minxCode.ToString();
                                            _databaseInstance.Update(site, "Sites");
                                        }
                                    }
                                }
                                _databaseInstance.Commit();
                            }
                        }
                    }
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                    page.Response.Write(res.ToXml());
                    page.Response.End();
                }

                catch (Exception ex)
                {

                    if (!(ex is System.Threading.ThreadAbortException))
                    {
                        res.SetValue("return_code", "FAIL");
                        res.SetValue("return_msg", "订单状态修改失败");
                        Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml() + ex);
                        page.Response.Write(res.ToXml());
                        page.Response.End();
                    }

                }
                finally
                {
                    if (_databaseInstance != null)
                        _databaseInstance.Dispose();
                }



            }

        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}