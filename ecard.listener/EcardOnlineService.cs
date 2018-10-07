using System;
using System.Collections.Generic;
using System.Linq;
using Ecard;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit.Data;
using log4net;

namespace PI8583
{
    public class EcardOnlineService : IAccountDealService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EcardOnlineService));

        public AccountServiceResponse InnerExecute(Func<IAccountDealService, AccountServiceResponse> func)
        {
            try
            {
                var database = new Database("ecard");
                using (var instance = new DatabaseInstance(database))
                {
                    instance.BeginTransaction();
                    IAccountDealService accountDealService = GetAccountDealService(instance);

                    AccountServiceResponse rsp = func(accountDealService);
                    if (rsp.Code == ResponseCode.Success)
                        instance.Commit();
                    return rsp;
                }
            }
            catch (Exception ex)
            {
                Log.Error("error", ex);
                return new AccountServiceResponse(ResponseCode.SystemError) { CodeText = ex.Message };
            }
        }

        private static IAccountDealService GetAccountDealService(DatabaseInstance instance)
        {
            var dal = new CachedSqlAccountDealDal(instance);
            var dealTracker = new SmsDealTracker(dal, new SmsHelper(new SqlSmsService(instance)), dal.GetSite());
            SqlOrder1Service OrderService = new SqlOrder1Service(instance);
            IPosKeyService PosKeyService = new SqlPosKeyService(instance);
            IAccountDealService accountDealService = new AccountDealService(dal, dealTracker, OrderService, PosKeyService);
            return accountDealService;
        }

        private static string GetTAccountToken(string accountName, string accountToken, DatabaseInstance instance)
        {
            if (string.Equals(accountToken, "pass", StringComparison.OrdinalIgnoreCase))
            {
                List<Account> accounts = instance.Query<Account>("select * from accounts where state = 1 and Name = @name", new { name = accountName }).ToList();
                if (accounts.Any())
                    accountToken = accounts.First().AccountToken;
            }
            return accountToken;
        }

        public AccountServiceResponse Pay(PayRequest request,bool IsWeb=false)
        {
            return InnerExecute((x) => x.Pay(request));
        }

        public AccountServiceResponse PrePay(PayRequest request)
        {
            return InnerExecute((x) => x.PrePay(request));
        }

        public AccountServiceResponse CancelPay(CancelPayRequest request, bool IsWeb = false)
        {
            return InnerExecute((x) => x.CancelPay(request));
        }

        public AccountServiceResponse Roolback(PayRequest_ request, bool IsWeb = false)
        {
            return InnerExecute((x) => x.Roolback(request));
        }

        public AccountServiceResponse DonePrePay(PayRequest request)
        {
            return InnerExecute((x) => x.DonePrePay(request));
        }

        public PosWithShop SignIn(string posName, string shopName, string userName, string password)
        {
            throw new System.Exception();
        }

        public AccountServiceResponse Query(string posName, string shopName, string accountName, string passwrod, string token, string NewPassword = "", string OpenCode = "")
        {
            return InnerExecute((x) => x.Query(posName, shopName, accountName, passwrod, token));
        }

        public AccountServiceResponse CancelPrePay(CancelPayRequest request)
        {
            return InnerExecute((x) => x.CancelPrePay(request));
        }

        public AccountServiceResponse QueryShop(string posName, string shopName, string shopTo)
        {
            return InnerExecute((x) => x.QueryShop(posName, shopName, shopTo));
        }

        public AccountServiceResponse Integral(PayRequest request, bool IsWeb = false)
        {
            return InnerExecute(x => x.Integral(request));
        }

        public AccountServiceResponse PayIntegral(PayRequest request)
        {
            return InnerExecute(x => x.PayIntegral(request));
        }

        public AccountServiceResponse Recharge(PayRequest request, bool IsWeb = false)
        {
            return InnerExecute(x => x.Recharge(request));
        }


        public AccountServiceResponse QuitCard(PayRequest request)
        {
            throw new NotImplementedException();
        }

        public AccountServiceResponse UpdatePwd(string accountName, string OldPwd, string NewPwd, string posName, string shopName, string UserToken)
        {
            throw new NotImplementedException();
        }


        public AccountServiceResponse CancelDonePrePay(CancelPayRequest request)
        {
            throw new NotImplementedException();
        }


        public void InsertPosKey(PosKey item)
        {
            throw new NotImplementedException();
        }

        public void UpdatePosKey(PosKey item)
        {
            throw new NotImplementedException();
        }

        public PosKey GetPosKey(string ShopName, string PosName)
        {
            throw new NotImplementedException();
        }
    }
}