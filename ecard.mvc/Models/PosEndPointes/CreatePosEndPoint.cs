using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.PosEndPointes
{
    public class CreatePosEndPoint : PosEndPointModelBase, IValidator
    {
        private Bounded _shopBounded;

        [DataType(DataType.Password)] 
        [Required(ErrorMessage = "请输入通讯密钥")]
        [StringLength(16,ErrorMessage="密钥必须是16位")]
        public string DataKey
        {
            get { return InnerObject.DataKey; }
            set { InnerObject.DataKey = value; }
        }
        public Bounded Shop
        {
            get
            {
                if (_shopBounded == null)
                {
                    _shopBounded = Bounded.CreateEmpty("ShopId", InnerObject.ShopId);
                }
                return _shopBounded;
            }
            set { _shopBounded = value; }
        }
        [UIHint("PosName")]
        public string Name
        {
            get { return InnerObject.Name; }
            set { InnerObject.Name = value.TrimSafty(); }
        }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [NoRender, Dependency]
        public IPosEndPointService PosEndPointService { get; set; }

        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }

        #region IValidator Members

        public IEnumerable<ValidationError> Validate()
        {
            if (PosEndPointService.Query(new PosEndPointRequest {Name = Name, ShopId = this.Shop}).Count() > 0)
                yield return
                    new ValidationError("Name", string.Format(Localize("messages.duplicationPos"), InnerObject.Name));
        }

        #endregion

        public void Ready(int? shopId)
        {
            InnerObject = new PosEndPoint {ShopId = (shopId ?? 0)};
            var q = ShopService.Query(new ShopRequest()).Select(x => new IdNamePair {Key = x.ShopId, Name = x.DisplayName});
            Shop.Bind(q);
        }

        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            Shop shop = ShopService.GetById(Shop.Key);
            if (shop != null)
            {
                InnerObject.ShopId = shop.ShopId;
                InnerObject.State = States.Normal;
                PosEndPointService.Create(InnerObject);
                AddMessage("success", Name, shop.DisplayName);
                Logger.LogWithSerialNo(LogTypes.PosCreate, serialNo, InnerObject.PosEndPointId, Name, shop.DisplayName);
                CacheService.Refresh(CacheKeys.PosKey);
            }
            return this;
        }
    }
}