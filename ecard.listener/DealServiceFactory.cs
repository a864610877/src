using System.Collections.Generic;
using System.Linq;
using Ecard;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;

namespace PI8583
{
    class DealServiceFactory : IDealServiceFactory
    {
        public IAccountDealService CreateService(DatabaseInstance databaseInstance)
        {
            //List<Shop> shop = new List<Shop>
            //                      {
            //                          new Shop{Address = "address 1", Amount = 0, Bank = "½¨ÐÐ", ShopId = 1, Name = "111111111111111"}
            //                      };
            //List<PosEndPoint> pos = new List<PosEndPoint>
            //                            {
            //                                new PosEndPoint{ Name = "11111111", CurrentUserId = 12, DataKey = "3131313131313131", ShopId = 1, PosEndPointId = 1}
            //                            };
            //List<Account> accounts = new List<Account>
            //                             {
            //                                 new Account{ Name = "1234567890123456", Amount = 10000, AccountToken = "12345678", PasswordSalt = "123456782222", Password = User.SaltAndHash("123456", "123456782222"), AccountId = 1}
            //                             };
            //return new MockAccountDealService(shop, pos, accounts);
            CachedSqlAccountDealDal dal = new CachedSqlAccountDealDal(databaseInstance);
            SmsDealTracker dealTracker = new SmsDealTracker(dal, new SmsHelper(new SqlSmsService(databaseInstance)), dal.GetSite());
            SqlOrderService OrderService = new SqlOrderService(databaseInstance);
            IPosKeyService PosKeyService = new SqlPosKeyService(databaseInstance);
            IAccountDealService accountDealService = new AccountDealService(dal, dealTracker, OrderService,PosKeyService);
            return accountDealService;
        }

    }

    internal class MockAccountDealService : IAccountDealService
    {
        private readonly List<Shop> _shopList;
        private readonly List<PosEndPoint> _posList;
        private readonly List<Account> _accountList;

        public MockAccountDealService(List<Shop> shopList, List<PosEndPoint> posList, List<Account> accountList)
        {
            _shopList = shopList;
            _posList = posList;
            _accountList = accountList;
        }

        public PosWithShop SignIn(string posName, string shopName, string userName, string password)
        {
            var shop = _shopList.FirstOrDefault(x => x.Name == shopName);
            if (shop == null)
                return new PosWithShop { Authenticated = false };
            var pos = _posList.FirstOrDefault(x => x.Name == posName);
            if (pos == null)
                return new PosWithShop { Authenticated = false };
            if (!string.IsNullOrEmpty(password))
            {
                if (userName != "01" || password != "1234")
                    return new PosWithShop { Authenticated = false };
            }
            return new PosWithShop
                       {
                           Authenticated = true,
                           DataKey = pos.DataKey,
                           ShopName = shopName,
                       };
        }

        public AccountServiceResponse Pay(PayRequest request, bool IsWeb = false)
        {
            return DoCall(request);
        }

        private AccountServiceResponse DoCall(PayRequest request)
        {
            var account = _accountList.FirstOrDefault(x => x.Name == request.AccountName);
            if (account == null)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            var shop = _shopList.FirstOrDefault(x => x.Name == request.ShopName);
            if (shop == null)
                return new AccountServiceResponse(ResponseCode.InvalidateShop);
            var pos = _posList.FirstOrDefault(x => x.Name == request.PosName);
            if (pos == null)
                return new AccountServiceResponse(ResponseCode.InvalidatePos);

            var dealLog = new DealLog("123456789012", DealTypes.Deal, request.Amount, (int)(request.Amount / 10), pos, shop, account,
                                      shop, 0);
            return new AccountServiceResponse(ResponseCode.Success, dealLog, shop, account, null) { SerialServerNo = "123456789012" };
        }

        public AccountServiceResponse PrePay(PayRequest request)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse CancelPay(CancelPayRequest request, bool IsWeb = false)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse Roolback(PayRequest_ request, bool IsWeb = false)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse DonePrePay(PayRequest request)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse Query(string posName, string shopName, string accountName, string passwrod, string token, string NewPassword = "", string OpenCode = "")
        {
            return DoCall(new PayRequest(accountName, passwrod, posName, 0, "123456789012", token, shopName, shopName));
        }

        public AccountServiceResponse CancelPrePay(CancelPayRequest request)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse QueryShop(string posName, string shopName, string shopTo)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse Integral(PayRequest request, bool IsWeb = false)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse PayIntegral(PayRequest request)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse Recharge(PayRequest request, bool IsWeb = false)
        {
            throw new System.NotImplementedException();
        }


        public AccountServiceResponse QuitCard(PayRequest request)
        {
            throw new System.NotImplementedException();
        }

        public AccountServiceResponse UpdatePwd(string accountName, string OldPwd, string NewPwd, string posName, string shopName, string UserToken)
        {
            throw new System.NotImplementedException();
        }


        public AccountServiceResponse CancelDonePrePay(CancelPayRequest request)
        {
            throw new System.NotImplementedException();
        }


        public void InsertPosKey(PosKey item)
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePosKey(PosKey item)
        {
            throw new System.NotImplementedException();
        }

        public PosKey GetPosKey(string ShopName, string PosName)
        {
            throw new System.NotImplementedException();
        }
    }
}