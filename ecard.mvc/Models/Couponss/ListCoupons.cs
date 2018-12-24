using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Requests;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Couponss
{
    public class ListCoupons : EcardModelListRequest<ListCoupon>
    {
        public ListCoupons()
        {
            OrderBy = "id asc";
        }
        private Bounded _couponTypeBounded;
        public Bounded CouponType
        {
            get
            {
                if (_couponTypeBounded == null)
                {
                    _couponTypeBounded = Bounded.Create<Coupons>("couponsType", CouponsType.DiscountedVolume);
                }
                return _couponTypeBounded;
            }
            set { _couponTypeBounded = value; }
        }
        private Bounded _stateBounded;
        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.Create<Coupons>("state", CouponsState.All);
                }
                return _stateBounded;
            }
            set { _stateBounded = value; }
        }
        private DataRange _data;
        public DataRange Data
        {
            get
            {
                if (_data == null)
                    _data = new DataRange();
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public string Name { get; set; }
        public string Code { get; set; }

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }

        [NoRender, Dependency]
        public ICouponsService CouponsService { get; set; }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListCoupon item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.id });
            if (item.InnerObject.state == CouponsState.Normal)
            {
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.id });
            }
            else
            {
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.id });
            }
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.InnerObject.id });
        }


        public void Query(out string pageHtml)
        {

            pageHtml = string.Empty;
            //系统总部应该查询全部时可以看到所有的卡，并可以进行编辑。 
            var request = new CouponsRequest();
            request.name = Name;
            if (!(request.state != CouponsState.All))
            {
                request.state = null;
            }
            request.couponsType = CouponsType.DiscountedVolume;
            request.startTime = Data.Start;
            request.endTime = Data.End;
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var query = CouponsService.Query(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListCoupon(u));
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
            }
        }

        public List<ListCoupon> AjaxGet(CouponsRequest request, out string pageHtml)
        {
            List<ListCoupon> data = null;
            pageHtml = string.Empty;
            if (!(request.state != AdmissionTicketState.All))
            {
                request.state = null;
            }
            //if (!(request.couponsType != CouponsType.All))
            //{
            //    request.couponsType = null;
            //}
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var query = CouponsService.Query(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListCoupon(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
                foreach (var item in data)
                {
                    if (this.SecurityHelper.HasPermission(Ecard.Permissions.CouponsEdit))
                        item.boor += "<a href='#' onclick=OperatorThis('Edit','/Coupons/Edit/" + item.id + "') class='tablelink'>编辑 </a> ";
                    if (item.InnerObject.state == AdmissionTicketState.Invalid)
                    {
                        if (this.SecurityHelper.HasPermission(Ecard.Permissions.CouponsResume))
                            item.boor += "<a href='#' onclick=OperatorThis('Resume','/Coupons/Resume/" + item.id + "') class='tablelink'>启用</a> ";
                    }
                    if (item.InnerObject.state == AdmissionTicketState.Normal)
                    {
                        if (this.SecurityHelper.HasPermission(Ecard.Permissions.CouponsSuspend))
                            item.boor += "<a href='#' onclick=OperatorThis('Suspend','/Coupons/Suspend/" + item.id + "') class='tablelink'>停售</a> ";
                    }
                    if (this.SecurityHelper.HasPermission(Ecard.Permissions.AdmissionTicketDelete))
                        item.boor += "<a href='#' onclick=OperatorThis('Delete','/Coupons/Delete/" + item.id + "') class='tablelink'>删除 </a> ";
                }
            }
            return data;
        }


        public ResultMsg Suspend(int id)
        {
            ResultMsg msg = new ResultMsg();
            try
            {
                var item = CouponsService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Normal)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Invalid;
                    CouponsService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.CouponSuspend, serialNo, item.id, item.name);
                }
                msg.Code = 1;
                msg.CodeText = "停用优惠券成功";
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.CouponSuspend, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }
        }
        public ResultMsg Resume(int id)
        {
            ResultMsg msg = new ResultMsg();
            try
            {
                var item = CouponsService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Invalid)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Normal;
                    CouponsService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.CouponResume, serialNo, item.id, item.name);
                }
                msg.Code = 1;
                msg.CodeText = "启用优惠券成功";
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.CouponResume, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }
        }

        public ResultMsg Delete(int id)
        {
            ResultMsg msg = new ResultMsg() { Code = -1, CodeText = "删除失败" };
            try
            {
                var item = CouponsService.GetById(id);
                if (item != null)
                {
                    var serialNo = SerialNoHelper.Create();
                    CouponsService.Delete(item);
                    Logger.LogWithSerialNo(LogTypes.CouponDelete, serialNo, item.id, item.name);
                    msg.Code = 1;
                    msg.CodeText = "删除优惠券" + item.name + "成功";
                }
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.CouponDelete, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }

        }
    }
}
