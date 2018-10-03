using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealOnlines
{
    public class CancelPayDone
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string ServerSerialNo { get; set; }
        private string _oldSerialNo;
        private string _sourceShopName;
        public string Error { get; set; }
        public string ShopName { get; set; }
        public decimal Amount { get; set; }
        public bool IsRetry { get; set; }
        [Bounded(typeof(ResponseCode))]
        public int Code { get; set; }
        public void Ready()
        {

            var user = SecurityHelper.GetCurrentUser().CurrentUser;

            // debug
            var posName = "";
            AdminUser adminUser = user as AdminUser;
            if (adminUser != null)
            { 
                _sourceShopName = Constants.SystemShopName;
                var pos = this.PosEndPointService.GetByCurrentUserId(adminUser.UserId);
                if (pos == null)
                {
                    Error = "请先创建终端，再进行交易！";
                    return;
                }
                posName = pos.Name;
            }
            ShopUser shopUser = user as ShopUser;
            if (shopUser != null)
            {
                var pos = PosEndPointService.Query(new PosEndPointRequest() { ShopId = shopUser.ShopId, State = PosEndPointStates.Normal }).FirstOrDefault();
                if (pos == null)
                {
                    Error = "请先创建终端，再进行交易！";
                    return;
                }
                posName = pos.Name;
                var shop = ShopService.GetById(shopUser.ShopId);
                _sourceShopName = shop.Name;
                ShopName = shop.Name;
            }



            // debug end 
            IPasswordService passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
            var password = passwordService.Decrypto(Password);

            var dealLog = DealLogService.GetById(Convert.ToInt32(ServerSerialNo));
            ShopName = dealLog.ShopName;
            Amount = dealLog.Amount;
            AccountName = dealLog.AccountName;
            _oldSerialNo = dealLog.SerialNo;
            var account = AccountService.GetByName(AccountName);
            var token = account == null ? "" : account.AccountToken;
            Response = InnerCall(token,posName, password);

            this.Code = Response.Code;
            ServerSerialNo = Response.SerialServerNo;
            if (Response.Code == ResponseCode.InvalidatePassword)
            {
                IsRetry = true;
                Error = ModelHelper.GetBoundText(this, x => x.Code);
            }
            else if (Response.Code != ResponseCode.Success)
            {
                Error = ModelHelper.GetBoundText(this, x => x.Code);
            }
        }

        private AccountServiceResponse InnerCall(string token, string posName, string password)
        {

            using (var proxy = new ServiceProxy<IAccountDealService>())
            {
                return proxy.Proxy.CancelPay(new CancelPayRequest(AccountName, password, posName, Amount, SerialNoHelper.Create(), _oldSerialNo, token,
                                                       _sourceShopName));
            }
        }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency]
        public Site HostSite { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        public IPosEndPointService PosEndPointService { get; set; }
        [Dependency]
        public IShopService ShopService { get; set; }

        public AccountServiceResponse Response { get; set; }
    }
}