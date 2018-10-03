using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Commands;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Ecard.Mvc.Models.Distributors;
using Ecard.Mvc.ActionFilters;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Accounts
{
    public class ListAccounts : EcardModelListRequest<ListAccount>
    {
        private Bounded _mobileStateBounded;
        private Bounded _stateBounded;
        private Bounded _shopBounded;
        private Bounded _distributorBounded;
        [NoRender]
        public Bounded Distributor
        {
            get
            {
                if (_distributorBounded == null)
                {
                    _distributorBounded = Bounded.CreateEmpty("DistributorId", Globals.All);
                }
                return _distributorBounded;
            }
            set { _distributorBounded = value; }
        }
        [NoRender]
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
        public ListAccounts()
        {
            OrderBy = "Name";
        }

        [NoRender]
        public string MessageTemplate { get; set; }

        [NoRender]
        public bool? SendToAll { get; set; }

        [StringLength(16, MinimumLength = 3)]
        public string Name { get; set; }

        public string Content { get; set; }

        [Bounded(typeof(MobileStates))]
        [NoRender]
        public int IsMobileAvaliable { get; set; }
        [NoRender]
        public Bounded MobileState
        {
            get
            {
                if (_mobileStateBounded == null)
                {
                    _mobileStateBounded = Bounded.Create<ListAccounts>("IsMobileAvaliable", MobileStates.None);
                }
                return _mobileStateBounded;
            }
            set { _mobileStateBounded = value; }
        }

        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.Create<Account>("State", States.All);
                }
                return _stateBounded;
            }
            set { _stateBounded = value; }
        }

        [NoRender, Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IDistributorService DistributorService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [NoRender, Dependency]
        public ISystemDealLogService SystemDealLogService { get; set; }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [NoRender, Dependency]
        public IPrintTicketService PrintTicketService { get; set; }

        [NoRender, Dependency]
        public IDealLogService DealLogService { get; set; }

        [NoRender, Dependency]
        public IAccountTypeService AccountTypeService { get; set; }

        [NoRender, Dependency]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        [NoRender, Dependency]
        public ICashDealLogService CashDealLogService { get; set; }

        [NoRender, Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }

        [NoRender, Dependency]
        public IDealWayService DealWayService { get; set; }

        [NoRender, Dependency]
        public Site HostSite { get; set; }

        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }

        public void Ready()
        {
            //var q = DistributorService.Query();
            //UserRequest user = new UserRequest();
            //var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();

            //var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
            //int distributorId = 0;
            //var currentUser = this.SecurityHelper.GetCurrentUser();
            //if (currentUser is AdminUserModel)
            //{
            //    var qq = (from x in list where x.InnerObject.ParentId == distributorId select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
            //    qq.Insert(0, new IdNamePair { Key = Ecard.Models.Distributor.Default.DistributorId, Name = Ecard.Models.Distributor.Default.FormatedName });
            //    Distributor.Bind(qq, true);
            //}
            //else if (currentUser is DistributorUserModel)
            //{
            //    distributorId = ((DistributorUserModel)currentUser).DistributorId;
            //    var totalId = DistributorService.Query().Select(x => x.DistributorId).ToList();
            //    var ids = GetChildrenDistributorId(distributorId, totalId);
            //    ids.Add(distributorId);
            //    var qq = (from x in list where ids.Contains(x.DistributorId) select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();
            //    //qq.Insert(0, new IdNamePair { Key = Ecard.Models.Distributor.Default.DistributorId, Name = Ecard.Models.Distributor.Default.FormatedName });
            //    Distributor.Bind(qq, true);
            //}
            //var query = (from x in ShopService.Query(new ShopRequest() { IsBuildIn = false })
            //             select new IdNamePair { Key = x.ShopId, Name = x.FormatedName }).ToList();
            //query.Insert(0, new IdNamePair { Key = Ecard.Models.Shop.Default.ShopId, Name = Ecard.Models.Shop.Default.FormatedName });
            //this.Shop.Bind(query, true);
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Init", null);
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Creates", null);
            yield return new ActionMethodDescriptor("Approves", null);
            yield return new ActionMethodDescriptor("Opens", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" });
            //yield return new ActionMethodDescriptor("SendSmsMessage", null);
            //yield return new ActionMethodDescriptor("SetDistributors", null);
            yield return new ActionMethodDescriptor("DeriveTXT", null);

        }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListAccount item)
        {
            //if (item.InnerObject.State < AccountStates.Saled && item.InnerObject.State > 10)
            //{
            //    yield return new ActionMethodDescriptor("SetDistributor", null, new { id = item.InnerObject.AccountId });
            //}
            if (item.InnerObject.State == States.Normal)
            {
                //yield return new ActionMethodDescriptor("Rebate", null, new { id = item.AccountId });
                yield return new ActionMethodDescriptor("Owner", null, new { id = item.AccountId });
                //yield return new ActionMethodDescriptor("GotoChangeLimitAmount", null, new { id = item.AccountId });
            }

            yield return new ActionMethodDescriptor("Delete", null, new { id = item.InnerObject.AccountId });
        }

        public ResultMsg Approve(int id)
        {
              ResultMsg msg=new ResultMsg();
              try
              {
                  Account item = AccountService.GetById(id);
                  if (item != null && item.State == AccountStates.Created)
                  {
                      item.State = AccountStates.Ready;
                      AccountService.Update(item);

                      Logger.LogWithSerialNo(LogTypes.AccountApprove, SerialNoHelper.Create(), id, item.Name);
                      msg.Code = 1;
                      msg.CodeText = "审核会员 " + item.Name + " 成功";
                  }
                  else
                  {
                      msg.CodeText = "不好意思,没有找到会员";
                  }
                  return msg;
              }
              catch (Exception ex)
              {
                  msg.CodeText = "不好意思,系统异常";
                  Logger.Error("审核会员", ex);
                  return msg;
              } 
        }
        public ResultMsg SetDistributorId(int id, int distributor)
        {
               ResultMsg msg=new ResultMsg();
               try
               {
                   Account item = AccountService.GetById(id);
                   int[] status = { AccountStates.Created, AccountStates.Initialized, AccountStates.Ready };
                   var dis = DistributorService.GetById(distributor);
                   if (item != null && status.Contains(item.State) && dis != null)
                   {
                       item.DistributorId = distributor;
                       AccountService.Update(item);

                       Logger.LogWithSerialNo(LogTypes.AdminUserEdit, SerialNoHelper.Create(), id, item.Name);
                       msg.Code = 1;
                       msg.CodeText = item.Name + " 设置经销商成功成功";
                   }
                   else
                   {
                       msg.CodeText = "所选择的经销商无效或者当前卡状态不能转换经销商";
                   }
                   return msg;
               }
               catch (Exception ex)
               {
                   msg.CodeText = "不好意思,系统异常";
                   Logger.Error("设置经销商", ex);
                   return msg;
               } 
        }
        public ResultMsg Delete(int id)
        {
              ResultMsg msg=new ResultMsg();
              try
              {
                  Account item = AccountService.GetById(id);
                  if (item != null && item.State == AccountStates.Initialized)
                  {
                      AccountService.Delete(item);

                      Logger.LogWithSerialNo(LogTypes.AccountDelete, SerialNoHelper.Create(), id, item.Name);
                      msg.Code = 1;
                      msg.CodeText = "删除会员 " + item.Name + " 成功";

                  }
                  else
                  {
                      msg.CodeText = "删除失败,只有初始化状态的卡片可以删除";
                  }
                  return msg;
              }
              catch (Exception ex)
              {
                  msg.CodeText = "不好意思,系统异常";
                  Logger.Error("删除会员", ex);
                  return msg;
              }
        }

        public ResultMsg Suspend(int id)
        {
             ResultMsg msg=new ResultMsg();
             try
             {
                 Account item = AccountService.GetById(id);
                 if (item != null && item.State == States.Normal)
                 {
                     item.State = States.Invalid;
                     AccountService.Update(item);


                     Logger.LogWithSerialNo(LogTypes.AccountSuspend, SerialNoHelper.Create(), id, item.Name);
                     if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountSuspend))
                     {
                         var owner = item.OwnerId.HasValue ? MembershipService.GetUserById(item.OwnerId.Value) : null;
                         if (owner != null && owner.IsMobileAvailable)
                         {
                             var accountType = AccountTypeService.GetById(item.AccountTypeId);
                             if (accountType != null && accountType.IsSmsSuspend)
                             {
                                 var msgs = MessageFormator.Format(HostSite.MessageTemplateOfAccountSuspend, owner);
                                 msgs = MessageFormator.Format(msgs, item);
                                 SmsHelper.Send(owner.Mobile, msgs);
                             }
                         }
                     }
                     msg.Code = 1;
                     msg.CodeText = "停用会员 " + item.Name + " 成功";
                    // AddMessage("suspend.success", item.Name);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到会员";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("停用会员", ex);
                 return msg;
             }
        }
        /// <summary>
        /// 更改经销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="distributorId"></param>
        public void SetDistributor(int id, int distributorId)
        {
            this.TransactionHelper.BeginTransaction();
            Account item = AccountService.GetById(id);
            if (item != null && item.State == States.Normal)
            {
                var distributor = DistributorService.GetById(distributorId);
                if (distributor != null)
                {
                    item.DistributorId = distributorId;
                    AccountService.Update(item);


                    Logger.LogWithSerialNo(LogTypes.AccountSuspend, SerialNoHelper.Create(), id, item.Name);
                    AddMessage("Update.success", item.Name);
                }
                else
                    AddError("经销商不存在？", distributorId);
            }
            this.TransactionHelper.Commit();
        }

        public ResultMsg Resume(int id)
        {
             ResultMsg msg = new ResultMsg();
             try
             {
                 Account item = AccountService.GetById(id);
                 if (item != null && item.State == States.Invalid)
                 {
                     item.State = States.Normal;
                     AccountService.Update(item);
                     if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfAccountResume))
                     {
                         var owner = item.OwnerId.HasValue ? MembershipService.GetUserById(item.OwnerId.Value) : null;
                         if (owner != null && owner.IsMobileAvailable)
                         {
                             var accountType = AccountTypeService.GetById(item.AccountTypeId);
                             if (accountType != null && accountType.IsSmsResume)
                             {
                                 var msgs = MessageFormator.Format(HostSite.MessageTemplateOfAccountResume, owner);
                                 msgs = MessageFormator.Format(msgs, item);
                                 SmsHelper.Send(owner.Mobile, msgs);
                             }
                         }
                     }
                     Logger.LogWithSerialNo(LogTypes.AccountResume, SerialNoHelper.Create(), id, item.Name);
                     msg.Code = 1;
                     msg.CodeText = "启用会员 " + item.Name + " 成功";
                     //AddMessage("resume.success", item.Name);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到会员";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("启用会员", ex);
                 return msg;
             }
        }

        public ResultMsg Create(int id)
        {
             ResultMsg msg=new ResultMsg();
             try
             {
                 Account item = AccountService.GetById(id);

                 if (item == null || item.State != AccountStates.Initialized)
                 {
                     //this.AddError("accountNameNotExist");
                     msg.CodeText = "不好意思,没有找到会员";
                     return msg;
                 }
                 item.State = AccountStates.Created;
                 AccountService.Update(item);
                 Logger.LogWithSerialNo(LogTypes.AccountCreate, SerialNoHelper.Create(), id, item.Name);
                 msg.Code = 1;
                 msg.CodeText = "会员建卡 " + item.Name + " 成功";

                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("批量建卡", ex);
                 return msg;
             }
        }

        public void LogOpenSuccess(int count)
        {
            AddMessage("open.success", count);
        }

        public void LogApproveSuccess(int count)
        {
            AddMessage("approve.success", count);
        }
        public void LogSetdistributorSuccess(int count)
        {
            AddMessage("Setdistributor.success", count);
        }
        public void LogSetdistributorError()
        {
            AddError("选择的经销商无效");
        }
        public void LogDeleteSuccess(int count)
        {
            AddMessage("delete.success", count);
        }

        public void LogCreateSuccess(int count)
        {
            AddMessage("creates.success", count);
        }

        public ResultMsg Open(int id, DealWay dealWay)
        {
            ResultMsg msgResult = new ResultMsg();
            try
            {
                User operatorUser = SecurityHelper.GetCurrentUser().CurrentUser;
                using (Transaction tran = TransactionHelper.BeginTransaction())
                {
                    string serialNo = SerialNoHelper.Create();
                    Account account = AccountService.GetById(id);
                    if (account.State != AccountStates.Ready)
                        msgResult.CodeText = "不好意思,会员状态不对";
                    var command = new OpenAccountCommand(serialNo, account.Name, null, null, null, true,
                                                         dealWay.DealWayId, null, "批量开卡", operatorUser.UserId, 0,
                                                         Genders.Male, null);
                    UnityContainer.BuildUp(command);
                    int code = command.Validate();
                    if (code != ResponseCode.Success)
                        msgResult.CodeText = "不好意思,验证失败";

                    command.Execute(operatorUser);
                    decimal? saleFee = 0m;
                    if (command.AccountType != null)
                    {
                        saleFee = HostSite.SaleCardFee;
                        // 手续费
                        //
                        if (saleFee != null && saleFee.Value != 0m)
                        {
                            account.ChargingAmount += saleFee.Value;
                            AccountService.Update(account);
                            DealWay d = DealWayService.Query().FirstOrDefault(x => x.State == States.Normal);
                            var systemDealLog = new SystemDealLog(serialNo, operatorUser)
                                                    {
                                                        Amount = saleFee.Value,
                                                        DealWayId = (d == null ? 0 : d.DealWayId),
                                                        DealType = SystemDealLogTypes.SaldCardFee,
                                                        Addin = account.AccountId.ToString()
                                                    };
                            SystemDealLogService.Create(systemDealLog);
                            if (d.IsCash)
                                CashDealLogService.Create(new CashDealLog(systemDealLog.Amount, 0, operatorUser.UserId,
                                                                          systemDealLog.DealType));
                        }
                    }

                    // sale Id
                    //

                    Logger.LogWithSerialNo(LogTypes.AccountOpen, serialNo, command.Account.AccountId,
                                           command.Account.Name);


                    if (!string.IsNullOrEmpty(HostSite.TicketTemplateOfOpen))
                    {
                        DealLog dealLog = command.DealLog;
                        string msg = HostSite.TicketTemplateOfOpen;
                        msg = MessageFormator.FormatForOperator(msg, SecurityHelper.GetCurrentUser());
                        msg = MessageFormator.Format(msg, dealLog);
                        msg = MessageFormator.FormatHowToDeal(msg, command.DealWay.DisplayName);
                        msg = MessageFormator.Format(msg, command.DealLog);
                        msg = MessageFormator.Format(msg, command.AccountType);
                        msg = MessageFormator.Format(msg, command.Owner);
                        msg = MessageFormator.Format(msg, HostSite);
                        PrintTicketService.Create(new PrintTicket(LogTypes.AccountOpen, serialNo, msg, command.Account));
                    }
                    tran.Commit();
                     msgResult.Code = 1;
                    msgResult.CodeText = "会员建卡 " + account.Name + " 成功";
                     
                    return msgResult;
                }
            }
            catch (Exception ex)
            {
                msgResult.CodeText = "不好意思,系统异常";
                Logger.Error("批量发放会员卡", ex);
                return msgResult;
            }
        }

        public void Query()
        { 
            //系统总部应该查询全部时可以看到所有的卡，并可以进行编辑。
            //QueryObject<Account> query = InnerQuery();

            //var currentUser = this.SecurityHelper.GetCurrentUser();

            //if (this.Distributor == -10001 || this.Distributor == 0)
            //{
            //    int distributorId = 0;
            //    if (currentUser is AdminUserModel)//系统总部。可以看到所有的卡
            //        List = query.ToList(this, u => new ListAccount(u));
            //    else if (currentUser is DistributorUserModel)//经销商只能看到他和他下属的卡。
            //    {
            //        distributorId = ((DistributorUserModel)currentUser).DistributorId;
            //        var totalId = DistributorService.Query().Select(x => x.DistributorId).ToList();
            //        var ids = GetChildrenDistributorId(distributorId, totalId);
            //        ids.Add(distributorId);
            //        var q1 = from c in query where ids.Contains(c.DistributorId) select c;
            //        List = q1.ToList(this, u => new ListAccount(u));
            //    }
            //}
            //else
            //{
            //    List = query.Where(x => x.DistributorId == this.Distributor).ToList(this, u => new ListAccount(u));
            //}
            //List<AccountType> accountTypes =
            //    AccountTypeService.Query(new AccountTypeRequest { State = States.Normal }).ToList();

            //List.Merge(accountTypes,
            //           (a, b) => a.InnerObject.AccountTypeId == b.AccountTypeId,
            //           (a, b) => a.AccountType = b.FirstOrDefault() == null ? "" : b.FirstOrDefault().DisplayName
            //    );

            ////----
            //var q = DistributorService.Query();
            //UserRequest user = new UserRequest();
            //var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();

            //var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
            //var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();

            ////----
            //var shopIds = List.Select(x => x.InnerObject.ShopId).ToArray();
            //var shops = ShopService.GetByIds(shopIds).ToList();
            //shops.Add(Ecard.Models.Shop.Default);
            //foreach (var account in List)
            //{
            //    if (account.InnerObject.DistributorId == 0)
            //        account.DistributorName = "系统总部";
            //    else
            //    {
            //        var distributor1 = qq.FirstOrDefault(u => u.Key == account.InnerObject.DistributorId);
            //        if (distributor1 != null)
            //        {
            //            account.DistributorName = distributor1.Name;
            //        }
            //    }
            //    if (account.InnerObject.ShopId == 0)
            //        account.ShopName = "系统总部";
            //    else
            //    {
            //        var shop = shops.FirstOrDefault(x => x.ShopId == account.InnerObject.ShopId);
            //        if (shop != null)
            //        {
            //            account.ShopName = shop.DisplayName;
            //        }
            //    }

            //}
        }
        public void New_Query(out string pageHtml)
        {
             
            pageHtml = string.Empty;
            //系统总部应该查询全部时可以看到所有的卡，并可以进行编辑。 
            var request = new AccountRequest();
            request.NameWith = Name;
            if (!(request.State != States.All))
            {
                request.State = null;
            } 
            if (MobileState == MobileStates.IsAvailable)
            {
                request.IsMobileAvailable = true;
            }
            if (MobileState == MobileStates.IsUnavailable)
            {
                request.IsMobileAvailable = false;
            } 
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var currentUser = this.SecurityHelper.GetCurrentUser();
            if ((currentUser is DistributorUserModel))
            {
                request.ShopId = ((DistributorUserModel)currentUser).DistributorId;
            }

            var query = AccountService.NewQuery(request);


            if (query != null) 
            {
                //if (this.Distributor == -10001 || this.Distributor == 0)
                //{
                //    int distributorId = 0;
                //    if (currentUser is AdminUserModel)//系统总部。可以看到所有的卡 
                //        List = query.ModelList.ToList(this, u => new ListAccount(u));
                //    else if (currentUser is DistributorUserModel)//经销商只能看到他和他下属的卡。
                //    {
                //        distributorId = ((DistributorUserModel)currentUser).DistributorId;
                //        var totalId = DistributorService.Query().Select(x => x.DistributorId).ToList();
                //        var ids = GetChildrenDistributorId(distributorId, totalId);
                //        ids.Add(distributorId);
                //        var q1 = from c in query.ModelList where ids.Contains(c.DistributorId) select c;
                //        List = q1.ToList(this, u => new ListAccount(u));
                //    }
                //}
                //else
                //{
                    List = query.ModelList.ToList(this, u => new ListAccount(u));
                //}
                List<AccountType> accountTypes =
               AccountTypeService.Query(new AccountTypeRequest { State = States.Normal }).ToList();

                List.Merge(accountTypes,
                           (a, b) => a.InnerObject.AccountTypeId == b.AccountTypeId,
                           (a, b) => a.AccountType = b.FirstOrDefault() == null ? "" : b.FirstOrDefault().DisplayName
                    );

                ////----
                //var q = DistributorService.Query();
                //UserRequest user = new UserRequest();
                //var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();

                //var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
                //var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();

                ////----
                //var shopIds = List.Select(x => x.InnerObject.ShopId).ToArray();
                //var shops = ShopService.GetByIds(shopIds).ToList();
                //shops.Add(Ecard.Models.Shop.Default);
                //foreach (var account in List)
                //{
                //    if (account.InnerObject.DistributorId == 0)
                //        account.DistributorName = "系统总部";
                //    else
                //    {
                //        var distributor1 = qq.FirstOrDefault(u => u.Key == account.InnerObject.DistributorId);
                //        if (distributor1 != null)
                //        {
                //            account.DistributorName = distributor1.Name;
                //        }
                //    }
                //    if (account.InnerObject.ShopId == 0)
                //        account.ShopName = "系统总部";
                //    else
                //    {
                //        var shop = shops.FirstOrDefault(x => x.ShopId == account.InnerObject.ShopId);
                //        if (shop != null)
                //        {
                //            account.ShopName = shop.DisplayName;
                //        }
                //    }

                //}
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
            } 
        }
        public List<ListAccount> AjaxGet(AccountRequest request, out string pageHtml)
        {
            List<ListAccount> data=null;
            pageHtml = string.Empty;
            int? distributor = 0;
            //系统总部应该查询全部时可以看到所有的卡，并可以进行编辑。 
            request.NameWith = Name;
            if (!(request.State != States.All))
            {
                request.State = null;
            } 
            if (MobileState == MobileStates.IsAvailable)
            {
                request.IsMobileAvailable = true;
            }
            if (MobileState == MobileStates.IsUnavailable)
            {
                request.IsMobileAvailable = false;
            } 
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var currentUser = this.SecurityHelper.GetCurrentUser();



            if ((currentUser is DistributorUserModel))
            {
                request.ShopId = ((DistributorUserModel)currentUser).DistributorId;
            }
            else
            {
                if (request.ShopId == Globals.All)
                {
                    distributor = request.ShopId;
                    request.ShopId = null;
                }
            }
            
            var query = AccountService.NewQuery(request);


            if (query != null)
            {
                //if (distributor == -10001 || distributor == 0)
                //{
                //    int distributorId = 0;
                //    if (currentUser is AdminUserModel)//系统总部。可以看到所有的卡
                //        data = query.ModelList.Select(u => new ListAccount(u)).ToList();
                //    else if (currentUser is DistributorUserModel)//经销商只能看到他和他下属的卡。
                //    {
                //        distributorId = ((DistributorUserModel)currentUser).DistributorId;
                //        var totalId = DistributorService.Query().Select(x => x.DistributorId).ToList();
                //        var ids = GetChildrenDistributorId(distributorId, totalId);
                //        ids.Add(distributorId);
                //        var q1 = from c in query.ModelList where ids.Contains(c.DistributorId) select c;
                //        data = q1.Select(u => new ListAccount(u)).ToList();
                //    }
                //}
                //else
                //{
                    data = query.ModelList.Select(u => new ListAccount(u)).ToList();
                //}


                List<AccountType> accountTypes =
                    AccountTypeService.Query(new AccountTypeRequest { State = States.Normal }).ToList();

                data.Merge(accountTypes,
                           (a, b) => a.InnerObject.AccountTypeId == b.AccountTypeId,
                           (a, b) => a.AccountType = b.FirstOrDefault() == null ? "" : b.FirstOrDefault().DisplayName
                    );

                ////----
                //var q = DistributorService.Query();
                //UserRequest user = new UserRequest();
                //var users = MembershipService.QueryUsers<DistributorUser>(user).ToList();

                //var list = (from x in (q.Where(x => users.Any(u => u.UserId == x.UserId)).ToList()) select new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
                //var qq = (from x in list select new IdNamePair { Key = x.DistributorId, Name = x.DisplayName }).ToList();

                ////----
                //var shopIds = data.Select(x => x.InnerObject.ShopId).ToArray();
                //var shops = ShopService.GetByIds(shopIds).ToList();
                //shops.Add(Ecard.Models.Shop.Default);
                //foreach (var account in data)
                //{
                //    if (account.InnerObject.DistributorId == 0)
                //        account.DistributorName = "系统总部";
                //    else
                //    {
                //        var distributor1 = qq.FirstOrDefault(u => u.Key == account.InnerObject.DistributorId);
                //        if (distributor1 != null)
                //        {
                //            account.DistributorName = distributor1.Name;
                //        }
                //    }
                //    if (account.InnerObject.ShopId == 0)
                //        account.ShopName = "系统总部";
                //    else
                //    {
                //        var shop = shops.FirstOrDefault(x => x.ShopId == account.InnerObject.ShopId);
                //        if (shop != null)
                //        {
                //            account.ShopName = shop.DisplayName;
                //        }
                //    }

                //}

                foreach (var item in data)
                {



                    //if (item.InnerObject.State < AccountStates.Saled && item.InnerObject.State > 10 && this.SecurityHelper.HasPermission("account"))
                    //{

                    //    item.boor += "<a href='#' onclick=OperatorThis('SetDistributor','/Account/SetDistributor/" + item.AccountId + "') class='tablelink'>更改经销商 </a> ";
                    //}
                    if (item.InnerObject.State == States.Normal)
                    {
                        //if (this.SecurityHelper.HasPermission("accountrebate"))
                        //    item.boor += "<a href='#' onclick=OperatorThis('Rebate','/Account/Rebate/" + item.AccountId + "') class='tablelink'>返利 </a> ";
                        if (this.SecurityHelper.HasPermission("accountowner"))
                            item.boor += "<a href='#' onclick=OperatorThis('Owner','/Account/Owner/" + item.AccountId + "') class='tablelink'>编辑 </a> ";
                    }
                    if (this.SecurityHelper.HasPermission("accountdelete"))
                        item.boor += "<a href='#' onclick=OperatorThis('Delete','/Account/Delete/" + item.AccountId + "') class='tablelink'>删除 </a> ";
                }
                if (query.ModelList != null)
                    pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
                return data;
            }
            return null;
        }


      

        public List<Account> Querys()
        {
            QueryObject<Account> query = InnerQuery();
            return query.ToList();
        }

        /// <summary>
        /// 取得一级经销商所有的下属经销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> GetChildrenDistributorId(int id, List<int> list)
        {
            List<int> ids = new List<int>();
            foreach (var item in list)
            {
                if (GetTopLevelDistributorId(item, id))
                    ids.Add(item);
            }
            return ids;
        }
        /// <summary>
        /// 是否是自己的下级
        /// </summary>
        /// <param name="distributorId">下级</param>
        /// <param name="pid">上级</param>
        /// <returns></returns>
        public bool GetTopLevelDistributorId(int distributorId, int pid)
        {
            var parent = DistributorService.GetById(distributorId);
            if (parent != null)
                if (parent.ParentId == pid)
                    return true;
            if (parent.ParentId == 0)
                return false;
            else return GetTopLevelDistributorId(parent.ParentId, pid);

        }

        private QueryObject<Account> InnerQuery()
        {
            AccountRequest rsq = GetAccountRequest();
            rsq.Content = Content;
            QueryObject<Account> query = AccountService.Query(rsq);
            return query;
        }

        private AccountRequest GetAccountRequest()
        {
            var rsq = new AccountRequest();

            rsq.NameWith = Name;
            if (State != States.All)
                rsq.State = State;
            if (MobileState == MobileStates.IsAvailable)
            {
                rsq.IsMobileAvailable = true;
            }
            if (MobileState == MobileStates.IsUnavailable)
            {
                rsq.IsMobileAvailable = false;
            }
            if (Shop != Globals.All)
            {
                rsq.ShopId = Shop;
            }
            return rsq;
        }

        public void SendMessage()
        {
            List<AccountWithOwner> accounts;
            if (SendToAll == true)
            {
                AccountRequest request = GetAccountRequest();
                accounts = AccountService.QueryAccountWithOwner(request).ToList();
            }
            else
            {
                int[] ids = CheckItems.GetCheckedIds();
                accounts = AccountService.QueryAccountWithOwner(new AccountRequest { Ids = ids }).ToList();
            }
            foreach (AccountWithOwner account in accounts)
            {
                string msg = MessageFormator.Format(MessageTemplate, account);
                msg = MessageFormator.Format(msg, HostSite);
                msg = MessageFormator.Format(msg,
                                             new AccountUser
                                                 {
                                                     DisplayName = account.OwnerDisplayName,
                                                     Gender = Genders.Male,
                                                     Mobile = account.OwnerMobileNumber
                                                 });
                SmsHelper.Send(account.OwnerMobileNumber, msg);
            }
            AddMessage("发送短信成功！共发送给 {0} 会员", accounts.Count);
        }
    }
}
