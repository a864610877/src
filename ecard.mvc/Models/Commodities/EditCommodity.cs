using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.Commodities
{
    [Bind(Prefix = "Item")]
    public class EditCommodity : CommodityModelBase
    {
        public EditCommodity()
        {
        }

        public EditCommodity(Commodity item)
            : base(item)
        {
        } 
        [Hidden]
        public int CommodityId
        {
            get { return InnerObject.CommodityId; }
            set { InnerObject.CommodityId = value; }
        }

        public void Read(int id)
        {
            this.SetInnerObject(CommodityService.GetById(id));
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = CommodityService.GetById(CommodityId);
            if (item != null)
            {
                item.DisplayName = DisplayName;
                item.Name = Name;
                item.Price = Price;
                CommodityService.Update(item);
                AddMessage("success", item.DisplayName);
                Logger.LogWithSerialNo(LogTypes.CommodityEdit, serialNo,item.CommodityId, item.DisplayName);
            }
        }

        public void Ready()
        {

        }
    }
}