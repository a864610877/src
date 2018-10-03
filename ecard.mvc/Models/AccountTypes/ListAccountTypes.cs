using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AccountTypes
{
    public class ListAccountTypes : EcardModelListRequest<ListAccountType>
    {
        public ListAccountTypes()
        {
            OrderBy = "AccountTypeId";
        }
        [Dependency]
        [NoRender]
        public IAccountTypeService AccountTypeService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }

        public void Ready()
        {
        }

        public void Delete(int id)
        {
            var item = this.AccountTypeService.GetById(id);
            if (item != null)
            {
                if (AccountService.Query(new AccountRequest() { AccountTypeId = item.AccountTypeId}).Count() > 0)
                {
                    AddError("delete.existAccount");
                }
                else
                {
                    AccountTypeService.Delete(item);
                    Logger.LogWithSerialNo(LogTypes.AccountTypeDelete, SerialNoHelper.Create(), item.AccountTypeId, item.DisplayName);
                    AddMessage("delete.success", item.DisplayName);
                }
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }

        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }
        public void Query()
        {
            AccountTypeRequest request = new AccountTypeRequest();
            if (State != AccountTypeStates.All)
                request.State = State;
            var query = AccountTypeService.Query(request);

            this.List = query.ToList(this, x => new ListAccountType(x));
        }

        public void Suspend(int id)
        {
            var item = this.AccountTypeService.GetById(id);
            if (item != null && item.State == AccountTypeStates.Normal)
            {
                item.State = AccountTypeStates.Invalid;
                AccountTypeService.Update(item);

                Logger.LogWithSerialNo(LogTypes.AccountTypeSuspend, SerialNoHelper.Create(), item.AccountTypeId, item.DisplayName);
                AddMessage("suspend.success", item.DisplayName);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }

        public void Resume(int id)
        {
            var item = this.AccountTypeService.GetById(id);
            if (item != null && item.State == AccountTypeStates.Invalid)
            {
                item.State = AccountTypeStates.Normal;
                AccountTypeService.Update(item);

                Logger.LogWithSerialNo(LogTypes.AccountTypeResume, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("resume.success", item.DisplayName);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            //yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            //yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListAccountType item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.AccountTypeId });
            if (item.InnerObject.State == PointPolicyStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.AccountTypeId });
            if (item.InnerObject.State == PointPolicyStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.AccountTypeId });
            //yield return new ActionMethodDescriptor("Delete", null, new { id = item.AccountTypeId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<AccountType>("State", AccountTypeStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}