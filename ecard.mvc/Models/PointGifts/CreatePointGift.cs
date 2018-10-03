using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.Models.PointRebates;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PointGifts
{
    public class CreatePointGift : PointGiftModelBase, IValidator
    {
        public CreatePointGift()
        {
            InnerObject.State = PointGiftStates.Normal;
        }
        public IMessageProvider Create()
        {
            var serialNo = SerialNoHelper.Create();
            InnerObject.State = PointGiftStates.Normal;
            HttpContext context = HttpContext.Current;
            if (Photo != null && Photo.File != null)
            {
                var name = Guid.NewGuid().ToString("N") + ".jpg";
                InnerObject.Photo = name;
                var fileName = context.Server.MapPath("~/content/pointgiftphotos/" + name);
                Moonlit.IO.DirectoryEnsure.EnsureFromFile(fileName);
                Photo.File.SaveAs(fileName);

            }

            TransactionHelper.BeginTransaction();
            base.OnSave(InnerObject);
            PointGiftService.Create(InnerObject);

            AddMessage("success", DisplayName);
            Logger.LogWithSerialNo(LogTypes.PointGiftCreate, serialNo, InnerObject.PointGiftId, DisplayName);

            return TransactionHelper.CommitAndReturn(this);
        }

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
    }
}