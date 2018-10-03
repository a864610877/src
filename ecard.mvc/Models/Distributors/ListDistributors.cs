using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;
using Ecard.Mvc.ActionFilters;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Distributors
{
    public class ListDistributors : EcardModelListRequest<ListDistributor>
    {
        public ListDistributors()
        {
            OrderBy = "DistributorId";
        }

        private string _name;
        public string Name
        {
            get { return _name.TrimSafty(); }
            set { _name = value; }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            //yield return new ActionMethodDescriptor("Deletes", null);
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };
            yield return new ActionMethodDescriptor("SendSmsMessage", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListDistributor item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.DistributorId });
            if (item.InnerObject.State == DistributorStates.Normal)
            {
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.DistributorId });
            }
            if (item.InnerObject.State == DistributorStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.DistributorId });
           // yield return new ActionMethodDescriptor("Delete", null, new { id = item.DistributorId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Distributor>("State", UserStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
        [Dependency, NoRender]
        public IDistributorService DistributorService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }
        [Dependency]
        [NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        [NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        public void Query(out string pageHtml)
        {
            pageHtml = string.Empty;
            var query = DistributorService.Query().ToList();
            var request = new UserRequest();
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            if (!string.IsNullOrEmpty(this.Name))
                request.NameWith = Name;
            var users = MembershipService.QueryUsers<DistributorUser>(request).ToList();
            var user = this.SecurityHelper.GetCurrentUser();
            int pid = 0;
             if (user is DistributorUserModel)
            {
                pid = ((DistributorUserModel)user).DistributorId;
                query = query.Where(x => users.Any(u => u.UserId == x.UserId) && x.ParentId == pid).ToList();
            }
            if (this.State == 100000)
            {
                
            }
            else
            {
                query = query.Where(x => users.Any(u => u.UserId == x.UserId) && x.State == this.State).ToList();//.ToList(this, x => new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) });
            }
            List = query.ToList(this, x => new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) });
            foreach (var item in List)
            {
                var rate = DistributorService.GetAccountLevelPolicyRates(item.DistributorId).FirstOrDefault();
                if(rate!=null)
                  item.Rate = rate.Rate*100;
            }
            if (query.Count >0)
            { 
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.Count);
            }
        }
        public List<ListDistributor> AjaxGet(UserRequest request, out string pageHtml)
        {
            var totalCount = 0;
            pageHtml = string.Empty;
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var query = DistributorService.Query().ToList(); 
            if (!string.IsNullOrEmpty(this.Name))
                request.NameWith = Name;
            var users = MembershipService.QueryUsers<DistributorUser>(request).ToList();
            var user = this.SecurityHelper.GetCurrentUser();
            int pid = 0;
            if (user is DistributorUserModel)
            {
                pid = ((DistributorUserModel)user).DistributorId;
                query = query.Where(x => users.Any(u => u.UserId == x.UserId) && x.ParentId == pid).ToList();
            }
            if (!(request.State == null))
            {
                query = query.Where(x => users.Any(u => u.UserId == x.UserId) && x.State == request.State).ToList();//.ToList(this, x => new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) });
            }
            totalCount = query.Count;
            query = query.Skip(Convert.ToInt32(request.PageSize) * Convert.ToInt32((request.PageIndex - 1))).Take(Convert.ToInt32(request.PageSize)).ToList();
            var datas = query.Select(x => new ListDistributor(x) { Owner = users.First(u => u.UserId == x.UserId) }).ToList();
            foreach (var item in datas)
            {
                var rate = DistributorService.GetAccountLevelPolicyRates(item.DistributorId).FirstOrDefault();
                if (rate != null)
                    item.Rate = rate.Rate * 100;
            } 
            foreach (var item in datas)
            {
                item.boor += "<a href='#' onclick=OperatorThis('Edit','/Distributor/Edit/" + item.DistributorId + "') class='tablelink'>编辑 </a> ";
                if (item.InnerObject.State == UserStates.Normal)
                    item.boor += "<a href='#' onclick=OperatorThis('Suspend','/Distributor/Suspend/" + item.DistributorId + "') class='tablelink'>停用 </a> ";
                if (item.InnerObject.State == UserStates.Invalid )
                    item.boor += "<a href='#' onclick=OperatorThis('Resume','/Distributor/Resume/" + item.DistributorId + "') class='tablelink'>启用 </a> ";
            }
            if (query.Count >0)
            {
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, totalCount);
            } 
            return datas;
        }

        public ResultMsg Delete(int id)
        {
             ResultMsg msg=new ResultMsg();
             try
             {
                 TransactionHelper.BeginTransaction();
                 var distributor = DistributorService.GetById(id);
                 if (distributor != null)
                 {
                     //把下属经销商的上级设为要删除的经销商的上级。
                     List<Distributor> list = DistributorService.GetByParentId(id);
                     foreach (var item in list)
                     {
                         if (item != null)
                         {
                             //var user1 = MembershipService.GetUserById(item.UserId);
                             item.ParentId = distributor.ParentId;
                             //TransactionHelper.BeginTransaction();
                             // MembershipService.DeleteUser(user1);
                             DistributorService.Update(item);

                             Logger.LogWithSerialNo(LogTypes.DistributorEdit, SerialNoHelper.Create(), id, item.Name);
                             AddMessage("update.success", item.Name);
                             //TransactionHelper.Commit();

                             CacheService.Refresh(CacheKeys.DistributorKey);
                         }
                     }
                     var accounts = AccountService.Query(new AccountRequest()).Where(x => x.DistributorId == id);
                     foreach (var account in accounts)
                     {
                         account.DistributorId = distributor.ParentId;
                         AccountService.Update(account);
                     }
                     var user = MembershipService.GetUserById(distributor.UserId);
                     MembershipService.DeleteUser(user);
                     DistributorService.Delete(distributor);

                     Logger.LogWithSerialNo(LogTypes.DistributorDelete, SerialNoHelper.Create(), id, user.Name);
                     //AddMessage("delete.success", user.Name);
                     msg.Code = 1;
                     msg.CodeText = "删除 " + user.DisplayName + " 成功";
                     TransactionHelper.Commit();

                     CacheService.Refresh(CacheKeys.PosKey);

                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到经销商";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("删除经销商", ex);
                 return msg;
             }
        }

        public ResultMsg Resume(int id)
        {
             ResultMsg msg = new ResultMsg();
             try
             {
                 var item = DistributorService.GetById(id);
                 if (item != null && item.State == DistributorStates.Invalid)
                 {
                     item.State = DistributorStates.Normal;

                     var owner = MembershipService.GetUserById(item.UserId);

                     TransactionHelper.BeginTransaction();
                     DistributorService.Update(item);

                     owner.State = UserStates.Normal;
                     MembershipService.UpdateUser(owner);
                     Logger.LogWithSerialNo(LogTypes.DistributorResume, SerialNoHelper.Create(), id, owner.Name);
                    // AddMessage("resume.success", owner.Name);
                     msg.Code = 1;
                     msg.CodeText = "启用经销商 " + owner.DisplayName + " 成功";
                     TransactionHelper.Commit();
                     CacheService.Refresh(CacheKeys.PosKey);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到经销商";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("启用经销商", ex);
                 return msg;
             }
        }

        public ResultMsg Suspend(int id)
        {
              ResultMsg msg=new ResultMsg();
              try
              {
                  var distributor = DistributorService.GetById(id);
                  if (distributor != null && distributor.State == DistributorStates.Normal)
                  {
                      var user = MembershipService.GetUserById(distributor.UserId) as DistributorUser;

                      TransactionHelper.BeginTransaction();
                      distributor.State = DistributorStates.Invalid;
                      DistributorService.Update(distributor);

                      user.State = UserStates.Invalid;
                      MembershipService.UpdateUser(user);
                      Logger.LogWithSerialNo(LogTypes.DistributorSuspend, SerialNoHelper.Create(), id, user.Name);
                     // AddMessage("suspend.success", user.Name);
                      msg.Code = 1;
                      msg.CodeText = "停用经销商 " + distributor.DisplayName + " 成功";
                      TransactionHelper.Commit();
                      CacheService.Refresh(CacheKeys.PosKey);
                  }
                  else
                  {
                      msg.CodeText = "不好意思,没有找到经销商";
                  }
                  return msg;
              }
              catch (Exception ex)
              {
                  msg.CodeText = "不好意思,系统异常";
                  Logger.Error("停用经销商", ex);
                  return msg;
              }
        }
        [NoRender, Dependency]
        public SmsHelper SmsHelper { get; set; }
        [NoRender, Dependency]
        public Site HostSite { get; set; }
    }
}