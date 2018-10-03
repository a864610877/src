using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard.Mvc.Models.DealWays
{
    public class CreateDealWay : DealWayModelBase
    {
        public CreateDealWay()
        {
            InnerObject.State = DealWayStates.Normal;
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            TransactionHelper.BeginTransaction();
            base.OnSave(InnerObject);
            DealWayService.Create(InnerObject);
            AddMessage("success", InnerObject.DisplayName);
            Logger.LogWithSerialNo(LogTypes.DealWayCreate, serialNo, InnerObject.DealWayId, InnerObject.DisplayName);
            return TransactionHelper.CommitAndReturn(this);
        }

        public void Ready()
        {

        }
    }
}