using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.PointRebates
{
    [Bind(Prefix = "Item")]
    public class EditPointRebate : PointRebateModelBase
    {
        [Hidden]
        public int PointRebateId
        {
            get { return InnerObject.PointRebateId; }
            set { InnerObject.PointRebateId = value; }
        }

        public void Read(int id)
        {
            var pointRebate = PointRebateService.GetById(id);
            this.SetInnerObject(pointRebate);
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = PointRebateService.GetById(PointRebateId);
            if (item != null)
            {
                TransactionHelper.BeginTransaction();
                item.DisplayName = DisplayName;
                item.Point = Point;

                item.Amount = Amount;
                item.DisplayName = DisplayName;
                OnSave(item);
                PointRebateService.Update(item);


                AddMessage("success", DisplayName);
                Logger.LogWithSerialNo(LogTypes.PointRebateEdit, serialNo, item.PointRebateId, DisplayName);

                TransactionHelper.Commit();
            }
        }

    }
}