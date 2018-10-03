using System;
using Ecard.Infrastructure;
using Ecard.Models;
using Oxite.Model;

namespace Ecard.Services
{
    public class AccountServiceValidator
    {
        /// <summary>
        /// 检查消费金额，不能是负数
        /// </summary>
        /// <param name="dealAmount"></param>
        /// <returns></returns>
        public static AccountServiceResponse ValidateAmount(decimal dealAmount)
        {
            if (dealAmount <= 0)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            return new AccountServiceResponse(ResponseCode.Success);
        }
        //777777
        public static AccountServiceResponse ValidateAccountType(AccountType account)
        {
            if (!account.IsRecharging)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            return new AccountServiceResponse(ResponseCode.Success);
        }
        /// <summary>
        /// 判断是否能积分
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static AccountServiceResponse ValidateAccountType2(AccountType account)
        {
            if (!account.IsPointable)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount) {CodeText="此卡不支持积分" };
            return new AccountServiceResponse(ResponseCode.Success);
        }

        public static AccountServiceResponse ValidatePoint(Account account, int point)
        {
            if (point <= 0)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            if (account.Point < point)
                return new AccountServiceResponse(ResponseCode.InvalidateAmount);
            return new AccountServiceResponse(ResponseCode.Success);
        }

        public static AccountServiceResponse ValidateDealItem(DealLog dealLog, string shopName, string posName, decimal dealAmount, bool isForce)
        {
            switch (dealLog.DealType)
            {
                case DealTypes.PayIntegral:
                    {
                        if (dealLog == null || dealLog.LiquidateDealLogId != 0 || dealLog.SourceShopName != shopName || dealLog.SourcePosName != posName || dealLog.State != DealLogStates.Normal || Math.Abs(dealLog.Point) != Math.Round(dealAmount, 0, MidpointRounding.AwayFromZero))
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        break;
                    }
                   // case DealTypes.
                default:
                    {
                        if (dealLog == null
                           || dealLog.LiquidateDealLogId != 0
                           || dealLog.SourceShopName != shopName
                           || dealLog.SourcePosName != posName
                           || dealLog.State != DealLogStates.Normal
                           )
                        {
                            return new AccountServiceResponse(ResponseCode.NonFoundDeal);
                        }
                        break;
                    }
            }
           
            return new AccountServiceResponse(ResponseCode.Success);
        }

        public static AccountServiceResponse ValidatePrePayForDeal_PrePay(PrePay prePay)
        {
            if (prePay.State != PrePayStates.Processing)
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);

            return new AccountServiceResponse(ResponseCode.Success);
        }
        public static AccountServiceResponse ValidatePrePayForDeal_DonePrePay(PrePay prePay)
        {
            if (prePay.State != PrePayStates.Complted)
                return new AccountServiceResponse(ResponseCode.NonFoundDeal);

            return new AccountServiceResponse(ResponseCode.Success);
        }
        public static AccountServiceResponse ValidateAccount(Account account)
        {
            if (account == null || account.State != AccountStates.Normal)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            }
            if (account.IsValidityDate())
            {
                return new AccountServiceResponse(ResponseCode.ValidityDate);
            }
            return new AccountServiceResponse(ResponseCode.Success);
        }
        /// <summary>
        /// 检验会员卡
        /// </summary>
        /// <param name="account"></param>
        /// <param name="shop"></param>
        /// <param name="password"></param>
        /// <param name="userToken"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        public static AccountServiceResponse ValidateAccount(Account account, Shop shop, string password, string userToken, bool isForce = false)
        {
            if (account == null || account.State != AccountStates.Normal)
            {
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);
            }
            if (account.ShopId != 0 && shop.ShopId != account.ShopId)
                return new AccountServiceResponse(ResponseCode.NonFoundAccount);

            if (userToken != null && account.AccountToken != userToken)
            {
                return new AccountServiceResponse(ResponseCode.Cracker);
            }
            if (!isForce)
                if (account.Password != User.SaltAndHash(password, account.PasswordSalt))
                {
                    return new AccountServiceResponse(ResponseCode.InvalidatePassword);
                }
            if (account.IsValidityDate())
            {
                return new AccountServiceResponse(ResponseCode.ValidityDate);
            }

            return new AccountServiceResponse(ResponseCode.Success);
        }

        public static AccountServiceResponse ValidatePos(PosAndShop pos)
        {
            if (pos == null || pos.PosEndPoint.State != PosEndPointStates.Normal)
                return new AccountServiceResponse(ResponseCode.InvalidatePos);
            if (pos.Shop.State != ShopStates.Normal)
                return new AccountServiceResponse(ResponseCode.InvalidateShop);
            return new AccountServiceResponse(ResponseCode.Success);
        }
        public static AccountServiceResponse ValidateShop(Shop shop)
        {
            if (shop == null || shop.State != ShopStates.Normal)
                return new AccountServiceResponse(ResponseCode.InvalidateShop);
            return new AccountServiceResponse(ResponseCode.Success);
        }

        internal static AccountServiceResponse ValidateAccountType1(AccountType cardType)
        {
            if (cardType!=null)
                return new AccountServiceResponse(ResponseCode.Success);
            return new AccountServiceResponse(ResponseCode.NonFoundAccount) {CodeText="此卡不能充值" };

        }
    }
}