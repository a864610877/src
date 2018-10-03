using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PosEndPointes
{
    public class EditPosEndPoint : PosEndPointModelBase, IValidator
    {
        public EditPosEndPoint()
        {
            InnerShop = new Shop();
        }

        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "√‹‘ø±ÿ–Î «16Œª")]
        public string DataKey
        {
            get { return InnerObject.DataKey; }
            set { InnerObject.DataKey = value; }
        }
        [Hidden]
        public int PosEndPointId
        {
            get { return InnerObject.PosEndPointId; }
            set { InnerObject.PosEndPointId = value; }
        }

        public string PosName
        {
            get { return InnerObject.Name; }
        }

        public string ShopName
        {
            get { return InnerShop.Name; }
        }

        [NoRender, Dependency]
        public IPosEndPointService PosEndPointService { get; set; }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [Hidden]
        public int ShopId
        {
            get { return InnerShop.ShopId; }
        }

        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }

        #region IValidator Members

        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }

        #endregion

        public void Read(int id)
        {
            PosEndPoint item = PosEndPointService.GetById(id);
            if (item != null)
            {
                Shop shop = ShopService.GetById(item.ShopId);
                InnerShop = shop;
                base.InnerObject = item;
            }
        }

        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            PosEndPoint item = PosEndPointService.GetById(PosEndPointId);

            if (item != null)
            {
                var shop = ShopService.GetById(item.ShopId);
                item.DisplayName = DisplayName;
                if (!string.IsNullOrEmpty(DataKey))
                    item.DataKey = DataKey;
                PosEndPointService.Update(item);
                AddMessage("success", item.Name);
                Logger.LogWithSerialNo(LogTypes.PosEdit, serialNo, item.PosEndPointId, item.Name, shop.DisplayName);
                CacheService.Refresh(CacheKeys.PosKey);
            }
            return this;
        }
    }
}