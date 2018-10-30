using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Ecard.Mvc.Models.Distributors;
using Ecard.Mvc.ActionFilters;

namespace Ecard.Mvc.Models.Accounts
{
    public class InitAccount : ViewModelBase
    {
        private Bounded _accountTypeBounded;

        public InitAccount()
        {
            Start = 1;
            End = 1;
            Interval = 1;
            IsEmptyPassword = false;
        }
        public void LogSetdistributorError()
        {
            AddError(1,"选择的经销商无效");
        }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        //-----
        [Dependency]
        [NoRender]
        public IMembershipService MembershipService { get; set; }
        //--
        [Dependency, NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public IDistributorService DistributorService { get; set; }
        [NoRender]
        public bool IsEmptyPassword { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Interval { get; set; }
        public string Format { get; set; }

        public string Password { get; set; }
        //---
        private Bounded _distributor;
        [NoRender]
        public Bounded Distributor
        {
            get
            {
                if (_distributor == null)
                {
                    _distributor = Bounded.CreateEmpty("DistributorId", Globals.All);
                }
                return _distributor;
            }
            set { _distributor = value; }
        }
        //--
        private Bounded _shopBounded;
        //这个不需要了可以加上[NoRender]
        
        public Bounded Shop
        {
            get
            {
                if (_shopBounded == null)
                {
                    _shopBounded = Bounded.CreateEmpty("ShopId", Globals.All);
                }
                return _shopBounded;
            }
            set { _shopBounded = value; }
        }
        public string AccountToken { get; set; }

        [Dependency]
        [NoRender]
        public Site Site { get; set; }

        public Bounded AccountType
        {
            get
            {
                if (_accountTypeBounded == null)
                {
                    _accountTypeBounded = Bounded.CreateEmpty("AccountTypeId", 0);
                }
                return _accountTypeBounded;
            }
            set { _accountTypeBounded = value; }
        }

        public IMessageProvider CreateAccounts()
        {
            var serialNo = SerialNoHelper.Create();
            List<Account> accounts = GetAccounts().ToList();
            int total = accounts.Count;
            if (accounts.Any())
            {
                int available = accounts.Count;
                if (accounts.Any())
                {
                    int count = 50;
                    TransactionHelper.BeginTransaction();
                    for (int i = 0; i < accounts.Count; )
                    {
                        var bufAccounts = accounts.Skip(i).Take(count).ToList();
                        var existingAccounts = AccountService.QueryByNames(bufAccounts.Select(x => x.Name).ToArray()).ToList();


                        foreach (var account in bufAccounts)
                        {
                            if (existingAccounts.Any(x => string.Equals(x.Name, account.Name, StringComparison.OrdinalIgnoreCase)))
                                available--;
                            else
                                AccountService.Create(account);
                        }
                        i += count;
                    }
                    Logger.LogWithSerialNo(LogTypes.AccountInit, serialNo, 0, available, total - available, accounts.First().Name, accounts.Last().Name);
                    AddMessage("success", available, total - available, accounts.First().Name, accounts.Last().Name);
                    TransactionHelper.Commit();
                    return this;
                }
                else
                {
                    AddError(LogTypes.AccountInit, "dumplicateAccounts", total);
                    return this;
                }
            }
            AddError(LogTypes.AccountInit, "noaccount");
            return this;
        }

        private IEnumerable<Account> GetAccounts()
        {
            int shopId = this.Shop;
            AccountType accountType = AccountTypeService.GetById(AccountType);
            if (accountType == null)
                yield break;

            for (int i = Start; i <= End; i += Interval)
            {
                string password = IsEmptyPassword ? "" : Password; //RandomHelper.GenerateNumber(6);
                string salt = RandomHelper.GenerateNumber(8);

                var account = new Account
                                  {
                                      Name = string.Format(Format, i),
                                      Amount = accountType.Amount,
                                      Point=accountType.Point,
                                      State = AccountStates.Initialized,
                                      ExpiredMonths = accountType.ExpiredMonths,
                                      InitPassword = password,
                                      PasswordSalt = salt,
                                      AccountTypeId = accountType.AccountTypeId,
                                      DepositAmount = accountType.DepositAmount,
                                      Frequency=accountType.Frequency,
                                  };
                if (shopId != Globals.All)
                    account.ShopId = shopId;
                account.DistributorId = Distributor;
                account.AccountToken = AccountToken ?? RandomHelper.GenerateNumber(8);
                account.Password = User.SaltAndHash(account.InitPassword, account.PasswordSalt);
                account.IsMessageOfDeal = accountType.IsMessageOfDeal;
                yield return account;
            }
        }

        public void Ready()
        {
            Password = "000000";
            Format = string.Format("{{0:{0}}}", "".PadLeft(Site.AccountNameLength, '0'));
            IEnumerable<IdNamePair> query = AccountTypeService.Query(new AccountTypeRequest { State = States.Normal })
                .ToList().Select(x => new IdNamePair { Key = x.AccountTypeId, Name = x.DisplayName });
            AccountType.Bind(query);
            AccountType.Callback = "/AccountType/GetAccountTypeById/{0}";
            AccountToken = Site.AccountToken;
            var q = DistributorService.Query();
            UserRequest user = new UserRequest();
            var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();

            var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
            int distributorId = 0;
            var currentUser = this.SecurityHelper.GetCurrentUser();
            if (currentUser is AdminUserModel)
            {
                var qq = (from x in list where x.InnerObject.ParentId == distributorId select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
                qq.Insert(0, new IdNamePair { Key = Ecard.Models.Distributor.Default.DistributorId, Name = Ecard.Models.Distributor.Default.FormatedName });
                Distributor.Bind(qq, false);
            }
            else if (currentUser is DistributorUserModel)
            {
                distributorId = ((DistributorUserModel)currentUser).DistributorId;
                var totalId = DistributorService.Query().Select(x => x.DistributorId).ToList();
                var ids = GetChildrenDistributorId(distributorId, totalId);
                //ids.Add(distributorId);
                var qq = (from x in list where ids.Contains(x.DistributorId) select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
                qq.Insert(0, new IdNamePair { Key = Ecard.Models.Distributor.Default.DistributorId, Name = Ecard.Models.Distributor.Default.FormatedName });
                Distributor.Bind(qq,false);
            }
            //shop不要了这里也 可以不要了
            var qshop = (from x in ShopService.Query(new ShopRequest() { State = ShopStates.Normal, IsBuildIn = false })
                     select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            //qshop.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            Shop.Bind(qshop, true, "全部");
            //var q = DistributorService.Query();
            //UserRequest user=new UserRequest();
            //user.Discriminator="DistributorUser";
            //var users = MembershipService.QueryUsers<DistributorUser>(new UserRequest()).ToList();

            // var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner=users.First(u=>u.UserId==x.UserId)}).ToList();
            //var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
            //qq.Insert(0, new IdNamePair { Key = Ecard.Models.Distributor.Default.DistributorId, Name = Ecard.Models.Distributor.Default.FormatedName });
            // Distributor.Bind(qq, true);
            //var q = (from x in DistributorService.Query()users, x => new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId
            //         select new IdNamePair { Key = x.DistributorId, Name = x. }).ToList();
            //q.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            //Shop.Bind(q, true, "默认");Distributors.ListDistributor
        }
        /// <summary>
        /// 取得所有的下属经销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> GetChildrenDistributorId(int id, List<int> list)
        {
            List<int> ids = new List<int>();
            foreach (var item in list)
            {
                int TopLevelDistributorId = GetTopLevelDistributorId(item);
                if (id == TopLevelDistributorId)
                    ids.Add(item);
            }
            return ids;
        }
        /// <summary>
        /// 根据输入的经销商Id取得一级经销商的Id
        /// </summary>
        /// <param name="distributorId"></param>
        /// <returns></returns>
        public int GetTopLevelDistributorId(int distributorId)
        {
            var parent = DistributorService.GetById(distributorId);
            if (parent != null)
                if (parent.ParentId == 0)
                    return parent.DistributorId;
                else return GetTopLevelDistributorId(parent.ParentId);
            else
                return 0;

        }
    }
}