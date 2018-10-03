using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace Ecard.Mvc.Models.PosEndPointes
{
    public class ListPosEndPoints : EcardModelListRequest<ListPosEndPoint>
    {
        public ListPosEndPoints()
        {
            OrderBy = "PosEndPointId";
            //_name = new Text();
        }

        private string _name;
        //public string Name
        //{
        //    get { return _name.TrimSafty(); }
        //    set { _name = value; }
        //}
        [UIHint("Text")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private Bounded _shopBounded;
        [CheckUserType(typeof(AdminUser))]
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

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
            yield return new ActionMethodDescriptor("Export", null, new { export = "excel" }) { IsPost = true };
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPosEndPoint user)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = user.PosEndPointId });
            if (user.InnerObject.State == PosEndPointStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = user.PosEndPointId });
            if (user.InnerObject.State == PosEndPointStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = user.PosEndPointId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = user.PosEndPointId });
        }
        [NoRender, Dependency]
        public IShopService ShopService { get; set; }
        [NoRender, Dependency]
        public IPosEndPointService PosEndPointService { get; set; }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<PosEndPoint>("State", PosEndPointStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }

        public void Ready()
        {
            var request = new ShopRequest() { State = ShopStates.Normal };
            

            var currentUser = SecurityHelper.GetCurrentUser();
            ShopUser shopUser = currentUser.CurrentUser as ShopUser;
            if (shopUser != null)
                request.ShopId = shopUser.ShopId;
            var query = ShopService.Query(request);
            this.Shop.Bind(query.Select(x => new IdNamePair() { Key = x.ShopId, Name = x.DisplayName }), true);
        }
        [NoRender,Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        public void Query(out string pageHtml)
        {
            PosEndPointRequest request = new PosEndPointRequest();
            pageHtml = string.Empty;
            request.NameWith = Name;
            if (State != PosEndPointStates.All)
                request.State = State;
            if (Shop != Globals.All)
                request.ShopId = Shop;
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var currentUser = SecurityHelper.GetCurrentUser();
            ShopUser shopUser = currentUser.CurrentUser as ShopUser;
            if (shopUser != null)
                request.ShopId = shopUser.ShopId;

            var _tables = PosEndPointService.New_Query(request);
            List = _tables.ModelList.ToList(this, x => new ListPosEndPoint(x));
            
            var shops = ShopService.Query(new ShopRequest() {ShopIds = List.Select(x => x.InnerObject.ShopId).ToArray()}).ToList();
            List.Merge(shops,
                       (p, s) => p.InnerObject.ShopId == s.ShopId,
                       (p, ss) => p.Shop = ss.FirstOrDefault()
                );
            if (_tables != null)
            {
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, _tables.TotalCount);
            }
        }
        public List<ListPosEndPoint> AjaxGet(PosEndPointRequest request, out string pageHtml)
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
            var currentUser = SecurityHelper.GetCurrentUser();
            ShopUser shopUser = currentUser.CurrentUser as ShopUser;
            if (shopUser != null)
                request.ShopId = shopUser.ShopId;
            var _tables = PosEndPointService.New_Query(request);
            var datas = _tables.ModelList.Select(x => new ListPosEndPoint(x)).ToList();

         
                var shops = ShopService.Query(new ShopRequest() { ShopIds = datas.Select(x => x.InnerObject.ShopId).ToArray() }).ToList();
                datas.Merge(shops,
                           (p, s) => p.InnerObject.ShopId == s.ShopId,
                           (p, ss) => p.Shop = ss.FirstOrDefault()
                    );
           

           
           
            foreach (var item in datas)
            {
                item.boor += "<a href='#' onclick=OperatorThis('Edit','/PosEndPoint/Edit/" + item.PosEndPointId + "') class='tablelink'>编辑 </a> ";
                if (item.InnerObject.State == UserStates.Normal)
                    item.boor += "<a href='#' onclick=OperatorThis('Suspend','/PosEndPoint/Suspend/" + item.PosEndPointId + "') class='tablelink'>停用 </a> "; 
                if (item.InnerObject.State == UserStates.Invalid)
                    item.boor += "<a href='#' onclick=OperatorThis('Resume','/PosEndPoint/Resume/" + item.PosEndPointId + "') class='tablelink'>启用 </a> ";
                item.boor += "<a href='#' onclick=OperatorThis('Delete','/PosEndPoint/Delete/" + item.PosEndPointId + "') class='tablelink'>删除 </a> ";
            }
            if (_tables != null)
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, _tables.TotalCount);
            return datas;
        }

        public ResultMsg Resume(int id)
        {
             ResultMsg msg = new ResultMsg();
             try
             {
                 var posEndPoint = PosEndPointService.GetById(id);
                 if (posEndPoint != null && posEndPoint.State == PosEndPointStates.Invalid)
                 {
                     var shop = ShopService.GetById(posEndPoint.ShopId);
                     posEndPoint.State = PosEndPointStates.Normal;
                     PosEndPointService.Update(posEndPoint);

                     Logger.LogWithSerialNo(LogTypes.PosResume, SerialNoHelper.Create(), id, posEndPoint.Name, shop.Name);
                    // AddMessage("resume.success", posEndPoint.Name, shop.Name);
                     msg.Code = 1;
                     msg.CodeText = "启用终端 " + posEndPoint.DisplayName + " 成功";
                     CacheService.Refresh(CacheKeys.PosKey);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到终端";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("启用终端", ex);
                 return msg;
             }
        }

        public ResultMsg Suspend(int id)
        {
              ResultMsg msg=new ResultMsg();
              try
              {
                  var posEndPoint = PosEndPointService.GetById(id);
                  if (posEndPoint != null && posEndPoint.State == PosEndPointStates.Normal)
                  {
                      posEndPoint.State = PosEndPointStates.Invalid;
                      PosEndPointService.Update(posEndPoint);

                      var shop = ShopService.GetById(posEndPoint.ShopId);
                      Logger.LogWithSerialNo(LogTypes.PosSuspend, SerialNoHelper.Create(), id, posEndPoint.Name, shop.Name);
                     // AddMessage("suspend.success", posEndPoint.Name, shop.Name);
                      msg.Code = 1;
                      msg.CodeText = "停用终端 " + posEndPoint.DisplayName + " 成功";
                      CacheService.Refresh(CacheKeys.PosKey);
                  }
                  else
                  {
                      msg.CodeText = "不好意思,没有找到终端";
                  }
                  return msg;
              }
              catch (Exception ex)
              {
                  msg.CodeText = "不好意思,系统异常";
                  Logger.Error("停用终端", ex);
                  return msg;
              }
        }

        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }
        public ResultMsg Delete(int id)
        {
             ResultMsg msg=new ResultMsg();
             try
             {
                 var posEndPoint = PosEndPointService.GetById(id);
                 if (posEndPoint != null)
                 {
                     var shop = ShopService.GetById(posEndPoint.ShopId);
                     Logger.LogWithSerialNo(LogTypes.PosDelete, SerialNoHelper.Create(), id, posEndPoint.Name, shop.Name);
                     //AddMessage("delete.success", posEndPoint.Name, shop.Name);
                     msg.Code = 1;
                     msg.CodeText = "删除终端 " + posEndPoint.DisplayName + " 成功";
                     PosEndPointService.Delete(posEndPoint);
                     CacheService.Refresh(CacheKeys.PosKey);
                 }
                 else
                 {
                     msg.CodeText = "不好意思,没有找到终端";
                 }
                 return msg;
             }
             catch (Exception ex)
             {
                 msg.CodeText = "不好意思,系统异常";
                 Logger.Error("删除终端", ex);
                 return msg;
             }
        }
    }
}