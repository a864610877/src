using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit;

namespace Ecard.Mvc.Models.GoodandOrder
{
    public class CreateOrderDetial:OrderDetialModelBase
    {
        public CreateOrderDetial()
        {
            //this.State = GoodState.Normal;
        }
        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            base.OnSave(InnerObject);
            OrderService.AddOrderDetial(InnerObject);
            AddMessage("success", InnerObject.GoodId);
            Logger.LogWithSerialNo(LogTypes.AddOrder, serialNo, InnerObject.GoodId, InnerObject.OrderId);
            return TransactionHelper.CommitAndReturn(this);
        }

        public void Ready()
        {
            //List<IdNamePair> states = new List<IdNamePair>();
            //states.Add(new IdNamePair() { Key = GoodState.Normal, Name = "正常" });
            //states.Add(new IdNamePair() { Key = GoodState.Invalid, Name = "停用" });
            //State.Bind(states);
        }
    }
}
