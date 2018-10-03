using System.Collections.Generic;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;

namespace Ecard.Mvc.Models.Commodities
{
    public class CreateCommodity : CommodityModelBase, IValidator
    {
        public CreateCommodity()
        {
            InnerObject.State = CommodityStates.Normal;
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            // InnerObject.AccountLevel = AccountLevel;
            InnerObject.State = PointRebateStates.Normal;
            CommodityService.Create(InnerObject);
            AddMessage("success", InnerObject.DisplayName);
            Logger.LogWithSerialNo(LogTypes.CommodityCreate,serialNo, InnerObject.CommodityId, InnerObject.DisplayName);

            return this;
        }

        public void Ready()
        {

        }

        public IEnumerable<ValidationError> Validate()
        {
            if(this.CommodityService.Query(new CommodityRequest(){Name = this.Name}).Count()> 0)
            {
                yield return new ValidationError("Name", string.Format(Localize("name.dumplicate"), Name));
            }
        }
    }
}