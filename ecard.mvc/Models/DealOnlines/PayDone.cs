using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealOnlines
{
    public class PayDone
    {
        public string ShopName { get; set; }
        public string AccountName { get; set; }
        public string AccountToken { get; set; }
        public string Password { get; set; }
        public decimal Amount { get; set; }
        public string SerialNo { get; set; }

        public string Error { get; set; }
        public bool IsRetry { get; set; }
        private string _sourceShopName;
        [Bounded(typeof(ResponseCode))]
        public int Code { get; set; }
        public void Ready()
        {
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            string posName = "";
            // debug
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
                var shop = ShopService.GetById(shopUser.ShopId);
                _sourceShopName = shop.Name;
                ShopName = shop.Name;
                posName = pos.Name;
            }


            // debug end 
            IPasswordService passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
            var password = passwordService.Decrypto(Password);

            Response = InnerCall(AccountToken, posName, password);

            this.Code = Response.Code;
            SerialNo = Response.SerialServerNo;
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
                return proxy.Proxy.Pay(new PayRequest(AccountName, password, posName, Amount, SerialNoHelper.Create(), token,
                                                  _sourceShopName, ShopName));
            }

        }
        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency]
        public Site HostSite { get; set; }
        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        public IPosEndPointService PosEndPointService { get; set; }

        public AccountServiceResponse Response { get; set; }
    }
}