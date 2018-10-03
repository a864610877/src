using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using System.Collections.Generic;
using System;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.Accounts
{
   
    public class SystemPayAccount
    {
        public string SeriaNo { get; set; }
        private string _accountName;

        private string _posName;

        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }
        public decimal Amount { get; set; }
        public string PosName
        {
            get { return _posName.TrimSafty(); }
            set { _posName = value; }
        }
        /// <summary>
        /// 支付方式（1表示现金支付，2表示会员卡支付（扣卡余额））
        /// </summary>
        public int PayWay { get; set; }
        public string AccountToken { get; set; }
        public string AccountPwd { get; set; }
        public void Ready()
        {
            var defaultPos = PosEndPointService.Query(new PosEndPointRequest() ).FirstOrDefault();
            if (defaultPos != null)
                PosName = defaultPos.Name;
        }
        [NoRender,Dependency]
        public IPosEndPointService PosEndPointService { get; set; }
        [NoRender,Dependency]
        public IAccountDealService AccountDealService { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        [NoRender, Dependency]
        public IAccountTypeService AccountTypeService { get; set; }
        [NoRender, Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [NoRender, Dependency]
        public IMembershipService _membershipService { get; set; }
        [NoRender, Dependency]
        public IAccountDealDal _accountDealDal { get; set; }
        [Dependency, NoRender]
        public Site HostSite { get; set; }
        [NoRender, Dependency]
        public LogHelper Logger { get; set; }
        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }
         [NoRender, Dependency]
        public IShopService _shopService { get; set; }
        internal AccountServiceResponse DoPay()
        {
            //AccountServiceResponse rsp = new AccountServiceResponse(0);
            var user = SecurityHelper.GetCurrentUser();
            if (user is ShopUserModel)
            {
                var shopUser = user.CurrentUser as ShopUser;
                var shop = _shopService.GetById(shopUser.ShopId);
                if (shop == null)
                    return new AccountServiceResponse(-1) {CodeText="无效商户" };
                Account account = null;
                var accountUser = (AccountUser)_membershipService.GetByMobile(AccountName.Trim());
                if (accountUser != null)
                {
                    account = AccountService.QueryByOwnerId(accountUser).FirstOrDefault();
                    //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
                }
                else
                {
                    account = AccountService.GetByName(AccountName.Trim());
                }
                string password1 = "";
                var passSvc = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                password1 = passSvc.Decrypto(AccountPwd);
                // var account = AccountService.GetByName(AccountName.Trim());
                if (account == null || account.State != AccountStates.Normal)
                    return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不好会员，请检查输入是否正确，会员卡状态是否正常！" };
                var sn = SeriaNo;//SerialNoHelper.Create();
                AccountServiceResponse rsp = new AccountServiceResponse(0);
                if (PayWay == 2)//会员卡交易扣卡余额
                {
                     rsp = AccountDealService.Pay(new PayRequest(account.Name, AccountPwd, "", Amount, SeriaNo, account.AccountToken, shop.Name, shop.Name),true);
                }
                else if (PayWay == 1)//现金交易，给现金积分
                {
                    rsp = AccountDealService.Integral(new PayRequest(account.Name, AccountPwd, "", Amount, SeriaNo, account.AccountToken, shop.Name, shop.Name),true);
                }
                else
                {
                    return new AccountServiceResponse(-1) { CodeText = "无效支付方式" };
                }

                if (rsp.Code == ResponseCode.Success)
                {
                    //做日志
                   // Logger.LogWithSerialNo(LogTypes.Deal, sn, account.AccountId, rsp.AccountType, account.Name);
                }
                return rsp;
            }
            else
            {
                return new AccountServiceResponse(-1) { CodeText = "无效商户" };
            }
        }
       
        public ShowPay ShowPay()
        {
            var item = new ShowPay();
            item.SeriaNo = SerialNoHelper.Create();
            //var user = (AccountUser)_membershipService.GetByMobile(AccountName.Trim());
            //if (user == null)
            //{
            //    item.Code = -1;
            //    item.CodeText = "手机号码不存在";
            //    return item;
            //}
            //var account = AccountService.QueryByOwnerId(user).FirstOrDefault();
            //var rsp = new AccountServiceResponse(-1, null, null);
            // var account = AccountService.GetByName(AccountName.Trim());
            if (string.IsNullOrWhiteSpace(AccountName))
            {
                item.Code = -1;
                item.CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！";
                return item;
            }
            Account account = null;
            var user = (AccountUser)_membershipService.GetByMobile(AccountName.Trim());
            if (user != null)
            {
                account = AccountService.QueryByOwnerId(user).FirstOrDefault();
                //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！" };
            }
            else
            {
                account = AccountService.GetByName(AccountName.Trim());
            }
            string password1 = "";
            var passSvc = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
            password1 = passSvc.Decrypto(AccountPwd.Trim());
            // var account = AccountService.GetByName(AccountName);
            if (account == null || account.State != AccountStates.Normal)
            {
                item.Code = -1;
                item.CodeText = "找不到会员，请检查输入是否正确，会员卡状态是否正常！";
                return item;
            }
            if (User.SaltAndHash(password1, account.PasswordSalt) != account.Password)
            {
                item.Code = -1;
                item.CodeText = "密码错误";
                return item;
            }

            //return new AccountServiceResponse(ResponseCode.NonFoundAccount) { CodeText = "找不好会员，请检查输入是否正确，会员卡状态是否正常！" };

            var accountType = AccountTypeService.GetById(account.AccountTypeId);
            if (accountType == null)
            {
                item.Code = -1;
                item.CodeText = "账户不存在";
                return item;
            }
            AccountUser owner = _accountDealDal.GetUserById(account);
            if (owner != null)
            {
                item.OwnerDisplayName = owner.DisplayName;
            }
            var PointAmount = GetPoint(Amount, account, owner,accountType);
            //switch (PayWay)
            //{ 
            //    case 1:
            //        item.PayWay = "现金支付";
            //        break;
            //    case 2:
            //        item.PayWay="会员卡支付";
            //        break;
            //}
            item.AccountName = account.Name;
            item.Amount = account.Amount;
            item.Point = account.Point;
            item.OldAmount = Amount;
            item.AccountTypeName = accountType.DisplayName;
            item.DealAmount = PointAmount.Amount;
            item.PayPoint = PointAmount.Point;
            item.Rebate = PointAmount.DiscountRate;
            item.ExpiredDate = account.ExpiredDate.ToString();
            //return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            //根据卡类型选择返回内容
            //switch (accountType.BaseType)
            //{
            //    case AccountBaseTypes.MeteringPay:
            //        item.AccountName = account.Name;
            //        item.Amount = account.Amount;
            //        item.Point = account.Point;
            //        var rebateAmount = Amount; //GetRebate(account, accountType);//折扣后金额
            //        var point = GetPoint(rebateAmount, account, accountType);//获得积分
            //        item.DealAmount = 0;
            //        item.PayPoint = 0;
            //        item.Rebate = 0;
            //        item.AccountTypeName = accountType.DisplayName;
            //        item.MeteringPayCount = account.MeteringPayCount;
            //        item.ExpiredDate = account.ExpiredDate.ToString();
            //        item.Code = 0;
            //        item.CodeText = "交易成功";
            //        break;//计次卡
            //    case AccountBaseTypes.Point:
            //        item.AccountName = account.Name;
            //        item.OldAmount = Amount;
            //        item.Amount = account.Amount;
            //        item.Point = account.Point;
            //        item.AccountTypeName = accountType.DisplayName;
            //        item.DealAmount = GetRebate(account, accountType);//折扣后金额
            //        item.PayPoint = GetPoint(item.DealAmount, account, accountType);//获得积分
            //        item.Rebate = accountType.Rebate;
            //        item.ExpiredDate = account.ExpiredDate.ToString();
            //        item.Code = 0;
            //        item.CodeText = "交易成功";
            //        break;//积分&折扣
            //    case AccountBaseTypes.AmountPay:
            //        item.AccountName = account.Name;
            //        item.OldAmount = Amount;
            //        item.Amount = account.Amount;
            //        item.Point = account.Point;
            //        item.AccountTypeName = accountType.DisplayName;
            //        item.DealAmount = GetRebate(account, accountType);//折扣后金额
            //        item.PayPoint = GetPoint(item.DealAmount, account, accountType);//获得积分
            //        item.Rebate = accountType.Rebate;
            //        item.ExpiredDate = account.ExpiredDate.ToString();
            //        item.Code = 0;
            //        item.CodeText = "交易成功";

            //        break;//储值卡
            //    default: return null;
            //        break;
            //}
            return item;
        }


        //private decimal GetRebate(Account account, AccountType accountType)
        //{
        //    decimal result = Amount;
        //    if (accountType.IsRebate)
        //        result = accountType.Rebate < 0 ? Amount : (Amount * accountType.Rebate) / 100;
        //    return result;
        //}

        /// <summary>
        /// 计算本次交易金额及积分。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        private PointAmount GetPoint(decimal Amount, Account account, AccountUser owner,AccountType accountType)
        {
            PointAmount result=new PointAmount();
            result.Point = 0;
            result.Amount = Amount;
            result.DiscountRate = 100;
            int point = 0;
            var accountLevel = _accountDealDal.GetAccountLevelPolicies().FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId && x.Level == account.AccountLevel);
            if (accountLevel == null) return result;

            decimal NewAmount = accountLevel.DiscountRate == null ? Amount : accountLevel.DiscountRate * Amount;
            if (accountType.IsPointable)
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
    }
}