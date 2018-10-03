using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.PointGifts
{
    public class ListPointGifts : EcardModelListRequest<ListPointGift>
    {
        public ListPointGifts()
        {
            OrderBy = "PointGiftId";
        }

        //private string _name;
        //public string Name
        //{
        //    get { return _name.TrimSafty(); }
        //    set { _name = value; }
        //}

        [Dependency]
        [NoRender]
        public IPointGiftService PointGiftService { get; set; }
        [Dependency]
        [NoRender]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        public void Ready()
        {
            var query = from x in AccountLevelPolicyService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal)
                        orderby x.AccountTypeId, x.Level
                        select new IdNamePair { Key = x.AccountLevelPolicyId, Name = x.DisplayName };
            this.AccountLevel.Bind(query, true);
        }

        public void Query()
        {
            var query = PointGiftService.Query();
            if (State != States.All)
                query = query.Where(x => x.State == State);
            if (AccountLevel != Globals.All)
                query = query.Where(x => x.IncludeLevel(AccountLevel));
            this.List = query.ToList(this, x => new ListPointGift(x));
        }

        private Bounded _accountLevelBounded;

        public Bounded AccountLevel
        {
            get
            {
                if (_accountLevelBounded == null)
                {
                    _accountLevelBounded = Bounded.CreateEmpty("AccountLevelId", Globals.All);
                }
                return _accountLevelBounded;
            }
            set { _accountLevelBounded = value; }
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);
        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPointGift item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.PointGiftId });
            if (item.InnerObject.State == PointPolicyStates.Normal)
                yield return new ActionMethodDescriptor("Suspend", null, new { id = item.PointGiftId });
            if (item.InnerObject.State == PointPolicyStates.Invalid)
                yield return new ActionMethodDescriptor("Resume", null, new { id = item.PointGiftId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.PointGiftId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<PointGift>("State", PointGiftStates.Normal);
                }
                return _state;
            }
            set { _state = value; }
        }

        public void Delete(int id)
        {
            var item = this.PointGiftService.GetById(id);
            if (item != null)
            {
                PointGiftService.Delete(item);

                Logger.LogWithSerialNo(LogTypes.PointGiftDelete, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("delete.success", item.DisplayName);
            }
        }

        public void Suspend(int id)
        {
            var item = this.PointGiftService.GetById(id);
            if (item != null && item.State == PointGiftStates.Normal)
            {
                item.State = PointGiftStates.Invalid;
                PointGiftService.Update(item);

                Logger.LogWithSerialNo(LogTypes.PointGiftSuspend, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("suspend.success", item.DisplayName);
            }
        }

        public void Resume(int id)
        {
            var item = this.PointGiftService.GetById(id);
            if (item != null && item.State == PointGiftStates.Invalid)
            {
                item.State = PointGiftStates.Normal;
                PointGiftService.Update(item);

                Logger.LogWithSerialNo(LogTypes.PointGiftResume, SerialNoHelper.Create(), id, item.DisplayName);
                AddMessage("resume.success", item.DisplayName);
            }
        }
    }
}