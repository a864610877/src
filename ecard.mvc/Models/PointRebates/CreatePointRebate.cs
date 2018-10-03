using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PointRebates
{

    public class CreatePointRebate : PointRebateModelBase, IValidator
    {
        public CreatePointRebate()
        {
            InnerObject.State = PointRebateStates.Normal;
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            InnerObject.State = PointRebateStates.Normal;

            TransactionHelper.BeginTransaction();
            OnSave(InnerObject);
            PointRebateService.Create(InnerObject);

            AddMessage("success", DisplayName);
            Logger.LogWithSerialNo(LogTypes.PointRebateCreate, serialNo, InnerObject.PointRebateId, DisplayName);

            return TransactionHelper.CommitAndReturn(this);
        }

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
    }
}