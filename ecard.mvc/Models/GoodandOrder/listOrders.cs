using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Mvc.ViewModels;
using Microsoft.Practices.Unity;
using Ecard.Services;
using Ecard.Models;
using Ecard.Infrastructure;
using Moonlit;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class listOrders : EcardModelListRequest<ListOrder>
    {
        [Dependency, NoRender]
        public IOrderService OrderService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency,NoRender]
        public IMembershipService MembershipService {get;set;}
        private Bounded _sender;
        [NoRender]
        public Bounded Senders
        {
            get
            {
                if (_sender == null)
                {
                    _sender = Bounded.Create<User>("State", OrderState.All);
                }

                return _sender;
            }
            set { _sender = value; }
        }
        public listOrders()
        {
            OrderBy = "OrderId";
        }

        /*查询的条件*/
        private Bounded _state;
        public Bounded Status
        {
            get
            {
                if (_state == null)
                {
                    //_state = Bounded.Create<Order>("State", OrderState.All);
                    var ss = Bounded.Create<Order>("State", UserStates.Normal).Items.OrderBy(c => c.Key).ToList();
                    _state = new Bounded(OrderState.All,ss);
                }
                    
                return _state;
            }
            set { _state = value; }
        }
        public string AccountName { get; set; }
        public string CreateUser { get; set; }
        private DataRange _data;
        public DataRange Data
        {
            get
            {
                if (_data == null)
                    _data = new DataRange();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public void Ready()
        {
            var tempSenders = new List<IdNamePair>();
            tempSenders.Add(new IdNamePair() { Key = -10001, Name = "请选择" });
            //取得所有具有派送权限的员工。
            var carryRoles = MembershipService.QueryRoles(new RoleRequest() { State = RoleStates.Normal }).Where(x => (!string.IsNullOrWhiteSpace(x.Permissions) && x.Permissions.IndexOf("OrderCarry") > 0));
            if (carryRoles.Count() > 0)
            {
                var carryRoleIds= carryRoles.Select(x => x.RoleId).ToArray();
                var carryRole = MembershipService.GetUerIdsByRoleIds(carryRoleIds).ToArray();
                var senders = MembershipService.QueryUsers<User>(new UserRequest()).Where(x=>carryRole.Contains(x.UserId));

                foreach (var item in senders)
                {
                    tempSenders.Add(new IdNamePair() { Key = item.UserId, Name = item.DisplayName });
                }

                Senders.Bind(tempSenders);
            }
            else
            {

                Senders.Bind(tempSenders);
            }
        }

        public void Delete(int id)
        {
            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.QueryOrder(new OrderRequest() { Serialnumber = id }).FirstOrDefault();
            if (item != null)
            {
                var detials = OrderService.GetByorderId(item.OrderId);
                foreach (var detial in detials)
                {
                    OrderService.DeleteOrderDetial(detial);
                }
                OrderService.DeleteOrder(item);
                Logger.LogWithSerialNo(LogTypes.DeleteGood, serialNo, item.Serialnumber, item.OrderId);
                AddMessage("delete.success", serialNo, item.OrderId, item.OrderId);
            }
            TransactionHelper.Commit();
        }

        public void Complete(int id)
        {
            //完成订单。

            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.QueryOrder(new OrderRequest() { Serialnumber = id }).FirstOrDefault();
            if (item != null)
            {
                if (item.State == OrderState.Partially|| item.State == OrderState.Carry)
                {
                    item.State = OrderState.Completed;
                    OrderService.UpdateOrder(item);
                    Logger.LogWithSerialNo(LogTypes.EditOrder, serialNo, item.Serialnumber, item.OrderId);
                    AddMessage("Update.success", item.OrderId);
                }
                else { AddError("当前状态不能进入完成状态", item.OrderId); }
            }
            TransactionHelper.Commit();
        }
        public void Carry(int orderId,int senderId)
        {
             //派送。

            TransactionHelper.BeginTransaction();
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.QueryOrder(new OrderRequest() { Serialnumber = orderId }).FirstOrDefault();
            if (item != null )
            {
                if (item.State == OrderState.Normal)
                {
                    var user = MembershipService.GetUserById(senderId);
                    item.State = OrderState.Carry;
                    item.SenderId = senderId;
                    item.Sender = user.DisplayName;
                    item.SubmitTime = DateTime.Now;
                    OrderService.UpdateOrder(item);
                    Logger.LogWithSerialNo(LogTypes.EditOrder, serialNo, item.Serialnumber, item.OrderId);
                    AddMessage("Update.success", item.OrderId);
                }
                else { AddError("当前状态不能进入派送阶段", item.OrderId); }
            }
            TransactionHelper.Commit();
        }

        public void Query()
        {
            OrderRequest request = new OrderRequest();
            if (Data.Start.HasValue)
                request.Bdate = Data.Start.Value;
            if (Data.End.HasValue)
                request.Edate = Data.End.Value;
            if (!string.IsNullOrWhiteSpace(AccountName))
            {
                var account1 = AccountService.GetByName(AccountName);
                if (account1 != null)
                    request.AccountId = account1.AccountId;
            }
            var query = this.OrderService.QueryOrder(request).ToList();
            if (this.Status != States.All)
                query = query.Where(x => x.State == this.Status).ToList();
            // fill condition

            var accounts = AccountService.Query(new AccountRequest()).ToList().Where(x => query.Select(o => o.AccountId).Contains(x.AccountId));

            //query.Where(x => accounts.Select(u => u.AccountId).Contains(x.AccountId)).ToList(this, x => new ListOrder(x) {  });
            List = query.ToList(this, x => new ListOrder(x));
            foreach (var item in accounts)
            {
                foreach (var orderItem in List)
                {
                    if (orderItem.InnerObject.AccountId == item.AccountId)
                        orderItem.AccountName = item.Name;
                }
            }
        }

        //停用，注销订单,完成和部分完成的不能注销。
        public void Suspend(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.QueryOrder(new OrderRequest() { Serialnumber = id }).FirstOrDefault();
            if (item != null)
            {
                if (item.State != OrderState.Invalid)
                {
                    item.State = OrderState.Invalid;
                    OrderService.UpdateOrder(item);
                    Logger.LogWithSerialNo(LogTypes.EditOrder, serialNo, item.Serialnumber, item.OrderId);
                    AddMessage("suspend.success", item.OrderId);
                }
                else { AddError("已完成或部分完成则不能被注销！", item.OrderId); }
            }
        }
        //订单回复到正常状态，
        public void Resume(int id)
        {
            var serialNo = SerialNoHelper.Create();
            var item = this.OrderService.QueryOrder(new OrderRequest() { Serialnumber = id }).FirstOrDefault();
            if (item != null )
            {
                if (item.State == OrderState.Invalid)
                {
                    item.State = GoodState.Normal;
                    OrderService.UpdateOrder(item);
                    Logger.LogWithSerialNo(LogTypes.EditGood, serialNo, item.Serialnumber, item.OrderId);
                    AddMessage("resume.success", item.OrderId);
                }
                else { AddError("只有已经被注销的订单才能启用！", item.OrderId); }
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Completes", null);
            //yield return new ActionMethodDescriptor("Resumes", null);
            //yield return new ActionMethodDescriptor("Carries", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListOrder item)
        {
            if (item.InnerObject.State ==OrderState.Normal)
                yield return new ActionMethodDescriptor("Edit", null, new { id = item.Serialnumber });
            if (item.InnerObject.State != OrderState.Invalid)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.Serialnumber });
            if(item.InnerObject.State == OrderState.Normal)
                yield return new ActionMethodDescriptor("Carry", null, new { id=item.Serialnumber});
            if(item.InnerObject.State==OrderState.Carry)
                yield return new ActionMethodDescriptor("Complete", null, new { id = item.Serialnumber });
        }

    }
}
