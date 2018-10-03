using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Infrastructure;
using Moonlit;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class CreateGood : GoodModelBase
    {
        public CreateGood()
        {
            //this.State = GoodState.Normal;
        }
        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            base.OnSave(InnerObject);
            OrderService.CreateGood(InnerObject);
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
