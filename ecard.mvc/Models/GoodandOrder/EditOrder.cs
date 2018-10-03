using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Infrastructure;
using Moonlit;
using Ecard.Mvc.Models.Commodities;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class TempDatial : IKeyObject
    {
        public TempDatial()
        { }
        public TempDatial(Commodity item)
        {
            CommodityId = item.CommodityId;
            Name = item.Name;
            DisplayName = item.DisplayName;
            Price = item.Price;
            Amount = 0;
        }
        public int CommodityId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        //public bool Isselect { get; set; }

        public int Id
        {
            get { return CommodityId; }
        }
    }
    public class EditOrder : OrderModelBase
    {
        
        public EditOrder()
        {
            
        }
        public void LoadInnerObject(int id)
        {
            var a = OrderService.QueryOrder(new Services.OrderRequest() { Serialnumber = id }).FirstOrDefault();
            SetInnerObject(a);
        }
        [NoRender]
        public MultiCheckList<ListCommodity> Detial { get; set; }
        [UIHint("ListOrderDetial")]
        public MultiCheckList<TempDatial> AllCommodity { get; set; }
        [Hidden]
        public string OrderId { get { return InnerObject.OrderId; } set { InnerObject.OrderId = value; } }
        [Hidden]
        public int Serialnumber { get { return InnerObject.Serialnumber; } set { InnerObject.Serialnumber = value; } }
        private string accountName;
        public string AccountName { get { return accountName; } }
        [Hidden]
        public int AccountID { get { return InnerObject.AccountId; } set { InnerObject.AccountId = value; } }
        [Hidden]
        public int MyState { get { return InnerObject.State; } set { InnerObject.State = value; } }
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
            , ErrorMessage = "输入的必须是手机号码和电话号码")]
        public string AccountPhone { get { return InnerObject.Phone; } set { InnerObject.Phone = value; } }
        public string Order_ID { get { return "订单号:"+InnerObject.OrderId+"，\t   金额："+InnerObject.TotalMoney.ToString("C"); } }
        public IMessageProvider Edit()
        {
            var newObject = OrderService.QueryOrder(new Services.OrderRequest() { OrderId = InnerObject.OrderId }).FirstOrDefault();
            if (newObject != null)
            {
                var serialNo = SerialNoHelper.Create();
                TransactionHelper.BeginTransaction();
                //InnerObject.AccountId = AccountID;
                //InnerObject.State = MyState;
                //newObject.Address = Address;
                newObject.Phone = AccountPhone;
                //newObject.SubmitTime = DateTime.Now;
                base.OnSave(newObject);
                OrderService.UpdateOrder(newObject);
                OrderService.DeleteOrderDetials(InnerObject.OrderId);
                foreach (var item in this.Detials)
                {
                    item.OrderId = OrderId;
                    OrderService.AddOrderDetial(item);
                }
                AddMessage("Edit.success", InnerObject.OrderId);
                Logger.LogWithSerialNo(LogTypes.EditOrder, serialNo, InnerObject.Serialnumber, InnerObject.OrderId);
                return TransactionHelper.CommitAndReturn(this);
            }
            else
            {
                AddError(LogTypes.EditOrder, "无此订单！", null);
                return this;
            }
        }
        public void Ready()
        {
            var account = this.AccountService.GetById(InnerObject.AccountId);
            var owner=AccountService.QueryAccountWithOwner(new Services.AccountRequest() { Ids = new int[] { account.AccountId } }).FirstOrDefault();
            string myName = "无会员信息";
            if(owner!=null)
                myName=owner.OwnerDisplayName;
            accountName ="会员卡号："+account.Name +"，会员名字："+myName;
        }
        public void Read()
        {

            var all = CommodityService.Query(new Services.CommodityRequest() { State = CommodityStates.Normal }).Select(x => new TempDatial(x)).ToList();
            this.Detials = OrderService.GetByorderId(InnerObject.OrderId).ToList();
            foreach (var item in Detials)
            {
                var model=all.FirstOrDefault(x=>x.CommodityId==item.GoodId);
                if(model!=null)
                {
                    model.Amount = item.Amount;
                    model.Price = item.price;
                }
            }
            this.AllCommodity = new MultiCheckList<TempDatial>(Detials.Select(x => new TempDatial() 
                              { Amount = x.Amount, CommodityId = x.GoodId, Price = x.price }));
            AllCommodity.Merge(all);

        }

        internal void AddMsg(string p)
        {
            AddMessage(p);
        }
    }
}
