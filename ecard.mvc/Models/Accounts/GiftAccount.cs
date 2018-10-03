using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Collections;

namespace Ecard.Mvc.Models.Accounts
{
    public class GiftAccount : EcardModelListRequest<PointGift>
    {
        public void Ready()
        {
        }
        private Account _innerObject;
        [Dependency, NoRender]
        public RandomCodeHelper RandomCodeHelper { get; set; }
        public GiftAccount()
        {
            _innerObject = new Account();
            this.OrderBy = "Point";
        }

        public int CurrentPoint { get; set; }
        public void Query()
        {
            if (string.IsNullOrWhiteSpace(this.AccountName))
            {
                var query = PointGiftService.Query().Where(x => x.State == PointGiftStates.Normal);

                
                List = BuildQuery(query).ToList(this, x => x);
                CurrentPoint = 0;
            }
            else
            {
                var account = AccountService.GetByName(AccountName);
                if (account != null)
                {
                    AccountUser owner = null;
                    if (account.OwnerId.HasValue)
                        owner = (AccountUser)MembershipService.GetUserById(account.OwnerId.Value);
                    CurrentPoint = account.Point;
                    var accountLevel = AccountLevelPolicyService.Query().FirstOrDefault(x => x.Level == account.AccountLevel && account.AccountTypeId == x.AccountTypeId);

                    var query = PointGiftService.Query().Where(x => x.IsFor(account, owner, accountLevel, DateTime.Now));

                    List = BuildQuery(query).ToList(this, x => x);
                }
                else
                {
                    ErrorMessage = Localize("nofoundAccount", string.Format("’ ªß {0} Œ¥’“µΩ", AccountName));
                    List = new PageOfList<PointGift>(this.OrderBy, this.PageSize);
                }
            }
        }

        private IEnumerable<PointGift> BuildQuery(IEnumerable<PointGift> query)
        {
            switch (this.OrderBy.ToLower())
            {
                case "point":
                    query = query.OrderBy(x => x.Point);
                    break;
                case "displayName":
                    query = query.OrderBy(x => x.DisplayName);
                    break;
            }
            query = query.OrderByDescending(x => x.Priority).GroupBy(x=>x.DisplayName + ":::" + x.Category).Select(x=>x.First());
            return query;
        }

        public string ErrorMessage { get; set; }
        public List<PointGift> Gifts { get; set; }
        [NoRender]
        public Account InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(Account item)
        {
            _innerObject = item;
        }


        [Hidden]
        public int AccountId { get; set; }

        [Dependency]
        [NoRender]
        public IPointGiftService PointGiftService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency]
        [NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }



        public string AccountName { get; set; }

        public void Save()
        {
            //var pointRebate = PointRebateService.GetById(this.PointRebate);
            //if (pointRebate == null) return;
            //var accountLevel = AccountLevelPolicyService.GetById(pointRebate.AccountLevelPolicyId);
            //if (accountLevel == null) return;
            //var account = AccountService.GetById(AccountId);
            //if (account == null) return;

            //if (account.AccountLevel != accountLevel.Level) return;

            //account.Amount += pointRebate.Amount;
            //account.Point -= pointRebate.Point;
            //AccountService.Update(account);
            //Logger.Log(LogTypes.AccountRebate, account.Name, pointRebate.DisplayName);
        }
    }
}