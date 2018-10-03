using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PointGifts
{
    [Bind(Prefix = "Item")]
    public class EditPointGift : PointGiftModelBase
    {
        [Hidden]
        public int PointGiftId
        {
            get { return InnerObject.PointGiftId; }
            set { InnerObject.PointGiftId = value; }
        }

        public void Read(int id)
        {
            var pointGift = PointGiftService.GetById(id);
            Photo = new Picture("~/content/pointgiftphotos/{0}", pointGift.Photo, 100, 100);
            this.SetInnerObject(pointGift);
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = PointGiftService.GetById(PointGiftId);
            if (item != null)
            {
                TransactionHelper.BeginTransaction();
                item.DisplayName = DisplayName;
                item.Point = Point;
                HttpContext contxt = HttpContext.Current;
                string oldFileName = "";
                if (Photo != null && Photo.File != null)
                {
                    var name = Guid.NewGuid().ToString("N") + ".jpg";
                    oldFileName = item.Photo;
                    item.Photo = name;
                    var fileName = contxt.Server.MapPath("~/content/pointgiftphotos/" + name);
                    Moonlit.IO.DirectoryEnsure.EnsureFromFile(fileName);
                    Photo.File.SaveAs(fileName);
                }

                item.Description = Description;
                item.Category = Category;
                OnSave(item);
                PointGiftService.Update(item);


                AddMessage("success", DisplayName);
                Logger.LogWithSerialNo(LogTypes.PointGiftEdit, serialNo, item.PointGiftId, DisplayName);

                 
                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    try
                    {
                        File.Delete(contxt.Server.MapPath("~/content/pointgiftphotos/" + oldFileName));
                    }
                    catch(Exception ex)
                    {
                        Logger.Error(LogTypes.PointGiftEdit, ex);
                    }
                }
                TransactionHelper.Commit();
            }
        }

    }
}