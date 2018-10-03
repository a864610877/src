using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit;

namespace Ecard.Mvc.Models.GoodandOrder
{
    [Bind(Prefix = "item")]
    public class EditGood : GoodModelBase
    {
        public EditGood()
        { }
        public EditGood(Good item)
            : base(item)
        { }
        [Hidden]
        public int GoodId
        {
            get { return InnerObject.GoodId; }
            set { InnerObject.GoodId = value; }
        }
        public void Read(int id)
        {
            SetInnerObject(this.OrderService.GetById(id));
        }
        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = OrderService.GetById(GoodId);
            if (item == null)
            {
                AddError(LogTypes.EditGood, "nonFoundGood");
                return this;
            }
            TransactionHelper.BeginTransaction();
            OnSave(item);
            OrderService.UpdateGood(item);
            AddMessage("success", InnerObject.GoodName);
            Logger.LogWithSerialNo(LogTypes.EditGood, serialNo, InnerObject.GoodId, InnerObject.GoodName);
            return TransactionHelper.CommitAndReturn(this);
        }
        public void Ready()
        {
            List<IdNamePair> states = new List<IdNamePair>();
            states.Add(new IdNamePair() { Key = GoodState.Normal, Name = "正常" });
            states.Add(new IdNamePair() { Key = GoodState.Invalid, Name = "停用" });
            State.Bind(states);
        }
    }
}
