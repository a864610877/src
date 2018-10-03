using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Infrastructure;
using Moonlit;
using Ecard.Mvc.Models.Commodities;
using System.ComponentModel.DataAnnotations;
using Ecard.Models.GoodandOrder;
using Ecard.Mvc.ActionFilters;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class CreateOrder:OrderModelBase
    {
        public CreateOrder()
        {
            InnerObject.State = OrderState.Normal;
        }
        [UIHint("ListCommodity")]
        public MultiCheckList<ListCommodity> Detial { get; set; }
        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            string str = "S" + DateTime.Now.ToString("yyyyMMdd");
            InnerObject.OrderId = str+"-" + OrderService.GetOrderSerialnumber();
            InnerObject.AccountId = AccountId;
            InnerObject.Phone = Phone;
            var user = SecurityHelper.GetCurrentUser();
            InnerObject.Creater = user.CurrentUser.DisplayName;
            InnerObject.CreaterId = user.CurrentUser.UserId;
            InnerObject.createDate = DateTime.Now;
            base.OnSave(InnerObject);
            OrderService.CreateOrder(InnerObject);
            foreach (var item in this.Detials)
            {
                item.OrderId = InnerObject.OrderId;
                OrderService.AddOrderDetial(item);
            }
            AddMessage("success", InnerObject.OrderId);
            Logger.LogWithSerialNo(LogTypes.AddOrder, serialNo, 0);
            return TransactionHelper.CommitAndReturn(this);
        }
        public void Ready()
        {
            var qq = CommodityService.Query(new Services.CommodityRequest() { State = CommodityStates.Normal }).Select(x => new ListCommodity(x)).ToList();
            this.Detial.Merge(qq);
        }
        public void Read()
        {
            //var accounts = AccountService.QueryAccountWithOwner(new Services.AccountRequest());
            //List<IdNamePair> accountBounded = new List<IdNamePair>();
            //int[] ids=new int[accounts.Count()];
            //int i=0;
            //foreach (var item in accounts)
            //{
            //    ids[i] = item.AccountId; i += 1;
            //    accountBounded.Add(new IdNamePair() {Key=item.AccountId,Name=item.OwnerDisplayName });
            //}
            //var accounts2 = AccountService.Query(new Services.AccountRequest()).Where(x => !ids.Contains(x.AccountId));
            //foreach (var item in accounts2)
            //{
            //    accountBounded.Add(new IdNamePair() { Key=item.AccountId,Name=item.Name });
            //}
            //AccountId.Bind(accountBounded);
            this.Detial = new MultiCheckList<ListCommodity>(CommodityService.Query(new Services.CommodityRequest() { State=CommodityStates.Normal }).Select(x => new ListCommodity(x)).ToList());
        }

        internal void AddMsg(string p)
        {
            AddMessage(p);
        }
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入客户的联系电话")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
                            , ErrorMessage = "输入的必须是手机号码和电话号码")]
        public string Phone
        {
            get;
            set;
        }
        //[Remote("CheckAccountName","Order",ErrorMessage="会员卡没有输入完整")]
        [UIHint("SearchAccount")]
        public string AccountName { get; set; }
        [NoRender]
        public int AccountId { get; set; }

    }
    public class CreateClientOrder : OrderModelBase
    {
        public CreateClientOrder()
        {
            InnerObject.State = OrderState.Normal;
        }
        [UIHint("ListCommodity")]
        public MultiCheckList<ListCommodity> Detial { get; set; }
        public IMessageProvider Create()
        {
            var user = SecurityHelper.GetCurrentUser();
            InnerObject.Creater = user.CurrentUser.DisplayName;
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            string str = "C" + DateTime.Now.ToString("yyyyMMdd");
            InnerObject.OrderId = str+"-" + OrderService.GetOrderSerialnumber();
            InnerObject.AccountId = AccountId;
            InnerObject.createDate = DateTime.Now;
            InnerObject.CreaterId = user.CurrentUser.UserId;
            InnerObject.Phone = Phone;
            base.OnSave(InnerObject);
            OrderService.CreateOrder(InnerObject);
            foreach (var item in this.Detials)
            {
                item.OrderId = InnerObject.OrderId;
                OrderService.AddOrderDetial(item);
            }
            AddMessage("success", InnerObject.OrderId);
            Logger.LogWithSerialNo(LogTypes.AddOrder, serialNo, 0);
            return TransactionHelper.CommitAndReturn(this);
        }
        public void Ready()
        {
            var qq = CommodityService.Query(new Services.CommodityRequest() { State = CommodityStates.Normal }).Select(x => new ListCommodity(x)).ToList();
            if (Detial == null)
            {
                this.Detial = new MultiCheckList<ListCommodity>(CommodityService.Query(new Services.CommodityRequest() { State = CommodityStates.Normal }).Select(x => new ListCommodity(x)).ToList());
            }
            this.Detial.Merge(qq);
        }
        public void Read(string phone)
        {
            AccountMsg = OrderService.GetAccountByPhone(phone);
            if (AccountMsg == null)
            {
                this.AddError(LogTypes.AddOrder, "找不到电话号码对应的会员", AccountMsg);
                return;
            }
            Address = AccountMsg.Address;
            gender = AccountMsg.Gender == 1 ? "先生" : AccountMsg.Gender == 2 ? "女士" : "保密";
            AccountId = AccountMsg.AccountId;
            _accountName = AccountMsg.AccountName;
            Phone = phone;
            this.Detial = new MultiCheckList<ListCommodity>(CommodityService.Query(new Services.CommodityRequest() { State=CommodityStates.Normal }).Select(x => new ListCommodity(x)).ToList());
        }

        internal void AddMsg(string p)
        {
            AddMessage(p);
        }
        [NoRender]
        public ClientAccount AccountMsg { get; set; }

        [Hidden]
        public int AccountId { get; set; }
        private string _accountName;
        public string AccountName { get { return _accountName; } }
        private string gender;
        public string Gender
        {
            get { return gender; }
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入客户的联系电话")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
                            , ErrorMessage = "输入的必须是手机号码和电话号码")]
        public string Phone
        {
            get;
            set;
        }
    }
}
