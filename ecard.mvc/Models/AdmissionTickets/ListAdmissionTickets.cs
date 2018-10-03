using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Requests;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.AdmissionTickets
{
    public class ListAdmissionTickets : EcardModelListRequest<ListAdmissionTicket>
    {
        public ListAdmissionTickets()
        {
            OrderBy = "id asc";
        }
        private Bounded _stateBounded;
        public Bounded State
        {
            get
            {
                if (_stateBounded == null)
                {
                    _stateBounded = Bounded.Create<AdmissionTicket>("state", AdmissionTicketState.All);
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

        [NoRender, Dependency]
        public SecurityHelper SecurityHelper { get; set; }

        [NoRender, Dependency]
        public IAdmissionTicketService AdmissionTicketService { get; set; }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListAdmissionTicket item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.id });
            if (item.InnerObject.state == States.Normal)
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
            var request = new AdmissionTicketRequest();
            request.name = Name;
            if (!(request.state != AdmissionTicketState.All))
            {
                request.state = null;
            }
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
            var query = AdmissionTicketService.Query(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListAdmissionTicket(u));
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
            }
        }


        public List<ListAdmissionTicket> AjaxGet(AdmissionTicketRequest request, out string pageHtml)
        {
            List<ListAdmissionTicket> data = null;
            pageHtml = string.Empty;
            if (!(request.state != AdmissionTicketState.All))
            {
                request.state = null;
            }
            if (request.PageIndex == null || request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize == null || request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var query = AdmissionTicketService.Query(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListAdmissionTicket(u)).ToList();
                pageHtml = MvcPage.AjaxPager((int)request.PageIndex, (int)request.PageSize, query.TotalCount);
                foreach (var item in data)
                {



                    //if (item.InnerObject.State < AccountStates.Saled && item.InnerObject.State > 10 && this.SecurityHelper.HasPermission("account"))
                    //{

                    //    item.boor += "<a href='#' onclick=OperatorThis('SetDistributor','/Account/SetDistributor/" + item.AccountId + "') class='tablelink'>更改经销商 </a> ";
                    //}
                    if (this.SecurityHelper.HasPermission(Ecard.Permissions.AdmissionTicketEdit))
                        item.boor += "<a href='#' onclick=OperatorThis('Edit','/AdmissionTicket/Edit/" + item.id + "') class='tablelink'>编辑 </a> ";
                    if (item.InnerObject.state == AdmissionTicketState.Invalid)
                    {
                        if (this.SecurityHelper.HasPermission(Ecard.Permissions.AdmissionTicketResume))
                            item.boor += "<a href='#' onclick=OperatorThis('Resume','/AdmissionTicket/Resume/" + item.id + "') class='tablelink'>启用</a> ";
                    }
                    if (item.InnerObject.state == AdmissionTicketState.Normal)
                    {
                        if (this.SecurityHelper.HasPermission(Ecard.Permissions.AdmissionTicketSuspend))
                            item.boor += "<a href='#' onclick=OperatorThis('Suspend','/AdmissionTicket/Suspend/" + item.id + "') class='tablelink'>停售</a> ";
                    }
                    if (this.SecurityHelper.HasPermission(Ecard.Permissions.AdmissionTicketDelete))
                        item.boor += "<a href='#' onclick=OperatorThis('Delete','/AdmissionTicket/Delete/" + item.id + "') class='tablelink'>删除 </a> ";
                }
            }
            return data;
        }

        public SimpleAjaxResult Suspend(int id)
        {
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Normal)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Invalid;
                    AdmissionTicketService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketSuspend, serialNo, item.id, item.name);
                }
                return new SimpleAjaxResult(Localize("AdmissionTicketNoExisting"));
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketSuspend, ex);
                return new SimpleAjaxResult(ex.Message);
            }
           
        }


        public ResultMsg Suspends(int id)
        {
            ResultMsg msg = new ResultMsg();
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Normal)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Invalid;
                    AdmissionTicketService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketSuspend, serialNo, item.id, item.name);
                }
                msg.Code = 1;
                msg.CodeText = "停售门票成功";
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketSuspend, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }
            
        }

        public SimpleAjaxResult Resume(int id)
        {
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Invalid)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Normal;
                    AdmissionTicketService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketResume, serialNo, item.id, item.name);
                }
                return new SimpleAjaxResult(Localize("AdmissionTicketNoExisting"));
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketResume, ex);
                return new SimpleAjaxResult(ex.Message);
            }

        }

        public ResultMsg Resumes(int id)
        {
            ResultMsg msg = new ResultMsg();
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null && item.state == AdmissionTicketState.Invalid)
                {
                    var serialNo = SerialNoHelper.Create();
                    item.state = AdmissionTicketState.Normal;
                    AdmissionTicketService.Update(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketResume, serialNo, item.id, item.name);
                }
                msg.Code = 1;
                msg.CodeText = "启用门票成功";
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketResume, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }

        }

        public SimpleAjaxResult Delete(int id)
        {
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null)
                {
                    var serialNo = SerialNoHelper.Create();
                    AdmissionTicketService.Delete(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketDelete, serialNo, item.id, item.name);
                }
                return new SimpleAjaxResult(Localize("AdmissionTicketNoExisting"));
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketDelete, ex);
                return new SimpleAjaxResult(ex.Message);
            }

        }
        public ResultMsg Deletes(int id)
        {
            ResultMsg msg = new ResultMsg() { Code=-1,CodeText="删除失败"};
            try
            {
                var item = AdmissionTicketService.GetById(id);
                if (item != null)
                {
                    var serialNo = SerialNoHelper.Create();
                    AdmissionTicketService.Delete(item);
                    Logger.LogWithSerialNo(LogTypes.AdmissionTicketDelete, serialNo, item.id, item.name);
                    msg.Code = 1;
                    msg.CodeText = "删除门票" + item.name + "成功";
                }
                return msg;
            }
            catch (Exception ex)
            {
                Logger.Error(LogTypes.AdmissionTicketDelete, ex);
                msg.CodeText = "不好意思,系统异常";
                return msg;
            }

        }
    }
}
