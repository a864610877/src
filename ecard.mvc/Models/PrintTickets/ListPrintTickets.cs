using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PrintTickets
{
    public class ListPrintTickets : EcardModelListRequest<ListPrintTicket>
    {
        public ListPrintTickets()
        {
            OrderBy = "SubmitTime desc"; 
        }
        [UIHint("AccountName")]
        public string AccountName { get; set; }
        private Bounded _logTypeBounded;

        public Bounded LogType
        {
            get
            {
                if (_logTypeBounded == null)
                {
                    _logTypeBounded = Bounded.Create<PrintTicket>("LogType", Globals.All);
                }
                return _logTypeBounded;
            }
            set { _logTypeBounded = value; }
        }
        [Dependency, NoRender]
        public IPrintTicketService PrintTicketService { get; set; }
        [Dependency, NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }

        [NoRender, Dependency]
        public Site HostSite { get; set; }
        public string SerialNo { get; set; }
        public void Ready()
        {
            var ids = new List<int>
                          {
                              LogTypes.AccountOpen,
                              LogTypes.AccountRecharge,
                              LogTypes.AccountSuspend,
                              LogTypes.AccountResume,
                              LogTypes.AccountChangeName,
                              LogTypes.AccountTransfer,
                              LogTypes.AccountRenew,
                          };
            foreach (var idNamePair in LogType.Items.ToArray())
            {
                if (!ids.Contains(idNamePair.Key))
                    LogType.Items.Remove(idNamePair);
            }
            LogType.BindAll();
        }

        public void Query()
        {
            var request = new PrintTicketRequest();
            if (!string.IsNullOrWhiteSpace(AccountName))
                request.AccountName = AccountName;
            if (LogType != Globals.All)
                request.LogType = LogType;
            if (!string.IsNullOrWhiteSpace(SerialNo))
            {
                request.SerialNo = SerialNo;
            }
            var query = PrintTicketService.Query(request);

            this.List = query.ToList(this, x => new ListPrintTicket(x));
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPrintTicket item)
        {
            yield return new ActionMethodDescriptor("Print", null, new { id = item.PrintTicketId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.PrintTicketId });
        }
        public void Delete(int id)
        {
            var item = this.PrintTicketService.GetById(id);
            if (item != null)
            {
                PrintTicketService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.PrintTicketDelete, SerialNoHelper.Create(), id, item.AccountName, ModelHelper.GetBoundText(item, x => x.LogType));
                AddMessage("delete.success", item.AccountName, ModelHelper.GetBoundText(item, x => x.LogType));
            }
        }

        public object Print(int id)
        {
            try
            {
                var item = this.PrintTicketService.GetById(id);
                if (item != null)
                {
                    item.PrintCount++;
                    PrintTicketService.Update(item);
 
                    Logger.LogWithSerialNo(LogTypes.PrintTicketPrint, SerialNoHelper.Create(), id, item.AccountName, ModelHelper.GetBoundText(item, x => x.LogType));
                    AddMessage("print.success", item.AccountName, ModelHelper.GetBoundText(item, x => x.LogType));
                    return new DataAjaxResult() { Data1 = item.Content };
                }
                return new SimpleAjaxResult(Localize("nonTicket", "指定小票未找到"));
            }
            catch (System.Exception ex)
            {
                Logger.Error(LogTypes.PrintTicketPrint, ex);
                return new SimpleAjaxResult(ex.Message);
            }
        }
    }
}