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
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Shops
{ 
    public class ListShops : EcardModelListRequest<ListShop>
    {
        public ListShops()
        {
            OrderBy = "ShopId";
        }

        private string _name;
        [UIHint("shopName")]
        public string Name
        {
            get { return _name.TrimSafty(); }
            set { _name = value; }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName.TrimSafty(); }
            set { _displayName = value; }
        } 

        [Bounded(typeof(MobileStates))]
        [NoRender]
        public int IsMobileAvaliable { get; set; }

        [NoRender]
        public string MessageTemplate { get; set; }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };
            //yield return new ActionMethodDescriptor("SendSmsMessage", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListShop item)
        {
            yield return new ActionMethodDescriptor("SerachPos", null, new { id = item.ShopId });
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.ShopId });
            if (item.InnerObject.State == ShopStates.Normal && !item.InnerObject.BuildIn)
            {
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.ShopId });
                //yield return new ActionMethodDescriptor("CreateDog", "User", new { id = item.Owner.UserId });
            }
            if (item.InnerObject.State == ShopStates.Invalid && !item.InnerObject.BuildIn)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.ShopId });
            yield return new ActionMethodDescriptor("Create", "PosEndPoint", new { shopId = item.ShopId });
            if (!item.InnerObject.BuildIn)
                yield return new ActionMethodDescriptor("Delete", null, new { id = item.ShopId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<Shop>("State", UserStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }
        [Dependency]
        [NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        [NoRender]
        public IPosEndPointService PosEndPointService { get; set; }
        public void Query(out string pageHtml)
        {
            pageHtml = string.Empty;
            var request = GetShopRequest();
            var query = ShopService.NewQuery(request);
            List = query.ModelList.ToList(this, x => new ListShop(x));
            List.Select(x => x).Merge(
                MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, List.Select(x => x.InnerObject.ShopId).ToArray()).ToList(),
                (shop, user) => shop.ShopId == user.ShopId,
                (shop, users) => shop.Owner = users.FirstOrDefault());
            if (query != null)
            {
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
            }
        }
        public List<ListShop> AjaxGet(ShopRequest request, out string pageHtml)
        {
            pageHtml = string.Empty;
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            if (request.State == ShopStates.All)
                request.State = null;
            var _tables = ShopService.NewQuery(request);
            var data = _tables.ModelList.Select(x => new ListShop(x)).ToList();
            data.Select(x => x).Merge(
                MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, data.Select(x => x.InnerObject.ShopId).ToArray()).ToList(),
                (shop, user) => shop.ShopId == user.ShopId,
                (shop, users) => shop.Owner = users.FirstOrDefault());

            foreach (var item in data)
            {
                item.boor += "<a href='#' onclick=OperatorThis('SerachPos','/Shop/SerachPos/" + item.ShopId + "') class='tablelink'>查看终端 </a> ";
                item.boor += "<a href='#' onclick=OperatorThis('Edit','/Shop/Edit/" + item.ShopId + "') class='tablelink'>编辑 </a> ";
                if (item.InnerObject.State == ShopStates.Normal && !item.InnerObject.BuildIn)
                {
                    //item.boor += "<a href='#' onclick=OperatorThis('CreateDog','/User/CreateDog/" + item.Owner.UserId + "') class='tablelink'>创建U盾 </a> ";
                    item.boor += "<a href='#' onclick=OperatorThis('Suspend','/Shop/Suspend/" + item.ShopId + "') class='tablelink' >停用 </a> ";
                }
                if (item.InnerObject.State == ShopStates.Invalid && !item.InnerObject.BuildIn)
                {
                    item.boor += "<a href='#' onclick=OperatorThis('Resume','/Shop/Resume/" + item.ShopId + "') class='tablelink'>启用 </a> ";
                    item.boor += "<a href='#' onclick=OperatorThis('Create','/PosEndPoint/Create/" + item.ShopId + "') class='tablelink'>创建Pos终端 </a> ";
                }
                if (!item.InnerObject.BuildIn)
                    item.boor += "<a href='#' onclick=OperatorThis('Delete','/Shop/Delete/" + item.ShopId + "') class='tablelink'>删除 </a> ";
            }
            if (_tables != null)
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, _tables.TotalCount); 
            return data;
        }
        private ShopRequest GetShopRequest()
        {
            var request = new ShopRequest();
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            request.NameWith = Name;
            request.DisplayNameWith = DisplayName;

            if (MobileState == MobileStates.IsAvailable)
            {
                request.IsMobileAvailable = true;
            }
            if (MobileState == MobileStates.IsUnavailable)
            {
                request.IsMobileAvailable = false;
            }
            if (State.Key != ShopStates.All)
                request.State = State;
            return request;
        }

        public ResultMsg Delete(int id)
        {
             ResultMsg msg=new ResultMsg();
             try
             {
                 var shop = ShopService.GetById(id);
                 if (shop != null)
                 {
                     var users = MembershipService.QueryUsersOfShops<ShopUser>(null, null, shop.ShopId).ToList();
                     var poses = PosEndPointService.Query(new PosEndPointRequest() { ShopId = shop.ShopId }).ToList();

                     TransactionHelper.BeginTransaction();
                     foreach (ShopUser shopUser in users.ToList())
                     {
                         MembershipService.DeleteUser(shopUser);
                     }
                     foreach (var pos in poses)
                     {
                         PosEndPointService.Delete(pos);
                     }
                     ShopService.Delete(shop);

                     Logger.LogWithSerialNo(LogTypes.ShopDelete, SerialNoHelper.Create(), id, shop.Name);
                    // AddMessage("delete.success", shop.Name);
                     msg.Code = 1;
                     msg.CodeText = "删除 " + shop.DisplayName + " 成功";
                     TransactionHelper.Commit();

                     CacheService.Refresh(CacheKeys.PosKey);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到商户";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("删除商户", ex);
                 return msg;
             }

        }

        public ResultMsg Resume(int id)
        {
            ResultMsg msg = new ResultMsg();
            try
            {
                var item = ShopService.GetById(id);
                if (item != null && item.State == ShopStates.Invalid)
                {
                    item.State = ShopStates.Normal;

                    var owner = MembershipService.QueryUsersOfShops<ShopUser>(null, ShopRoles.Owner, item.ShopId).FirstOrDefault();

                    TransactionHelper.BeginTransaction();
                    ShopService.Update(item);

                    owner.State = UserStates.Normal;
                    MembershipService.UpdateUser(owner);
                    Logger.LogWithSerialNo(LogTypes.ShopResume, SerialNoHelper.Create(), id, item.Name);
                    //AddMessage("resume.success", item.Name);
                    msg.Code = 1;
                    msg.CodeText = "启用商户 " + item.DisplayName + " 成功";
                    TransactionHelper.Commit();
                    CacheService.Refresh(CacheKeys.PosKey);
                }
                else
                {
                    msg.CodeText = "不好意思,没有找到商户";
                }
                return msg;
            }
            catch (Exception ex)
            {
                msg.CodeText = "不好意思,系统异常";
                Logger.Error("启用商户", ex);
                return msg;
            }
        }

        public ResultMsg Suspend(int id)
        {
            ResultMsg msg=new ResultMsg();
            try
            {
                var shop = ShopService.GetById(id);
                if (shop != null && shop.State == ShopStates.Normal)
                {
                    var users = MembershipService.QueryUsersOfShops<ShopUser>(null, null, shop.ShopId).ToList();
                    var poses = PosEndPointService.Query(new PosEndPointRequest() { ShopId = shop.ShopId }).ToList();

                    TransactionHelper.BeginTransaction();
                    shop.State = ShopStates.Invalid;
                    ShopService.Update(shop);


                    foreach (var user in users.ToList())
                    {
                        user.State = UserStates.Invalid;
                        MembershipService.UpdateUser(user);
                    }
                    foreach (var pos in poses.ToList())
                    {
                        pos.State = PosEndPointStates.Invalid;
                        PosEndPointService.Update(pos);
                    }
                    Logger.LogWithSerialNo(LogTypes.ShopSuspend, SerialNoHelper.Create(), id, shop.Name);
                    //AddMessage("suspend.success", shop.Name);
                    msg.Code = 1;
                    msg.CodeText = "停用商户 " + shop.DisplayName + " 成功";
                    TransactionHelper.Commit();
                    CacheService.Refresh(CacheKeys.PosKey);

                }
                else
                {
                    msg.CodeText = "不好意思,没有找到商户";
                }
                return msg;
            }
            catch (Exception ex)
            {
                msg.CodeText = "不好意思,系统异常";
                Logger.Error("停用商户", ex);
                return msg;
            }
        }

        public List<PosEndPoint> GetListPos(int id)
        {
            var poses = PosEndPointService.Query(new PosEndPointRequest() { ShopId = id }).ToList();
            return poses;
        }

        private Bounded _mobileStateBounded; 
        public Bounded MobileState
        {
            get
            {
                if (_mobileStateBounded == null)
                {
                    _mobileStateBounded = Bounded.Create<ListShops>("IsMobileAvaliable", MobileStates.None);
                }
                return _mobileStateBounded;
            }
            set { _mobileStateBounded = value; }
        }
        [NoRender, Dependency]
        public SmsHelper SmsHelper { get; set; }
        [NoRender, Dependency]
        public Site HostSite { get; set; } 
    }
}