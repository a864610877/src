using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit;
using Microsoft.Practices.Unity;

namespace Ecard.Services
{
    public class AccountDealService : IAccountDealService
    {
        private readonly IAccountDealDal _accountDealDal;
        private readonly IDealTracker _dealTracker;
        private readonly IPosKeyService _posKeyService;
        private SqlOrder1Service OrderService { get; set; }
        public AccountDealService(IAccountDealDal accountDealDal, IDealTracker dealTracker, SqlOrder1Service orderService, IPosKeyService posKeyService)
        {
            _accountDealDal = accountDealDal;
            _dealTracker = dealTracker;
            OrderService = orderService;
            _posKeyService = posKeyService;
        }

        public AccountServiceResponse DonePrePay(PayRequest request)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            // 系统强制执行
            if (request.IsForce)
                posAndShop = GetSystemPosAndShop();
            rsp = AccountServiceValidator.ValidatePos(posAndShop);

            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            var account = _accountDealDal.GetAccountByName(request.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, request.Password, request.UserToken, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var owner = _accountDealDal.GetUserById(account);

            PrePay prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);

            if (prePay == null)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            }
            if (prePay.Amount < request.Amount)
            {
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            }

            return InnerDonePrePay(request, pos, shop, prePay, account, owner, true);
        }
        /// <summary>
        /// 预授权完成撤销
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountServiceResponse CancelDonePrePay(CancelPayRequest request)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            // 系统强制执行
            if (request.IsForce)
                posAndShop = GetSystemPosAndShop();
            rsp = AccountServiceValidator.ValidatePos(posAndShop);

            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            var account = _accountDealDal.GetAccountByName(request.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, request.Password, request.UserToken, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var owner = _accountDealDal.GetUserById(account);
            DealLog oldDealItem = _accountDealDal.GetDealLog(pos.Name, request.ShopName, request.OldSerialNo);
            if (oldDealItem.DealType != DealTypes.DonePrePay)
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);

            //PrePay prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Complted);
            PrePay prePay = _accountDealDal.GetPrePay(oldDealItem.Addin);
            if (prePay.State != PrePayStates.Complted||prePay.AccountId!=account.AccountId||pos.PosEndPointId!=prePay.PosId)
                       return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            if (prePay == null)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            }
            if (prePay.Amount < request.Amount)
            {
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            }
           
            return InnerCancelDonePrePay(request, prePay, pos, shop, account, oldDealItem);
        }

        private PosAndShop GetSystemPosAndShop()
        {
            return new PosAndShop()
                       {
                           PosEndPoint = new PosEndPoint()
                                             {
                                                 DisplayName = "System",
                                                 Name = "System",
                                                 State = PosEndPointStates.Normal,
                                             },
                           Shop = new Shop()
                                      {
                                          Name = "System",
                                          DisplayName = "System",
                                          State = ShopStates.Normal,
                                      }
                       };
        }

        /// <summary>
        /// 消费（扣卡余额）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountServiceResponse Pay(PayRequest request,bool IsWeb=false)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var posAndShop = new PosAndShop(); 
            if (IsWeb == true)
            {
                request.UserToken = null;
                posAndShop.Shop = _accountDealDal.GetShop(request.ShopName);
                if (posAndShop.Shop == null)
                    return new AccountServiceResponse(ResponseCode.InvalidateShop);
                if (posAndShop.PosEndPoint == null)
                    posAndShop.PosEndPoint = new PosEndPoint();
            }
            else
            {
               posAndShop= _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
                rsp = AccountServiceValidator.ValidatePos(posAndShop);
            }

            if (rsp.Code != ResponseCode.Success)
                return rsp;

            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);

            var cardType = _accountDealDal.GetAccounTypes().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId);
            rsp = AccountServiceValidator.ValidateAccountType1(cardType);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            AccountUser owner = _accountDealDal.GetUserById(account);

            var pay = _accountDealDal.GetPrePay(posAndShop.PosEndPoint.PosEndPointId, account.AccountId, PrePayStates.Processing);
            if (pay != null)
                return InnerDonePrePay(request, posAndShop.PosEndPoint, posAndShop.Shop, pay, account, owner, true);
           
            rsp = AccountServiceValidator.ValidateAccount(account, posAndShop.Shop, request.Password, request.UserToken);

            if (rsp.Code != ResponseCode.Success)
                return rsp;

            //if (!account.HasEnoughAmount(request.Amount))//信用额度
            //{
            //    return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            //}

            // shop to
            var shopTo = GetShopTo(request.ShopNameTo, posAndShop.Shop);
            rsp = AccountServiceValidator.ValidateShop(shopTo);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var guid = Guid.NewGuid().ToString();

            ShopUser shopOwner = _accountDealDal.GetShopUser(shopTo);

            var Amountpoint = GetPoint(request, account, owner, cardType);//计算积分折扣
            var chargeAmount = GetCharge(request, account);//计算手续费
            if (account.Amount < Amountpoint.Amount)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            //DoAmountRate(account);
            account.Pay(Amountpoint.Amount, Amountpoint.Point);
            var dealItem = new DealLog(request.SeriaNo);
            dealItem.DealType = DealTypes.Deal;
            dealItem.Amount = Amountpoint.Amount;// request.Amount;
            dealItem.YAmount = request.Amount;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Pos = posAndShop.PosEndPoint;
            dealItem.SourceShop = posAndShop.Shop;
            dealItem.Account = account;
            dealItem.Shop = shopTo;
            dealItem.Point = Amountpoint.Point;
            dealItem.DiscountRate = Amountpoint.DiscountRate;
            dealItem.AccountAmount = dealItem.AccountAmount;
            _accountDealDal.Save(dealItem);

            ShopDealLog shopDealLog = dealItem.ToShopDealLog(shopTo);

            var site = _accountDealDal.GetSite();
            if (!string.IsNullOrWhiteSpace(site.MessageTemplateOfDealCode))
                shopDealLog.Code = Math.Abs(guid.ToString().GetHashCode()).ToString().PadLeft(8, '0').Substring(0, 8);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shopTo);
            if (chargeAmount != 0)
            {
                if (!account.HasEnoughAmount(chargeAmount))
                    return new AccountServiceResponse(ResponseCode.InvalidateAmount);

                account.PayCharge(chargeAmount);
                dealItem = new DealLog(SerialNoHelper.Create(), DealTypes.DealCharge, chargeAmount, 0, null, null, account, null, dealItem.DealLogId);
                _accountDealDal.Save(dealItem);

                //var systemDealLog = new SystemDealLog(request.SeriaNo, null) { Addin = dealItem.DealLogId.ToString(), Amount = chargeAmount, DealWayId = 0, DealType = SystemDealLogTypes.AccountDealCharge };
                //_accountDealDal.Save(systemDealLog);
            }
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            //OrderComplete(account, owner, dealItem);
            if (cardType.IsSmsDeal)
               _dealTracker.Notify(account, owner, dealItem);
            //if (shopTo.ShopType == ShopTypes.OutOfClub)
            //{
            //    _dealTracker.NotifyCode(posAndShop.Shop, shopTo, shopDealLog, request.ShopNameTo);
            //}

            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, shopTo, shopOwner) { DealAmount = Amountpoint.Amount };
        }
        IEnumerable<AmountRate> GetAmountRate(int level, int accountTypeId)
        {
            var acccountLevel = _accountDealDal.GetAccountLevelPolicies().FirstOrDefault(x => x.AccountTypeId == accountTypeId && x.Level == level);
            if (acccountLevel == null) yield break;

            var rates = _accountDealDal.GetAmountRate();
            foreach (var amountRate in rates.Where(x => x.AccountLevel == acccountLevel.AccountLevelPolicyId).OrderByDescending(x => x.Days))
            {
                yield return amountRate;
            }
        }
        public void OrderComplete(Account account, AccountUser owner, DealLog dealItem)
        {
            

            //完成订单：
            if (dealItem.DealType == DealTypes.Integral || dealItem.DealType == DealTypes.DonePrePay || dealItem.DealType == DealTypes.Deal)
            {
                var items = OrderService.QueryOrder(new OrderRequest() { AccountId = account.AccountId }).ToList();
                var item1 = items.Where(x => x.TotalMoney == dealItem.Amount).OrderByDescending(x => x.SubmitTime).FirstOrDefault();
                if (item1 != null)
                {
                    if (item1.State == OrderState.Carry)
                    {
                        item1.State = OrderState.Completed;
                        item1.SubmitTime = DateTime.Now;
                        OrderService.UpdateOrder(item1);
                    }
                }
            }
            //撤销
            if (dealItem.DealType == DealTypes.CancelDeal || dealItem.DealType == DealTypes.CancelDonePrePay || dealItem.DealType == DealTypes.CancelIntegral)
            {
                var items = OrderService.QueryOrder(new OrderRequest() { AccountId = account.AccountId }).Where(x => x.State == OrderState.Completed).ToList();
                var item1 = items.Where(x => x.TotalMoney == Math.Abs( dealItem.Amount)).OrderByDescending(x => x.SubmitTime).FirstOrDefault();
                if (item1 != null)
                {
                    item1.State = OrderState.Invalid;
                    item1.SubmitTime = DateTime.Now;
                    OrderService.UpdateOrder(item1);
                }
            }
            //-------
        }
        private void DoAmountRate(Account account)
        {
            IEnumerable<AmountRate> rates = GetAmountRate(account.AccountLevel, account.AccountTypeId);
            var now = DateTime.Now;
            while (true)
            {
                var days = now - account.LastDealTime;
                var rate = rates.FirstOrDefault(x => x.Days <= days.TotalDays);
                if (rate != null)
                {
                    decimal amount = 0m;
                    if (rate.Rate != null)
                        amount = account.Amount * rate.Rate.Value;
                    else if (rate.Amount != null)
                        amount = rate.Amount.Value;
                    account.Amount = amount.ToRound(2) + account.Amount;
                    account.LastDealTime = account.LastDealTime.AddDays(rate.Days);
                    var dealItem = new DealLog("");
                    dealItem.DealType = DealTypes.InterestSettlement;
                    dealItem.Amount = amount;
                    dealItem.SourceShopName = "";
                    dealItem.SourcePosName = "";
                    dealItem.SubmitTime = DateTime.Now;
                    dealItem.SourceShopDisplayName = "";
                    dealItem.SourcePosId = 0;
                    dealItem.SourceShopId = 0;
                    dealItem.Account = account;
                    dealItem.Point = 0;

                    _accountDealDal.Save(dealItem);
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 计算本次交易积分及折扣后金额
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        /// <param name="owner"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        private PointAmount GetPoint(PayRequest request, Account account, AccountUser owner,AccountType accountType=null)
        {
            //var accountLevel = _accountDealDal.GetAccountLevelPolicies().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId && x.Level == account.AccountLevel);
            //if (accountLevel == null) return 0;

            //var pointPolicy = _accountDealDal.GetPointPolicies()
            //    .Where(x => x.IsFor(account, owner, accountLevel, DateTime.Now)).OrderByDescending(x => x.Priority).FirstOrDefault();

            //var point = (int)(pointPolicy == null ? 0 : pointPolicy.Point * request.Amount);
            //return point;
            PointAmount result = new PointAmount();
            result.Point = 0;
            result.Amount = request.Amount;
            result.DiscountRate = 100;
            var accountLevel = _accountDealDal.GetAccountLevelPolicies().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId && x.Level == account.AccountLevel);
            if (accountLevel == null) return result;
            int point = 0;
            decimal NewAmount = accountLevel.DiscountRate == 0 ? request.Amount : accountLevel.DiscountRate * request.Amount;
            if (accountType != null && accountType.IsPointable)
            {
                var pointPolicy = _accountDealDal.GetPointPolicies()
                    .Where(x => x.IsFor(account, owner, accountLevel, DateTime.Now)).OrderByDescending(x => x.Priority).FirstOrDefault();
                 point = (int)(pointPolicy == null ? 0 : pointPolicy.Point * NewAmount);
            }
            result.Amount = NewAmount;
            result.DiscountRate = accountLevel.DiscountRate;
            result.Point = point;
            return result;
        }
        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private decimal GetCharge(PayRequest request, Account account)
        {
            var accountLevel = _accountDealDal.GetAccountLevelPolicies().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId && x.Level == account.AccountLevel);
            if (accountLevel == null) return 0;

            return 0;
        } 
        /// <summary>
        /// 预授权
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountServiceResponse PrePay(PayRequest request)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, request.Password, request.UserToken);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            if (!account.HasEnoughAmount(request.Amount))
            {
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            }
            PrePay prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);
            if (prePay == null)
            {
                prePay = new PrePay();
                prePay.CopyFromPos(pos, shop);
                prePay.CopyFromAccount(account);
            }
            AccountUser owner = _accountDealDal.GetUserById(account);
            var dealLog = new DealLog(request.SeriaNo);
            account.PrePay(request.Amount);
            prePay.DoPrePay(request.Amount);
            dealLog.DealType = DealTypes.PrePay;
            dealLog.Amount = request.Amount;
            dealLog.SourcePosId = pos.PosEndPointId;
            dealLog.SourcePosName = pos.Name;
            dealLog.SubmitTime = DateTime.Now;
            dealLog.AccountId = account.AccountId;
            dealLog.AccountAmount = account.Amount;
            dealLog.AccountName = request.AccountName;
            dealLog.SourceShop = shop;
            dealLog.Shop = shop;
            dealLog.Point = 0;
            dealLog.IsHidden = true;
            _accountDealDal.Save(prePay);
            dealLog.Addin = prePay.PrePayId;
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            _accountDealDal.Save(dealLog);
            _dealTracker.Notify(account, owner, dealLog);
            return new AccountServiceResponse(ResponseCode.Success, dealLog, _accountDealDal.GetShop(account.ShopId), account, null, null, null);
        }

        public AccountServiceResponse CancelPay(CancelPayRequest request,bool IsWeb=false)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var posAndShop = new PosAndShop();
            if (IsWeb == true)
            {
                request.UserToken = null;
                posAndShop.Shop = _accountDealDal.GetShop(request.ShopName);
                if (posAndShop.Shop == null)
                    return new AccountServiceResponse(ResponseCode.InvalidateShop);
                if (posAndShop.PosEndPoint == null)
                    posAndShop.PosEndPoint = new PosEndPoint();
            }
            else
            {
                posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
                rsp = AccountServiceValidator.ValidatePos(posAndShop);
            }
            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;


            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, request.Password, request.UserToken, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            DealLog oldDealItem = _accountDealDal.GetDealLog(pos.Name, request.ShopName, request.OldSerialNo);
            if (oldDealItem == null || oldDealItem.AccountName != request.AccountName || (oldDealItem.DealType!=DealTypes.Recharging&&oldDealItem.DealType != DealTypes.Deal && oldDealItem.DealType != DealTypes.DonePrePay && oldDealItem.DealType != DealTypes.PayIntegral && oldDealItem.DealType != DealTypes.Integral))
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);

            if (oldDealItem.DealType == DealTypes.DonePrePay)
            {
                var pay = _accountDealDal.GetPrePay(oldDealItem.Addin);
                if (pay == null || pay.State != PrePayStates.Complted)
                    return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                var processingPay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);
                if (processingPay != null)
                    return new AccountServiceResponse(ResponseCode.NonFoundDeal);   // 如果已经有正在交易的交易出现，则不允许交易
                return InnerCancelDonePrePay(request, pay, pos, shop, account, oldDealItem);
            }

            rsp = AccountServiceValidator.ValidateDealItem(oldDealItem, request.ShopName, pos.Name, request.Amount, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            }
            var dealItem = new DealLog(request.SerialNo);
            int point = oldDealItem.Point;
             if (oldDealItem.DealType==DealTypes.Integral)
            {
                //现金交易，撤销时，要把积分扣减出来。2.要扣减已消费的金额总数。
                account.IntegralCancel(oldDealItem.Amount, point);
                //return rsp;
            }
             else if (oldDealItem.DealType == DealTypes.PayIntegral)
             {
                 //积分交易的撤销。增加客户的未使用积分
                 account.PayIntegralCancel(point);
             }
             else if (oldDealItem.DealType == DealTypes.Recharging)
             {
                 //撤销充值
                 account.RechargeCancel(oldDealItem.Amount);
             }
             else
             {
                 account.PayCancel(oldDealItem.Amount, point);
             }
            // shop to
            var shopTo = GetShopTo(oldDealItem.ShopName, posAndShop.Shop);
            rsp = AccountServiceValidator.ValidateShop(shopTo);
            if (rsp.Code != ResponseCode.Success)
                return rsp;


            if (shopTo.Amount - oldDealItem.Amount < 0)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);

            ShopUser shopOwner = _accountDealDal.GetShopUser(shopTo);
            dealItem.DealType = oldDealItem.DealType == DealTypes.Recharging ? DealTypes.CancelRecharging : DealTypes.CancelDeal;
            dealItem.Amount = -oldDealItem.Amount;
            dealItem.SourceShop = shop;
            dealItem.Pos = pos;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Account = account;
            dealItem.Addin = oldDealItem.DealLogId;
            dealItem.Point = -oldDealItem.Point;
            dealItem.Shop = shopTo;
            oldDealItem.State = DealLogStates.Cancel;
            _accountDealDal.Save(oldDealItem);

            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            _accountDealDal.Save(dealItem);

            var shopDealLog = dealItem.ToShopDealLog(shopTo);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shopTo);

            var oldShopDealLog = _accountDealDal.GetShopDealLogByAddin(oldDealItem.DealLogId);
            oldShopDealLog.State = DealLogStates.Cancel;
            _accountDealDal.Save(oldShopDealLog);
            AccountUser owner = _accountDealDal.GetUserById(account);
            OrderComplete(account, owner, dealItem);
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, null, shopTo, shopOwner);

        }

        private Shop GetShopTo(string shopNameTo, Shop shop)
        {
            if (string.IsNullOrWhiteSpace(shopNameTo) || shopNameTo.EqualsIgnoreCase(shop.Name))
                return shop;

            var shopTo = _accountDealDal.GetShop(shopNameTo);
            if (shopTo == null && shop.ShopType == ShopTypes.OutOfClub) return shop;
            return shopTo;
        }

        public AccountServiceResponse CancelPrePay(CancelPayRequest request)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            if (request.IsForce)
                posAndShop = GetSystemPosAndShop();

            rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            var account = _accountDealDal.GetAccountByName(request.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, request.Password, request.UserToken, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            account.CancelPrePay(request.Amount);

            var prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);
            if (prePay == null || prePay.State == PrePayStates.Complted)
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);

            if (prePay.Amount < request.Amount)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);

            prePay.CancelPrePay(request.Amount);
            if (prePay.Amount == 0m)
            {
                _accountDealDal.Delete(prePay);
            }
            else
                _accountDealDal.Save(prePay);

            var dealItem = new DealLog(request.SerialNo);
            dealItem.DealType = DealTypes.CancelPreDeal;
            dealItem.Amount = -request.Amount;
            dealItem.SourcePosId = pos.PosEndPointId;
            dealItem.SourceShopName = shop.Name;
            dealItem.SourceShopDisplayName = shop.DisplayName;
            dealItem.SourceShopId = pos.ShopId;
            dealItem.AccountId = account.AccountId;
            dealItem.SourcePosName = pos.Name;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.AccountName = account.Name;
            dealItem.AccountAmount = account.Amount;
            dealItem.Shop = shop;
            dealItem.Addin = 0;
            dealItem.Point = 0;
            dealItem.IsHidden = true;
            _accountDealDal.Save(dealItem);
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, null, null, null);

        }

        public AccountServiceResponse QueryShop(string posName, string shopName, string shopToName)
        {
            var posAndShop = _accountDealDal.GetPosAndShopByName(posName, shopName);
            var rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            Shop shopTo = GetShopTo(shopToName, posAndShop.Shop);
            rsp = AccountServiceValidator.ValidateShop(shopTo);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var dealway = _accountDealDal.GetDealWay(shopTo.DealWayId);

            rsp = new AccountServiceResponse(ResponseCode.Success, null, null, null, null, shopTo, _accountDealDal.GetShopUser(shopTo));
            if (dealway != null)
                rsp.ShopToDealWay = dealway.DisplayName;
            return rsp;
        }

        /// <summary>
        /// 会员卡交易（单积分）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountServiceResponse Integral(PayRequest request,bool IsWeb=false)
        {
            // todo: 实现单积分逻辑,现金支付，生成消费记录，增加积分，。如果是现金卡，则不能现金支付。 
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var posAndShop = new PosAndShop();
            if (IsWeb == true)
            {
                request.UserToken = null;
                posAndShop.Shop = _accountDealDal.GetShop(request.ShopName);
                if (posAndShop.Shop == null)
                    return new AccountServiceResponse(ResponseCode.InvalidateShop);
                if (posAndShop.PosEndPoint == null)
                    posAndShop.PosEndPoint = new PosEndPoint();
            }
            else
            {
                posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
                rsp = AccountServiceValidator.ValidatePos(posAndShop);
            }

            if (rsp.Code != ResponseCode.Success)
                return rsp;

            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            var cardType = _accountDealDal.GetAccounTypes().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId);
            rsp = AccountServiceValidator.ValidateAccountType2(cardType);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            AccountUser owner = _accountDealDal.GetUserById(account);
            //---------------
            var pay = _accountDealDal.GetPrePay(posAndShop.PosEndPoint.PosEndPointId, account.AccountId, PrePayStates.Processing);//是否有预授权
            if (pay != null)
                return InnerDonePrePay(request, posAndShop.PosEndPoint, posAndShop.Shop, pay, account, owner, true);
            //---------------
            //rsp = AccountServiceValidator.ValidateAccount(account, posAndShop.Shop, request.Password, request.UserToken);

            //if (rsp.Code != ResponseCode.Success)
            //    return rsp;
            ////现金支付不需要检查会员卡余额。
            //if (!account.HasEnoughAmount(request.Amount))
            //{
            //    return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            //}

            // shop to
            var shopTo = GetShopTo(request.ShopNameTo, posAndShop.Shop);
            rsp = AccountServiceValidator.ValidateShop(shopTo);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var guid = Guid.NewGuid().ToString();

            ShopUser shopOwner = _accountDealDal.GetShopUser(shopTo);

            var Amountpoint = GetPoint(request, account, owner,cardType);
            var chargeAmount = GetCharge(request, account);

            //DoAmountRate(account);//不需要
            account.IntegralPay(Amountpoint.Amount, Amountpoint.Point);
            var dealItem = new DealLog(request.SeriaNo);
            dealItem.DealType = DealTypes.Integral;
            dealItem.Amount = Amountpoint.Amount;
            dealItem.YAmount = request.Amount;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Pos = posAndShop.PosEndPoint;
            dealItem.SourceShop = posAndShop.Shop;
            dealItem.Account = account;
            dealItem.Shop = shopTo;
            dealItem.Point = Amountpoint.Point;
            dealItem.DiscountRate = Amountpoint.DiscountRate;
            dealItem.AccountAmount = dealItem.AccountAmount;
            _accountDealDal.Save(dealItem);

            ShopDealLog shopDealLog = dealItem.ToShopDealLog(shopTo);

            var site = _accountDealDal.GetSite();
            if (!string.IsNullOrWhiteSpace(site.MessageTemplateOfDealCode))
                shopDealLog.Code = Math.Abs(guid.ToString().GetHashCode()).ToString().PadLeft(8, '0').Substring(0, 8);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shopTo);
            if (chargeAmount != 0)//手续费。
            {
                if (!account.HasEnoughAmount(chargeAmount))
                    return new AccountServiceResponse(ResponseCode.InvalidateAmount);

                account.PayCharge(chargeAmount);
                dealItem = new DealLog(SerialNoHelper.Create(), DealTypes.DealCharge, chargeAmount, 0, null, null, account, null, dealItem.DealLogId);
                _accountDealDal.Save(dealItem);

                //var systemDealLog = new SystemDealLog(request.SeriaNo, null) { Addin = dealItem.DealLogId.ToString(), Amount = chargeAmount, DealWayId = 0, DealType = SystemDealLogTypes.AccountDealCharge };
                //_accountDealDal.Save(systemDealLog);
            }
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            if(cardType.IsSmsDeal)
              _dealTracker.Notify(account, owner, dealItem); 
            OrderComplete(account, owner, dealItem);//交易完成的操作
            //if (shopTo.ShopType == ShopTypes.OutOfClub)
            //{
            //    _dealTracker.NotifyCode(posAndShop.Shop, shopTo, shopDealLog, request.ShopNameTo);
            //}

            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, shopTo, shopOwner) { DealAmount = Amountpoint.Amount };
        }

        public AccountServiceResponse PayIntegral(PayRequest request)
        {
            // todo: 实现积分+消费逻辑//客户端会传进消费的积分。
            //积分消费只扣减积分，不扣减余额，
            //取整
            request.Amount = Math.Round(request.Amount, 0, MidpointRounding.AwayFromZero);
            AccountServiceResponse rsp;
            var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            rsp = AccountServiceValidator.ValidatePoint(account, (int)request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            AccountUser owner = _accountDealDal.GetUserById(account);
            ////---------------积分消费不用
            //var pay = _accountDealDal.GetPrePay(posAndShop.PosEndPoint.PosEndPointId, account.AccountId, PrePayStates.Processing);//是否有预授权
            //if (pay != null)
            //    return InnerDonePrePay(request, posAndShop.PosEndPoint, posAndShop.Shop, pay, account, owner, true);
            ////---------------
            rsp = AccountServiceValidator.ValidateAccount(account, posAndShop.Shop, request.Password, request.UserToken);

            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var shopTo = GetShopTo(request.ShopNameTo, posAndShop.Shop);
            rsp = AccountServiceValidator.ValidateShop(shopTo);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var guid = Guid.NewGuid().ToString();
            ShopUser shopOwner = _accountDealDal.GetShopUser(shopTo);
            //var point = GetPoint(request, account, owner);//积分交易不用增加会员卡的积分。
            var chargeAmount = GetCharge(request, account);
            account.PayIntegralPay(request.Amount);
            var dealItem = new DealLog(request.SeriaNo);
            dealItem.DealType = DealTypes.PayIntegral;
            dealItem.Amount = 0m;//积分交易的金额为0.
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Pos = posAndShop.PosEndPoint;
            dealItem.SourceShop = posAndShop.Shop;
            dealItem.Account = account;
            dealItem.Shop = shopTo;
            dealItem.Point = (int)request.Amount;
            dealItem.AccountAmount = dealItem.AccountAmount;
            _accountDealDal.Save(dealItem);
            ShopDealLog shopDealLog = dealItem.ToShopDealLog(shopTo);

            var site = _accountDealDal.GetSite();
            if (!string.IsNullOrWhiteSpace(site.MessageTemplateOfDealCode))
                shopDealLog.Code = Math.Abs(guid.ToString().GetHashCode()).ToString().PadLeft(8, '0').Substring(0, 8);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shopTo);
            if (chargeAmount != 0)//手续费。
            {
                if (!account.HasEnoughAmount(chargeAmount))
                    return new AccountServiceResponse(ResponseCode.InvalidateAmount);

                account.PayCharge(chargeAmount);
                dealItem = new DealLog(SerialNoHelper.Create(), DealTypes.DealCharge, chargeAmount, 0, null, null, account, null, dealItem.DealLogId);
                _accountDealDal.Save(dealItem);

                //var systemDealLog = new SystemDealLog(request.SeriaNo, null) { Addin = dealItem.DealLogId.ToString(), Amount = chargeAmount, DealWayId = 0, DealType = SystemDealLogTypes.AccountDealCharge };
                //_accountDealDal.Save(systemDealLog);
            }
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            _dealTracker.Notify(account, owner, dealItem); //交易完成的操作
            if (shopTo.ShopType == ShopTypes.OutOfClub)
            {
                _dealTracker.NotifyCode(posAndShop.Shop, shopTo, shopDealLog, request.ShopNameTo);
            }
          //  return new AccountServiceResponse(ResponseCode.Success) { SerialServerNo = request.SeriaNo, AccountName = request.AccountName, Amount = request.Amount, Point = 100, ThisTimePoint=-100 };
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, shopTo, shopOwner) { DealAmount = (int)request.Amount };

        }

        public AccountServiceResponse Recharge(PayRequest request,bool IsWeb=false)
        {
            // todo: 充值逻辑
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            //var posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
            //rsp = AccountServiceValidator.ValidatePos(posAndShop);
            var posAndShop = new PosAndShop();
            if (IsWeb == true)
            {
                request.UserToken = null;
                posAndShop.Shop = _accountDealDal.GetShop(request.ShopName);
                if (posAndShop.Shop == null)
                    return new AccountServiceResponse(ResponseCode.InvalidateShop);
                if (posAndShop.PosEndPoint == null)
                    posAndShop.PosEndPoint = new PosEndPoint();
            }
            else
            {
                posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
                rsp = AccountServiceValidator.ValidatePos(posAndShop);
            }
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            Account account = _accountDealDal.GetAccountByName(request.AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            var cardType = _accountDealDal.GetAccounTypes().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId);
            rsp = AccountServiceValidator.ValidateAccountType(cardType);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            AccountUser owner = _accountDealDal.GetUserById(account);
            rsp = AccountServiceValidator.ValidateAccount(account, posAndShop.Shop, request.Password, request.UserToken,true);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var shopTo = posAndShop.Shop;
            var guid = Guid.NewGuid().ToString();
            ShopUser shopOwner = _accountDealDal.GetShopUser(shopTo);
            if(request.Amount>posAndShop.Shop.RechargeAmount)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            account.Recharge(request.Amount);
            shopTo.RechargeAmount -= request.Amount;
            var dealItem = new DealLog(request.SeriaNo);
            dealItem.DealType = DealTypes.Recharging;
            dealItem.Amount = request.Amount;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Pos = posAndShop.PosEndPoint;
            dealItem.SourceShop = posAndShop.Shop;
            dealItem.Account = account;
            dealItem.Shop = shopTo;
            dealItem.Point = 0;
            dealItem.AccountAmount = dealItem.AccountAmount;
            _accountDealDal.Save(dealItem);
            ShopDealLog shopDealLog = dealItem.ToShopDealLog(shopTo);
            var site = _accountDealDal.GetSite();
            if (!string.IsNullOrWhiteSpace(site.MessageTemplateOfDealCode))
                shopDealLog.Code = Math.Abs(guid.ToString().GetHashCode()).ToString().PadLeft(8, '0').Substring(0, 8);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shopTo);
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            if(cardType.IsSmsRecharge)
               _dealTracker.Notify(account, owner, dealItem);// OrderComplete(account, owner, dealItem);
            //if (shopTo.ShopType == ShopTypes.OutOfClub)
            //{
            //    _dealTracker.NotifyCode(posAndShop.Shop, shopTo, shopDealLog, request.ShopNameTo);
            //}
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, shopTo, shopOwner) { DealAmount=request.Amount};
        }

        public AccountServiceResponse Roolback(PayRequest_ request,bool IsWeb=false)
        {
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAmount(request.Amount);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var posAndShop = new PosAndShop();
            if (IsWeb)
            {
                request.UserToken = null;
                posAndShop.Shop = _accountDealDal.GetShop(request.ShopName);
                if (posAndShop.Shop == null)
                    return new AccountServiceResponse(ResponseCode.InvalidateShop);
                if (posAndShop.PosEndPoint == null)
                    posAndShop.PosEndPoint = new PosEndPoint();
            }
            else
            {
                posAndShop = _accountDealDal.GetPosAndShopByName(request.PosName, request.ShopName);
                rsp = AccountServiceValidator.ValidatePos(posAndShop);
            }
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            DealLog dealItem = _accountDealDal.GetDealLog(pos.Name, request.ShopName, request.OldSerialNo);
            if (IsWeb)
                pos.Name = dealItem.SourcePosName;
            rsp = AccountServiceValidator.ValidateDealItem(dealItem, request.ShopName, pos.Name, request.Amount, request.IsForce);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            if (!request.IsForce && (DateTime.Now - dealItem.SubmitTime) > TimeSpan.FromMinutes(1))
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            var account = _accountDealDal.GetAccountByName(dealItem.AccountName);
            rsp = AccountServiceValidator.ValidateAccount(account);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            var dealType = dealItem.DealType;

            var dealPoint = dealItem.Point;
            var dealAmount = dealItem.Amount;
            var shopTo = GetShopTo(dealItem.ShopName, shop);
            var owner = _accountDealDal.GetUserById(account);


            var chargeDealLog = _accountDealDal.GetDealLog(DealTypes.DealCharge, dealItem.DealLogId);
            if (chargeDealLog != null)
            {
                account.CancelPayCharge(chargeDealLog.Amount);
                chargeDealLog.State = DealLogStates.Normal_;
                _accountDealDal.Save(chargeDealLog);

                var invaildChargeDeal = new DealLog(SerialNoHelper.Create(), DealTypes.Invalid,
                                      -(chargeDealLog.Amount), 0,
                                      pos, shop, account, null, dealItem.DealLogId);
                _accountDealDal.Save(invaildChargeDeal);

            }
            switch (dealType)
            {
                case DealTypes.Deal:
                    {
                        account.PayCancel(dealItem.Amount, dealItem.Point);
                        break;
                    }
                case DealTypes.Integral:
                    {
                        account.IntegralCancel(dealItem.Amount, dealItem.Point);
                        break;
                    }
                case DealTypes.PayIntegral:
                    {
                        account.PayIntegralCancel((int)dealItem.Amount);
                        break;
                    }
                case DealTypes.Recharging:
                    {
                        //根据冲正中的流水号取出 deallog 交易
                        //在 deallog 中添加记录，类别为冲正，addinId 为 被撤销的交易的 dealLogId
                        //在 deallog 中添加记录，类别为冲正，addinId 为 被撤销的交易的 dealLogId, 金额为 负 充值金额 
                        //将 deallogitem.state 设置为被冲正
                        //systemdeallog 和 cashdeallog 我还没想好怎么处理，也许可以直接删除掉
                        //如果撤销交易被冲正的话，原交易要由已撤销改为正常状态
                        var oldDealLog = _accountDealDal.GetDealLog(dealItem.Addin);
                        if (oldDealLog == null || oldDealLog.State != DealLogStates.Cancel)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        dealItem.Addin = oldDealLog.DealLogId;
                        break;
                    }

                case DealTypes.CancelIntegral:
                    {
                        //撤销现金交易
                        var oldDealLog = _accountDealDal.GetDealLog(dealItem.Addin);
                        if (oldDealLog == null || oldDealLog.State != DealLogStates.Cancel)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        dealItem.Addin = oldDealLog.DealLogId;
                        account.IntegralCancel(dealItem.Amount, dealItem.Point);
                        oldDealLog.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldDealLog);
                        var oldShopDeal = _accountDealDal.GetShopDealLogByAddin(oldDealLog.Addin);
                        oldShopDeal.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldShopDeal);
                        break;
                    }
                case DealTypes.CancelPayIntegral:
                    {
                        //撤销积分消费交易
                        var oldDealLog = _accountDealDal.GetDealLog(dealItem.Addin);
                        if (oldDealLog == null || oldDealLog.State != DealLogStates.Cancel)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        dealItem.Addin = oldDealLog.DealLogId;
                        account.PayIntegralCancel((int)dealItem.Amount);
                        oldDealLog.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldDealLog);
                        var oldShopDeal = _accountDealDal.GetShopDealLogByAddin(oldDealLog.Addin);
                        oldShopDeal.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldShopDeal);
                        break;
                    }
                case DealTypes.CancelDeal:
                    {
                        var oldDealLog = _accountDealDal.GetDealLog(dealItem.Addin);
                        if (oldDealLog == null || oldDealLog.State != DealLogStates.Cancel)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        dealItem.Addin = oldDealLog.DealLogId;
                        account.PayCancel(dealItem.Amount, dealItem.Point);
                        oldDealLog.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldDealLog);

                        var oldShopDeal = _accountDealDal.GetShopDealLogByAddin(oldDealLog.Addin);
                        oldShopDeal.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldShopDeal);
                        //shopTo.Amount += oldDealLog.Amount;
                        break;
                    }
                case DealTypes.PrePay:
                    {
                        account.CancelPrePay(dealItem.Amount);
                        PrePay prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);
                        if (prePay == null)
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        rsp = AccountServiceValidator.ValidatePrePayForDeal_PrePay(prePay);
                        if (rsp.Code != ResponseCode.Success)
                            return rsp;

                        prePay.CancelPrePay(dealItem.Amount);
                        if (prePay.Amount == 0)
                            _accountDealDal.Delete(prePay);
                        else
                            _accountDealDal.Save(prePay);
                        break;
                    }
                case DealTypes.CancelPreDeal:
                    {
                        var amount = Math.Abs(dealItem.Amount);
                        PrePay prePay = _accountDealDal.GetPrePay(pos.PosEndPointId, account.AccountId, PrePayStates.Processing);
                        if (prePay == null)
                        {
                            prePay = new PrePay();
                            prePay.CopyFromPos(pos, shop);
                            prePay.CopyFromAccount(account);
                            prePay.State = PrePayStates.Processing;
                        }
                        account.PrePay(amount);
                        prePay.DoPrePay(amount);
                        _accountDealDal.Save(prePay);
                        break;
                    }
                case DealTypes.DonePrePay:
                    {
                        var amount = Math.Abs(dealItem.Amount);
                        PrePay prePay = _accountDealDal.GetPrePay(dealItem.Addin);
                        if (prePay == null || prePay.State != PrePayStates.Complted)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }

                        rsp = AccountServiceValidator.ValidatePrePayForDeal_DonePrePay(prePay);
                        if (rsp.Code != ResponseCode.Success)
                            return rsp;

                        account.PrePay(prePay.Amount);
                        account.PayCancel(amount, dealItem.Point);
                        prePay.CancelDonePrePay(amount);
                        _accountDealDal.Save(prePay);
                        break;
                    }
                case DealTypes.CancelDonePrePay:
                    {
                        var oldDealLog = _accountDealDal.GetDealLog(dealItem.Addin);
                        if (oldDealLog == null || oldDealLog.State != DealLogStates.Cancel)
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        dealItem.Addin = oldDealLog.DealLogId;
                        PrePay prePay = _accountDealDal.GetPrePay(oldDealLog.Addin);
                        if (prePay.State == PrePayStates.Processing)
                        {
                            var payRequest = new PayRequest(request.AccountName, request.Password, request.PosName, request.Amount, request.SerialNo, request.UserToken, request.ShopName, dealItem.ShopName);
                            InnerDonePrePay(payRequest, pos, shop, prePay, account, owner, false);
                            oldDealLog.State = DealLogStates.Normal;
                            _accountDealDal.Save(oldDealLog);
                            _accountDealDal.Save(prePay);
                        }


                        var oldShopDeal = _accountDealDal.GetShopDealLogByAddin(oldDealLog.DealLogId);
                        oldShopDeal.State = DealLogStates.Normal;
                        _accountDealDal.Save(oldShopDeal);
                        //shopTo.Amount += oldDealLog.Amount;

                        break;
                    }
                default:
                    return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            }
            dealItem.State = DealLogStates.Normal_;
            _accountDealDal.Save(dealItem);
            var shopDealLog = _accountDealDal.GetShopDealLogByAddin(dealItem.DealLogId);
            if (shopDealLog != null)
            {
                shopDealLog.State = DealLogStates.Normal_;
                _accountDealDal.Save(shopDealLog);
            }
            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);

            var newDealItem = new DealLog(request.SerialNo, DealTypes.Invalid,
                                          -(dealAmount), -dealPoint,
                                          pos, shop, account, shopTo, dealItem.DealLogId);
            newDealItem.IsHidden = dealItem.IsHidden;
            if (shopDealLog != null)
            {
                shopDealLog = newDealItem.ToShopDealLog(shopTo);
                _accountDealDal.Save(shopDealLog);
                _accountDealDal.Save(shopTo);
            }

            _accountDealDal.Save(newDealItem);
            OrderComplete(account, owner, dealItem);
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, null, null);
        }

        public PosWithShop SignIn(string posName, string shopName, string userName, string password)
        {
            var posAndShop = _accountDealDal.GetPosAndShopByName(posName, shopName);
            if (posAndShop == null)
            {
                return null;
            }

            var result = new PosWithShop
                       {
                           DataKey = posAndShop.PosEndPoint.DataKey,
                           ShopName = posAndShop.Shop.Name,
                           Authenticated = true,
                       };
            if (string.IsNullOrEmpty(password))
            {
                _accountDealDal.UpdatePosOwnerId(posAndShop.PosEndPoint.PosEndPointId, 0);
                return result;
            }

            AdminUser adminUser = _accountDealDal.GetUserByName(userName);
            if (adminUser == null || !string.Equals(AdminUser.SaltAndHash(password, adminUser.PasswordSalt), adminUser.Password))
            {
                result.Authenticated = false;
                return result;
            }


            _accountDealDal.UpdatePosOwnerId(posAndShop.PosEndPoint.PosEndPointId, adminUser.UserId);
            CachePools.Remove(CacheKeys.PosKey);
            return result;
        }

        public AccountServiceResponse Query(string posName, string shopName, string accountName, string password, string userToken, string NewPassword = "", string OpenCode = "")
        {
            var posAndShop = _accountDealDal.GetPosAndShopByName(posName, shopName);
            var rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var pos = posAndShop.PosEndPoint;
            var shop = posAndShop.Shop;

            var account = _accountDealDal.GetAccountByName(accountName);
            var owner = _accountDealDal.GetUserById(account);
            rsp = AccountServiceValidator.ValidateAccount(account, shop, password, userToken);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            if (OpenCode == "000000000000000000000000000000PASSWORD")
            {
                string PasswordSalt = Guid.NewGuid().ToString("N").Substring(0, 8);
                string Password = User.SaltAndHash(NewPassword, PasswordSalt);
                account.PasswordSalt = PasswordSalt;
                account.Password = Password;
                _accountDealDal.Save(account);
            }

            return new AccountServiceResponse(ResponseCode.Success, null, _accountDealDal.GetShop(account.ShopId), account, owner, null, null);
        }

        private AccountServiceResponse InnerCancelDonePrePay(CancelPayRequest request, PrePay prePay, PosEndPoint pos, Shop shop, Account account, DealLog oldDealItem)
        {
            if (prePay == null || prePay.ShopId != pos.ShopId || prePay.PosId != pos.PosEndPointId || prePay.AccountId != account.AccountId)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);
            }
            if (prePay.ActualAmount != request.Amount)
            {
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            }

            account.PayCancel(oldDealItem.Amount, oldDealItem.Point);
            account.CancelDonePrePay(prePay);
            prePay.CancelDonePrePay(request.Amount);

            var dealItem = new DealLog(request.SerialNo);
            dealItem.DealType = DealTypes.CancelDonePrePay;
            dealItem.Amount = -oldDealItem.Amount;
            dealItem.SubmitTime = DateTime.Now;
            dealItem.Pos = pos;
            dealItem.SourceShop = shop;
            dealItem.Account = account;
            dealItem.Point = -oldDealItem.Point;

            oldDealItem.State = DealLogStates.Cancel;

            if (_accountDealDal.Save(account) == 0)
                return new AccountServiceResponse(ResponseCode.AccountConflict);
            _accountDealDal.Save(prePay);
            _accountDealDal.Save(oldDealItem);
            dealItem.Addin = oldDealItem.DealLogId;
            _accountDealDal.Save(dealItem);

            var oldShopDealLog = _accountDealDal.GetShopDealLogByAddin(oldDealItem.DealLogId);
            oldShopDealLog.State = DealLogStates.Cancel;
            _accountDealDal.Save(oldShopDealLog);

            var shopDealLog = dealItem.ToShopDealLog(shop);
            _accountDealDal.Save(shopDealLog);
            _accountDealDal.Save(shop);
            return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, null, null, null);
        }

        private AccountServiceResponse InnerDonePrePay(PayRequest request, PosEndPoint pos, Shop shop, PrePay prePay, Account account, AccountUser owner, bool addLog)
        {
            if (request.Amount > prePay.Amount)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            var cardType = _accountDealDal.GetAccounTypes().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId);
            AccountServiceResponse rsp = AccountServiceValidator.ValidateAccountType1(cardType);
            if (rsp.Code != ResponseCode.Success)
                return rsp;
            var Amountpoint = GetPoint(request, account, owner, cardType);
            var chargeAmount = GetCharge(request, account);

            account.Pay(Amountpoint.Amount, Amountpoint.Point);
            account.DonePrePay(prePay);
            prePay.DonePrePay(Amountpoint.Amount, request.SeriaNo);

            _accountDealDal.Save(prePay);
            if (addLog)
            {
                var dealItem = new DealLog(request.SeriaNo);
                dealItem.DealType = DealTypes.DonePrePay;
                dealItem.Amount = Amountpoint.Amount;
                dealItem.SubmitTime = DateTime.Now;
                dealItem.Pos = pos;
                dealItem.SourceShop = shop;
                dealItem.Account = account;
                dealItem.Point = Amountpoint.Point;
                dealItem.Shop = shop;
                dealItem.Addin = prePay.PrePayId;
                dealItem.AccountAmount = dealItem.AccountAmount;

                _accountDealDal.Save(dealItem);

                ShopDealLog shopDealLog = dealItem.ToShopDealLog(shop);
                _accountDealDal.Save(shopDealLog);
                _accountDealDal.Save(shop);
                if (chargeAmount != 0)
                {
                    if (!account.HasEnoughAmount(chargeAmount))
                        return new AccountServiceResponse(ResponseCode.InvalidateAmount);
                    account.PayCharge(chargeAmount);
                    dealItem = new DealLog(SerialNoHelper.Create(), DealTypes.DealCharge, chargeAmount, 0, null, null, account, null, dealItem.DealLogId);
                    _accountDealDal.Save(dealItem);


                    //var systemDealLog = new SystemDealLog(request.SeriaNo, null) { Addin = dealItem.DealLogId.ToString(), Amount = chargeAmount, DealWayId = 0, DealType = SystemDealLogTypes.AccountDealCharge };
                    //_accountDealDal.Save(systemDealLog);
                }
                _dealTracker.Notify(account, owner, dealItem); OrderComplete(account, owner, dealItem);
                _accountDealDal.Save(account);
                return new AccountServiceResponse(ResponseCode.Success, dealItem, _accountDealDal.GetShop(account.ShopId), account, owner, null, null);
            }
            return new AccountServiceResponse(ResponseCode.Success, null, _accountDealDal.GetShop(account.ShopId), account, owner, null, null);
        }

        public void InsertPosKey(PosKey item)
        {
            _posKeyService.Insert(item);
        }

        public void UpdatePosKey(PosKey item)
        {
            _posKeyService.Update(item);
        }

        public PosKey GetPosKey(string ShopName, string PosName)
        {
            return _posKeyService.GetPosKey(ShopName, PosName);
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="accountName">卡号</param>
        /// <param name="OldPwd">原密码</param>
        /// <param name="NewPwd">新密码</param>
        /// <param name="posName">终端号</param>
        /// <param name="shopName">商户号</param>
        /// <param name="UserToken">识别码</param>
        /// <returns></returns>
        public AccountServiceResponse UpdatePwd(string accountName, string OldPwd, string NewPwd, string posName, string shopName,string UserToken )
        {
            AccountServiceResponse rsp = null; //AccountServiceValidator.ValidateAmount(request.Amount);
            var posAndShop = new PosAndShop();
            posAndShop = _accountDealDal.GetPosAndShopByName(posName, shopName);
            rsp = AccountServiceValidator.ValidatePos(posAndShop);
            if (rsp.Code != ResponseCode.Success)
                return rsp;

            Account account = _accountDealDal.GetAccountByName(accountName);

            if (account == null || (account.State != AccountStates.Normal && account.State != AccountStates.Invalid))
            {
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            }
            //User owner = null;
            //if (account.OwnerId.HasValue)
            //    owner = MembershipService.GetUserById(account.OwnerId.Value);
           // AccountUser owner = _accountDealDal.GetUserById(account);
            rsp = AccountServiceValidator.ValidateAccount(account, posAndShop.Shop, OldPwd, UserToken);
            if (rsp.Code != ResponseCode.Success)
                 return rsp;
           string PasswordSalt = Guid.NewGuid().ToString("N").Substring(0, 8);
           string Password = User.SaltAndHash(NewPwd, PasswordSalt);
           account.PasswordSalt = PasswordSalt;
           account.Password = Password;
           _accountDealDal.Save(account);
           return new AccountServiceResponse(ResponseCode.Success);

            
        }
    }
}
