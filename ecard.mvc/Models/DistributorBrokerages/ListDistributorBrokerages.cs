using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Ecard.Mvc.Models.Distributors;

namespace Ecard.Mvc.Models.DistributorBrokerages
{
    public class ListDistributorBrokerages : EcardModelListRequest<ListDistributorBrokerage>
    {
        public ListDistributorBrokerages()
        {
            OrderBy = "Rate";
        }
        /// <summary>
        /// 包含导出、确认结算请求
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("confirms", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListDistributorBrokerage item)
        {
            int id =0;
            var currentuser = this.SecurityHelper.GetCurrentUser();
            if(currentuser is DistributorUserModel)
                id = ((DistributorUserModel)currentuser).DistributorId;
            if (!item.InnerObject.status&&item.settlementDistributorId==id)
            {
                yield return new ActionMethodDescriptor("confirm", null, new { id = item.Id });
            }
        }
        [NoRender,Dependency]
        public IDealLogService DistributorBrokerageService { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IDistributorService DistributorService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        public string AccountName { get; set; }
        private Bounded _stateBounded;
        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.Create<DealLog>("State", States.All);
                }
                return _stateBounded;
            }
            set { _stateBounded = value; }
        }
        private Bounded _distributorBounded;
        public Bounded Distributor
        {
            get
            {
                if (_distributorBounded == null)
                {
                    _distributorBounded = Bounded.CreateEmpty("DistributorId", Globals.All);
                }
                return _distributorBounded;
            }
            set { _distributorBounded = value; }
        }
        public void Ready()
        {
            var q = DistributorService.Query();
            UserRequest user = new UserRequest();
            var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();
            var currentuser = this.SecurityHelper.GetCurrentUser();
            if (currentuser is AdminUserModel)
            {
                ////张飞牛肉，需要总部可以看到所有的经销商。
                ////如果是系统总部，列表中只包含一级经销商。
                //var distributors = DistributorService.Query().Where(x => x.ParentId == 0).ToList();
                //var list = (from x in (distributors.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
                //var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
                //Distributor.Bind(qq, true);
                var distributors = DistributorService.Query().ToList();
                var list = (from x in (distributors.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
                var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
                Distributor.Bind(qq, true);
                
            }
            else if (currentuser is DistributorUserModel)
            {
                int id = ((DistributorUserModel)currentuser).DistributorId;
                var totalIds = q.Select(x => x.DistributorId).ToList();
                var ids = GetChildrenDistributorId(id, totalIds);
                //ids.Add(id);
                var distributors = from c in DistributorService.Query().ToList() where ids.Contains(c.DistributorId) select c;
                var list = (from x in (distributors.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
                var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
                Distributor.Bind(qq, true);
            }
            else
            {
                List<IdNamePair> qq = new List<IdNamePair>();
                Distributor.Bind(qq, true);
            }

            List<IdNamePair> qs = new List<IdNamePair>();
            qs.Insert(0, new IdNamePair { Key = 0, Name = "未结算" });
            qs.Insert(0, new IdNamePair { Key = 1, Name = "已结算" });
            State.Bind(qs, true);

           
        }
        public void Query()
        {
            var request=new DistributorBrokerageRequest();
            //request.AccountId = this.AccountId;
            request.SubmitTimeMin = Data.Start;
            request.SubmitTimeMax = Data.End;
            request.NameWith = AccountName;
            if (Distributor != -10001)
                request.DistributorId = Distributor;
            if(State==1)
                request.Status = true;
            if (State == 0)
                request.Status = false;
            var q = DistributorBrokerageService.QueryBrokerage(request).ToList();
            //如果是系统总部，列表中只包含一级经销商。
            var distributors=DistributorService.Query().Where(x=>x.ParentId==0).ToList();
            var user = this.SecurityHelper.GetCurrentUser();
            if (user is AdminUserModel)
                List = List = q.ToList(this, x => new ListDistributorBrokerage(x));//(from c in q where distributors.Any(p=>p.DistributorId==c.DistributorId) select c).ToList(this,x=>new ListDistributorBrokerage(x));
            //显示直属当前经销商和归属当前经销商的下级
            else if (user is DistributorUserModel)
            {
                int id = ((DistributorUserModel)user).DistributorId;
                var totalIds = q.Select(x => x.DistributorId).ToList();
                var ids = GetChildrenDistributorId(id, totalIds);
                ids.Add(id);
                List = (from c in q where ids.Contains(c.settlementDistributorId) select c).ToList(this, x => new ListDistributorBrokerage(x));

            }
            foreach (var item in List)
            {
                var distributor1 = DistributorService.GetById(item.DistributorId);
                if (distributor1 != null)
                {
                    var distributoruser = MembershipService.GetUserById(distributor1.UserId);
                    if(user!=null)
                        item.DistributorName = distributoruser.DisplayName;
                }
                var distributor2 = DistributorService.GetById(item.settlementDistributorId);
                if (distributor2 != null)
                {
                    var distributoruser = MembershipService.GetUserById(distributor2.UserId);
                    if (user != null)
                        item.SettlementDistributorName = distributoruser.DisplayName;
                }
                var account = AccountService.GetById(item.AccountId);
                if (account != null)
                {
                    item.AccountName = account.Name;
                }
            }
        }
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

        /// <summary>
        /// 取得所有的下属经销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> GetChildrenDistributorId(int id,List<int> list)
        {
            List<int> ids = new List<int>();
            foreach (var item in list)
            {
                int TopLevelDistributorId=GetTopLevelDistributorId(item);
                if (id == TopLevelDistributorId)
                    ids.Add(item);
            }
            return ids;
        }

        /// <summary>
        /// 根据输入的经销商Id取得一级经销商的Id
        /// </summary>
        /// <param name="distributorId"></param>
        /// <returns></returns>
        public int GetTopLevelDistributorId(int distributorId)
        {
            var parent=DistributorService.GetById(distributorId);
            if (parent != null)
                if (parent.ParentId == 0)
                    return parent.DistributorId;
                else return GetTopLevelDistributorId(parent.ParentId);
            else
                return 0;

        }
        /// <summary>
        /// 确认结算，如果这条消费记录的卡号不属于自己的，则给下级经销商生成结算请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Close(int id)
        {

            TransactionHelper.BeginTransaction();
            var item = DistributorBrokerageService.GetDistributorBrokerageById(id);
            if (item == null)
                return;
            var user = this.SecurityHelper.GetCurrentUser();
            int currentId=-1;
            if (user is DistributorUserModel)
            {
                currentId=DistributorService.GetByUserId(user.CurrentUser.Id).DistributorId;

            }
            if(item.settlementDistributorId==currentId&&item.status==false)
            {
                item.status = true;
                DistributorBrokerageService.UpdateDistributorBrokerage(item);
                //增加经销商的余额
                var distributor1 = DistributorService.GetById(item.settlementDistributorId);
                distributor1.Amount += item.brokerage;
                DistributorService.Update(distributor1);
                if (item.DistributorId != item.settlementDistributorId)
                {
                    //归属他下属的卡消费，他要跟下属结算。
                    //如果是多级还要找到他的直接下属，只能向他的直接下属发起结算请求。
                    int underId = GetDirectlyUnder(item.settlementDistributorId, item.DistributorId);
                    if (underId != 0)
                    {
                        var distributor = DistributorService.GetAccountLevelPolicyRates(underId).FirstOrDefault();
                        DistributorBrokerage model = new DistributorBrokerage();
                        model.DistributorId = item.DistributorId;
                        model.AccountId = item.AccountId;
                        model.Bdate = item.Bdate;
                        model.consume = item.consume;
                        model.Edate = item.Edate;
                        model.settlementDistributorId = underId;
                        model.Rate = distributor.Rate;
                        model.brokerage = item.consume * distributor.Rate;
                        model.status = false;
                        DistributorBrokerageService.CreateDistributorBrokerage(model);

                    }
                    AddMessage("update.success", item.Id);
                }
               
            }
            else { AddError("不是结算经销商", item.Id); }
            TransactionHelper.Commit();
        }
        /// <summary>
        /// 取得直接下属的Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int GetDirectlyUnder(int parentId, int Id)
        {
            var item = DistributorService.GetById(Id);
            if (item != null)
                if (item.ParentId == parentId)
                    return Id;
                else return GetDirectlyUnder(parentId, item.ParentId);
            else
                return 0;
        }
    }
}
