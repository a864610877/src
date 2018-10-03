using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Accounts
{
    public class DoGiftAccount : ViewModelBase
    {
        public int GiftId { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }

        [Dependency, NoRender]
        public IPointGiftService PointGiftService { get; set; }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public RandomCodeHelper RandomCodeHelper { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        [Dependency, NoRender]
        public Site CurrentSite { get; set; }

        public object Save()
        {
            var serialNo = SerialNoHelper.Create();
            try
            {
                var account = AccountService.GetByName(AccountName);
                if (account == null || account.State != AccountStates.Normal)
                    return new DataAjaxResult(string.Format(Localize("NoAccount", "会员卡号 {0} 未找到"), AccountName));
                var passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                var password = passwordService.Decrypto(Password);
                var accountLevel = AccountLevelPolicyService.Query().FirstOrDefault(x => x.Level == account.AccountLevel && account.AccountTypeId == x.AccountTypeId);
              
                var owner = (AccountUser)(account.OwnerId.HasValue ? MembershipService.GetUserById(account.OwnerId.Value) : null);
                var gift = PointGiftService.Query().Where(x => x.IsFor(account, owner, accountLevel, DateTime.Now)).FirstOrDefault(x => x.PointGiftId == GiftId);
                if (gift == null)
                    return new DataAjaxResult(Localize("NoGift", "礼品未找到"));

                if (gift.Point > account.Point)
                    return new DataAjaxResult(Localize("NoEnoughPoint", "积分不足"));
                if (User.SaltAndHash(password, account.PasswordSalt) != account.Password)
                    return new DataAjaxResult(Localize("error.Password", "密码错误"));

                account.Point -= gift.Point;
                TransactionHelper.BeginTransaction();
                AccountService.Update(account);
                DealLogService.Create(new DealLog(serialNo, DealTypes.Gift, 0, -gift.Point, null, null, account, null, gift.PointGiftId));

                Logger.LogWithSerialNo(LogTypes.AccountDoGift, serialNo, account.AccountId, account.Name, gift.DisplayName);
                return TransactionHelper.CommitAndReturn(new DataAjaxResult());
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.AccountDoGift, ex);

                return new DataAjaxResult(Localize("Error", "兑换失败"));
            }
        }
    }
}