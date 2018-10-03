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

namespace Ecard.Mvc.Models.DealLogs
{
    public class ListDealLogs : EcardModelListRequest<ListDealLog>
    {
        public ListDealLogs()
        {
            OrderBy = "DealLogId desc";
        }
        [UIHint("AccountName")]
        public string AccountName { get; set; }
        //private Bounded _dealLogQueryType;

        //public Bounded DealLogQueryType
        //{
        //    get
        //    {
        //        if (_dealLogQueryType == null)
        //        {
        //            _dealLogQueryType = Bounded.Create<DealLogRequest>("DealLogQueryType", DealLogQueryTypes.All);
        //        }
        //        return _dealLogQueryType;
        //    }
        //    set { _dealLogQueryType = value; }
        //}

        private Bounded _posBounded;
        [CheckUserType(typeof(AdminUser))]
        public Bounded Pos
        {
            get
            {
                if (_posBounded == null)
                {
                    _posBounded = Bounded.CreateEmpty("PosId", Globals.All);
                }
                return _posBounded;
            }
            set { _posBounded = value; }
        }
        [NoRender, Dependency]
        public IDealLogService DealLogService { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        [NoRender, Dependency]
        public IShopService ShopService { get; set; }
        [NoRender, Dependency]
        public IAccountDealService AccountDealService { get; set; }
        [NoRender, Dependency]
        public LogHelper LogHelper { get; set; }
        [NoRender, Dependency]
        public IPosEndPointService PosEndPointService { get; set; }
        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("List", null, new { export = "excel" });
            yield return new ActionMethodDescriptor("Rollback", null);
        }

        public string ShopName { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListDealLog item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.DealLogId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<DealLog>("State", DealLogStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }

        private Bounded _posIdBounded;

        public Bounded PosId
        {
            get
            {
                if (_posIdBounded == null)
                {
                    _posIdBounded = Bounded.CreateEmpty("PosIdId", Globals.All);
                }
                return _posIdBounded;
            }
            set { _posIdBounded = value; }
        }

        private Bounded _accountShopBounded;

        public Bounded AccountShop
        {
            get
            {
                if (_accountShopBounded == null)
                {
                    _accountShopBounded = Bounded.CreateEmpty("AccountShopId", Globals.All);
                }
                return _accountShopBounded;
            }
            set { _accountShopBounded = value; }
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

        private Bounded _dealType;
        public Bounded DealType
        {
            get 
            {
                if (_dealType == null)
                    _dealType = Bounded.CreateEmpty("DealType", 0);
                return _dealType;
            }
            set
            {
                _dealType = value;
            }
        }

        public void Query()
        {
            var request = new DealLogRequest();

            if (State != DealLogStates.All)
                request.State = State;
            request.AccountName = this.AccountName;
            //if (DealLogQueryType != DealLogQueryTypes.All)
            //{
            //    request.DealLogQueryType = DealLogQueryType;
            //}
            request.SubmitTimeMin = Data.Start;
            request.SubmitTimeMax = Data.GetEnd();
            request.ShopName = this.ShopName;
            if (this.AccountShop != Globals.All)
            {
                //var user = (AdminUser)MembershipService.GetUserById(this.Pos);
                //var pos = PosEndPointService.GetByName(user.PosName);
                request.ShopId = AccountShop.Key;
            }
            if (this.PosId != Globals.All)
            {
                request.PosId = this.PosId;
            }
            if (DealType.Key != 0)
                request.DealType = DealType.Key;
            var currentUser = SecurityHelper.GetCurrentUser();
            var shopUser = currentUser as ShopUserModel;
            if (shopUser != null)
                request.ShopId = shopUser.ShopId;

            var accountUser = currentUser as AccountUserModel;
            if (accountUser != null)
            {
                var accountId = accountUser.Accounts.Select(x => x.AccountId).First();
                request.AccountId = accountId;
            }
            if (AccountShop != Globals.All)
                request.AccountShopId = AccountShop;
            var query = this.DealLogService.Query(request);
            List = query.ToList(this, x => new ListDealLog(x));
            var NewItem = new ListDealLog();
            NewItem.SerialNo = "汇 总";
            foreach (var item in List)
            {
                NewItem.Amount += item.Amount;
                NewItem.Point += item.Point;
            }
            List.Add(NewItem);
            //var shopIds = List.Select(x => x.InnerObject.ShopId).Distinct();

            //var shops = ShopService.Query(new ShopRequest() { ShopIds = shopIds.ToArray() }).ToList();
            //foreach (var log in List)
            //{
            //    var shop = shops.FirstOrDefault(x => x.ShopId == log.InnerObject.ShopId);
            //    if (shop != null)
            //        log.TempShop = shop;
            //}

            // ready
            // 
            var queryPos = PosEndPointService.Query(new PosEndPointRequest() { State = PosEndPointStates.Normal }).ToList();
            this.PosId.Bind(queryPos.Select(x => new IdNamePair { Key = x.PosEndPointId, Name = x.Name }), true);
            var users = MembershipService.QueryUsers<AdminUser>(new UserRequest() { State = UserStates.Normal }).ToList();
            var poss = PosEndPointService.Query(new PosEndPointRequest() { State = PosEndPointStates.Normal }).ToList();
            var adminusers = from x in users
                             let p = poss.FirstOrDefault(y => y.CurrentUserId == x.UserId)
                             select new IdNamePair { Key = x.UserId, Name = string.Format("{0} - {1}", x.DisplayName, p == null ? "" : p.Name) };
            this.Pos.Bind(adminusers, true);
            // 
            var q = (from x in ShopService.Query(new ShopRequest() { IsBuildIn = false })
                     select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            this.AccountShop.Bind(q, true);

            IdNamePair ii = new IdNamePair();
            ii.Key = 0;
            ii.Name = "全部";
            DealType.Items.Add(ii);
            IdNamePair ip = new IdNamePair();
            ip.Key = DealTypes.Deal;
            ip.Name = "会员卡支付";
            DealType.Items.Add(ip);
            IdNamePair ip2 = new IdNamePair();
            ip2.Key = DealTypes.Integral;
            ip2.Name = "现金支付";
            DealType.Items.Add(ip2);
            IdNamePair ip1 = new IdNamePair();
            ip1.Key =DealTypes.CancelDeal;
            ip1.Name = "撤销交易";
            DealType.Items.Add(ip1);
            IdNamePair ip3 = new IdNamePair();
            ip3.Key = DealTypes.PayIntegral;
            ip3.Name = "积分消费";
            DealType.Items.Add(ip3);
            IdNamePair ip4 = new IdNamePair();
            ip4.Key = DealTypes.Recharging;
            ip4.Name = "充值";
            DealType.Items.Add(ip4);
            IdNamePair ip5 = new IdNamePair();
            ip5.Key = DealTypes.CancelRecharging;
            ip5.Name = "取消充值";
            DealType.Items.Add(ip5);
            IdNamePair ip6 = new IdNamePair();
            ip6.Key = DealTypes.Invalid;
            ip6.Name = "冲正";
            DealType.Items.Add(ip6);
        }

        public AccountServiceResponse  Rollback(int id)
        {
            try
            {
            // RollbackType   
                using (var tran = TransactionHelper.BeginTransaction())
                {
                    var serialNo = SerialNoHelper.Create();
                    var dealLog = this.DealLogService.GetById(id);
                    if (dealLog == null)
                    {
                        throw new Exception(Localize("nofoundDeal"));
                    }
                    var account = AccountService.GetById(dealLog.AccountId);
                    if (account == null)
                    {
                        throw new Exception(Localize("noufoundAccount"));
                    }
                    AccountServiceResponse rsp = null;
                    RollbackType r = RollbackType.None;
                    //if (dealLog.SubmitTime.Date != DateTime.Now.Date)
                    //{
                    //    rsp = AccountDealService.CancelPay(new CancelPayRequest(dealLog.AccountName, "", dealLog.SourcePosName, Math.Abs(dealLog.Amount), serialNo, dealLog.SerialNo, account.AccountToken,
                    //                                                   dealLog.SourceShopName) { IsForce = true },true);
                       
                    //    r = RollbackType.Cancel;
                    //}
                    //else
                    //{
                        rsp = AccountDealService.Roolback(new PayRequest_(dealLog.AccountName, "", dealLog.SourcePosName, Math.Abs(dealLog.Amount), serialNo, dealLog.SerialNo, account.AccountToken,
                                                                          dealLog.SourceShopName) { IsForce = true },true);
                        r = RollbackType.Undo;
                    //}
                    if (rsp.Code != ResponseCode.Success)
                    {
                        throw new Exception(string.Format("{0}", ModelHelper.GetBoundText(rsp, x => x.Code)));
                    }
                    LogHelper.LogWithSerialNo(LogTypes.DealLogRollback, serialNo, dealLog.DealLogId, dealLog.SerialServerNo, dealLog.SerialNo);
                    tran.Commit();
                    return rsp;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.DealLogRollback, ex);
               // return RollbackType.None;
                return new AccountServiceResponse(-1);
            }
        }

        public void LogRollbackSuccess(string Message)
        {
            this.AddMessage(Message);
        }
        public void LogRollbackError(string Message)
        {
            this.AddError(Message);
        }
    }
}