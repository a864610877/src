using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit;

namespace Ecard.Mvc.Models.AmountRates
{
    public class CreateAmountRate : AmountRateModelBase
    {
        public CreateAmountRate()
        {
            InnerObject.State = AmountRateStates.Normal;
        }

        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            InnerObject.AccountLevel = AccountLevel;
            InnerObject.State = AmountRateStates.Normal;
            AmountRateService.Create(InnerObject);
            AddMessage("success", DisplayName);
            Logger.LogWithSerialNo(LogTypes.AmountRateCreate, serialNo, InnerObject.AmountRateId, DisplayName);
            CacheService.Refresh(CacheKeys.AmountRateKey);
            return this;
        }

        public void Ready()
        {
            var levels = AccountLevelService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal).ToList();
            this.AccountLevel.Bind(levels.Select(x => new IdNamePair { Key = x.AccountLevelPolicyId, Name = x.DisplayName }));
        }
    }
}
