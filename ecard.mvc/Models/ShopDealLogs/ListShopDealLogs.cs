using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.ShopDealLogs
{
    public class ListShopDealLogs : EcardModelListRequest<ListShopDealLog>
    {
        public ListShopDealLogs()
        {
            OrderBy = "ShopDealLogId Desc";
        }
        public string SerialNo { get; set; }
        [UIHint("Triple")]
        public bool? IsLiquidate { get; set; }
        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public ShopDealLogExecutor ShopDealLogExecutor { get; set; }
        [Dependency, NoRender]
        public IAccountDealService AccountDealService { get; set; }
        [Dependency, NoRender]
        public LogHelper LogHelper { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };
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

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListShopDealLog item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.ShopDealLogId });
            var types = new[] { DealTypes.Deal, DealTypes.CancelDeal, DealTypes.DonePrePay, DealTypes.CancelDonePrePay };

            if (item.TempShop != null
                && new[] { DealLogStates.Normal }.Contains(item.InnerObject.State)
                && (new[] { DealTypes.Deal }.Contains(item.InnerObject.DealType))
                && item.DealLog != null && item.DealLog.LiquidateDealLogId == 0
                )
            {
                yield return new ActionMethodDescriptor("Print", null, new { id = item.InnerObject.ShopDealLogId });
            }
        }

        private Bounded _shopBounded;

        [CheckUserType(typeof(AdminUser))]
        public Bounded Shop
        {
            get
            {
                if (_shopBounded == null)
                {
                    _shopBounded = Bounded.CreateEmpty("ShopId", DealLogQueryTypes.All);
                }
                return _shopBounded;
            }
            set { _shopBounded = value; }
        }
        public string AccountName { get; set; }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<ShopDealLog>("State", ShopDealLogStates.All);
                }
                return _state;
            }
            set { _state = value; }
        }
        public void Query()
        {
            var request = new ShopDealLogRequest();

            if (State != ShopDealLogStates.All)
                request.State = State;
            request.SubmitDateMin = Data.Start;
            request.SubmitDateMax = Data.GetEnd();
            request.AccountName = AccountName;
            if (Shop != Globals.All)
                request.ShopId = Shop;
            request.SerialNo = SerialNo;
            request.IsLiquidate = IsLiquidate;
            var user = SecurityHelper.GetCurrentUser().CurrentUser as ShopUser;
            if (user != null)
                request.ShopId = user.ShopId;
            if (AccountShop != Globals.All)
                request.AccountShopId = AccountShop;
            var query = ShopDealLogService.Query(request);
            this.List = query.ToList(this, x => new ListShopDealLog(x));

            var shopIds = List.Select(x => x.InnerObject.ShopId).Distinct();
            var shops = ShopService.Query(new ShopRequest() { ShopIds = shopIds.ToArray() }).ToList();
            foreach (var log in List)
            {
                var shop = shops.FirstOrDefault(x => x.ShopId == log.InnerObject.ShopId);
                if (shop != null)
                    log.TempShop = shop;
            }

            var dealLogIds = List.Select(x => x.InnerObject.Addin).Distinct();

            var dealLogs = DealLogService.GetByIds(dealLogIds.ToArray()).ToList();
            foreach (var log in List)
            {
                var shop = dealLogs.FirstOrDefault(x => x.DealLogId == log.InnerObject.Addin);
                if (shop != null)
                    log.DealLog = shop;
            }


            // 
            var q = (from x in ShopService.Query(new ShopRequest() { IsBuildIn = false, State = ShopStates.Normal})
                    select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            this.AccountShop.Bind(q, true);
        }

        public void Ready()
        {
            var query = from x in ShopService.Query(new ShopRequest { State = ShopStates.Normal })
                        select new IdNamePair { Key = x.ShopId, Name = x.FormatedName };
            this.Shop.Bind(query, true);
        }
        public RollbackType Rollback(int id)
        {
            try
            {
                using (var tran = TransactionHelper.BeginTransaction())
                {
                    var serialNo = SerialNoHelper.Create();
                    var shopDealLog = this.ShopDealLogService.GetById(id);
                    if (shopDealLog == null)
                    {
                        throw new Exception(Localize("nofoundDeal"));
                    }
                    var dealLog = this.DealLogService.GetById(shopDealLog.Addin);
                    if (dealLog == null)
                    {
                        throw new Exception(Localize("nofoundDeal"));
                    }
                    var account = AccountService.GetById(dealLog.AccountId);
                    if (account == null)
                    {
                        throw new Exception(Localize("nofoundAccount"));
                    }
                    AccountServiceResponse rsp = null;
                    RollbackType r = RollbackType.None;
                    if (dealLog.SubmitTime.Date != DateTime.Now.Date)
                    {
                        rsp = AccountDealService.CancelPay(new CancelPayRequest(dealLog.AccountName, "", dealLog.SourcePosName, Math.Abs(dealLog.Amount), serialNo, dealLog.SerialNo, account.AccountToken,
                                                                       dealLog.SourceShopName) { IsForce = true });
                        r = RollbackType.Cancel;
                    }
                    else
                    {
                        rsp = AccountDealService.Roolback(new PayRequest_(dealLog.AccountName, "", dealLog.SourcePosName, Math.Abs(dealLog.Amount), serialNo, dealLog.SerialNo, account.AccountToken,
                                                                          dealLog.SourceShopName) { IsForce = true });
                        r = RollbackType.Undo;
                    }
                    if (rsp.Code != ResponseCode.Success)
                    {
                        throw new Exception(string.Format("{0}", ModelHelper.GetBoundText(rsp, x => x.Code)));
                    }
                    LogHelper.LogWithSerialNo(LogTypes.DealLogRollback, serialNo, dealLog.DealLogId, dealLog.SerialServerNo, dealLog.SerialNo);
                    tran.Commit();
                    return r;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.DealLogRollback, ex);
                return RollbackType.None;
            }
        }

        public void LogRollbackSuccess(int cancelCount, int undoCount)
        {
            this.AddMessage("rollback", cancelCount, undoCount);
        }
    }
}
