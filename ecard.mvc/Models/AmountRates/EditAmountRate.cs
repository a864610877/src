using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.AmountRates
{
    [Bind(Prefix = "Item")]
    public class EditAmountRate : AmountRateModelBase
    {
        [Hidden]
        public int AmountRateId
        {
            get { return InnerObject.AmountRateId; }
            set { InnerObject.AmountRateId = value; }
        }

        public void Read(int id)
        {
            this.SetInnerObject(AmountRateService.GetById(id));
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = AmountRateService.GetById(AmountRateId);
            if (item != null)
            {
                item.DisplayName = DisplayName;
                item.AccountLevel = AccountLevel;
                item.Amount = InnerObject.Amount;
                item.Rate = InnerObject.Rate;
                item.Days = InnerObject.Days;
                AmountRateService.Update(item);

                AddMessage("success", DisplayName);
                Logger.LogWithSerialNo(LogTypes.AmountRateEdit, serialNo, item.AmountRateId, DisplayName);
                CacheService.Refresh(CacheKeys.AmountRateKey);
            }
        }

        public void Ready()
        {
            var query = AccountLevelService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal).ToList()
                .Select(x => new IdNamePair { Key = x.AccountLevelPolicyId, Name = x.DisplayName });

            this.AccountLevel.Bind(query);
        }
    }
}