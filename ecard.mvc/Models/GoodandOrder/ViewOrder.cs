using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Microsoft.Practices.Unity;
using Ecard.Services;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class ViewOrder : ViewModelBase
    {
        private  Order _innerObject;
        [NoRender]
        public Order InnerObject { get { return this._innerObject; } }
        public ViewOrder()
        {
            _innerObject = new Order();
        }
        [NoRender, Dependency]
        public IOrderService OrderService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        public ViewOrder(Order order)
        {
            _innerObject = order;
        }
        private List<OrderDetialBase> detials;
        [UIHint("OrderDetialView")]
        public List<OrderDetialBase> Detials { get { return detials; } }
        [Dependency, NoRender]
        public ICommodityService CommodityService { get; set; }
        public string OrderId { get; set; }
        public string TotalMoney { get; set; }
        public string CreateDate { get; set; }
        public string Creater { get; set; }
        public string AccountName { get; set; }
        public string AccountDiaplayName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Sender { get; set; }

        public void Read()
        {
            var account = this.AccountService.GetById(InnerObject.AccountId);
            string myName = "无会员信息";
            var my = AccountService.QueryAccountWithOwner(new Services.AccountRequest() { Ids = new int[] { InnerObject.AccountId } }).FirstOrDefault();
            if (my != null)
                myName = my.OwnerDisplayName;
            OrderId = InnerObject.OrderId;
            TotalMoney = InnerObject.TotalMoney.ToString("C");
            CreateDate = InnerObject.createDate.ToString("yyyy-MM-dd HH:mm:ss");
            Creater = InnerObject.Creater;
            AccountName = account.Name;
            AccountDiaplayName = myName;
            Phone = InnerObject.Phone;
            Address = InnerObject.Address;
            State = ModelHelper.GetBoundText(InnerObject, x => x.State);
            Sender = (string.IsNullOrWhiteSpace(InnerObject.Sender)?"无派送信息":InnerObject.Sender);
            detials = OrderService.GetByorderId(InnerObject.OrderId).Select(x => new OrderDetialBase(x)).ToList();
            foreach (var item in detials)
            {
                var good = CommodityService.GetById(item.GoodId);
                item.GoodName = good.Name;
                item.DisplayName = good.DisplayName;
            }
        }

        public void SetInnerObject(Order item)
        {
            _innerObject = item;
        }
    }
}
