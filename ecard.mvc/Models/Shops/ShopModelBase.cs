using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.Shops
{
    public class ShopModelBase : ViewModelBase
    {
        private Shop _innerObject;
        private ShopUser _owner;

        public ShopModelBase()
        {
            _innerObject = new Shop();
            _innerObject.State = States.Normal;
            _owner = new ShopUser();
            Owner.ShopRole = ShopRoles.Owner;
            Owner.State = States.Normal;
        }
        public void Ready()
        {
            this.Bank.Bind(HostSite.GetBanks());
            var idNamePairs = DealWayService.Query().Where(x => x.State == DealWayStates.Normal && new ApplyToModel(x.ApplyTo).EnabledShop).Select(x => new IdNamePair() { Key = x.DealWayId, Name = x.DisplayName });
            this.DealWay.Bind(idNamePairs);

            if (this._owner != null)
            {
                Mobile.Value1 = _owner.Mobile;
                Mobile.Value2 = _owner.Mobile2;
            }
            Mobile.HasBinding = !string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity);
            this.PhoneNumber.Value1 = InnerObject.PhoneNumber;
            this.PhoneNumber.Value2 = InnerObject.PhoneNumber2;
        }

        protected void SetInnerObject(Shop shop, ShopUser owner)
        {
            _owner = owner;
            _innerObject = shop;
        }

        private Mobiles _mobile;

        public Mobiles Mobile
        {
            get
            {
                if (_mobile == null)
                {
                    _mobile = new Mobiles();
                }
                return _mobile;
            }
            set { _mobile = value; }
        }
        [StringLength(30, ErrorMessage = "商户地址长度不可超过 30")]
        public string Address
        {
            get { return InnerObject.Address; }
            set { InnerObject.Address = value; }
        }

        [StringLength(20, ErrorMessage = "商户账户长度不可超过 30")]
        public string BankAccountName
        {
            get { return InnerObject.BankAccountName; }
            set { InnerObject.BankAccountName = value; }
        }


        [StringLength(20, ErrorMessage = "商户账户用户名长度不可超过 30")]
        public string BankUserName
        {
            get { return InnerObject.BankUserName; }
            set { InnerObject.BankUserName = value; }
        }

        [StringLength(10, ErrorMessage = "商户描述长度不可超过 10")]
        public string Description
        {
            get { return InnerObject.Description; }
            set { InnerObject.Description = value; }
        }
        [StringLength(10, ErrorMessage = "银行支行描述长度不可超过 100")]
        public string BankPoint
        {
            get { return InnerObject.BankPoint; }
            set { InnerObject.BankPoint = value; }
        }

        public decimal RechargeAmount { get { return InnerObject.RechargeAmount; } set { InnerObject.RechargeAmount = value; } }

        private Bounded _bankBounded;

        public Bounded Bank
        {
            get
            {
                if (_bankBounded == null)
                {
                    _bankBounded = Bounded.CreateEmpty("BankId", (this._innerObject.Bank ?? "").GetHashCode());
                }
                return _bankBounded;
            }
            set { _bankBounded = value; }
        }

        private PhoneNumbers _phoneNumber;
        //[RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "输入的必须是手机号码和电话号码")]
        public PhoneNumbers PhoneNumber
        {
            get
            {
                if (_phoneNumber == null)
                    _phoneNumber = new PhoneNumbers();
                return _phoneNumber;
            }
            set { _phoneNumber = value; }
        }

        [NoRender, Dependency]
        public IShopService ShopService { get; set; }

        [NoRender, Dependency]
        public RandomCodeHelper CodeHelper { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }
        [NoRender, Dependency]
        public IDealWayService DealWayService { get; set; }


        [NoRender]
        public Shop InnerObject
        {
            get { return _innerObject; }
        }
        //[RegularExpression(@"(?:^(?:[1-9][\d]?)(?:\.[\d]{1,2})?$)", ErrorMessage = "比例有误")]
        public decimal? ShopDealLogChargeRate
        {
            get { return InnerObject.ShopDealLogChargeRate.HasValue ? InnerObject.ShopDealLogChargeRate.Value * 100 : (decimal?)null; }
            set
            {
                InnerObject.ShopDealLogChargeRate = value;
                if (value.HasValue)
                    InnerObject.ShopDealLogChargeRate = value / 100;
            }
        }
         
        private Bounded _dealWayBounded;

        public Bounded DealWay
        {
            get
            {
                if (_dealWayBounded == null)
                {
                    _dealWayBounded = Bounded.CreateEmpty("DealWayId", InnerObject.DealWayId);
                }
                return _dealWayBounded;
            }
            set { _dealWayBounded = value; }
        }
        [Required(ErrorMessage = "请输入显示名称")]
        [StringLength(40)]
        public string DisplayName
        {
            get { return _innerObject.DisplayName; }
            set
            {
                _innerObject.DisplayName = value.TrimSafty();
                OnDisplayNameChanged(value.TrimSafty());
            }
        }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "两次密码必须一致")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [NoRender]
        internal ShopUser Owner
        {
            get { return _owner; }
        }

        protected virtual void OnDisplayNameChanged(string value)
        {
        }

        protected void OnSave(Shop shop, ShopUser owner)
        {
            if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity))
            {
                var code = CodeHelper.GetObject<string>("sms", false);
                var mobile = CodeHelper.GetObject<string>("sms_mobile", false);
                // 校验成功
                if (!string.IsNullOrWhiteSpace(code) && code == this.Mobile.Code)
                {
                    // 校验成功，并且提交号码和校验号码相同，则为绑定
                    if (this.Mobile.Value1 == mobile)
                    {
                        owner.Mobile = mobile;
                        owner.IsMobileAvailable = true;
                    }
                    else // 否则为取消绑定
                    {
                        owner.Mobile = "";
                        owner.IsMobileAvailable = false;
                    }
                }
            }
            else
            {
                owner.Mobile = this.Mobile.Value1;
                owner.IsMobileAvailable = !string.IsNullOrWhiteSpace(owner.Mobile);
            }
            owner.Mobile2 = Mobile.Value2;
            shop.Description = Description;
            shop.BankUserName = BankUserName;
            shop.BankAccountName = BankAccountName;
            shop.DealWayId = DealWay;
            shop.Mobile = owner.Mobile;
            shop.Mobile2 = owner.Mobile2;
            shop.PhoneNumber = PhoneNumber.Value1;
            shop.PhoneNumber2 = PhoneNumber.Value2;
            shop.Bank = HostSite.GetBank(this.Bank);
            shop.BankPoint = BankPoint;
            shop.Address = this.Address; 
        }
    }
}