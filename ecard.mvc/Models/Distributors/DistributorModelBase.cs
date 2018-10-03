using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using System;
using Ecard.Mvc.ViewModels;

namespace Ecard.Mvc.Models.Distributors
{
    /// <summary>
    /// 经销商model基类
    /// </summary>
    public class DistributorModelBase : ViewModelBase
    {
        private Distributor _innerObject;
        private Mobiles _mobile;
        private DistributorUser _owner;
        private PhoneNumbers _phoneNumber;

        public DistributorModelBase()
        {
            _innerObject = new Distributor();
            _innerObject.State = States.Normal;
            _owner = new DistributorUser();
            Owner.State = States.Normal;
        }
        //---
        //[UIHint("DistributorAccountLevelRate")]
        //public List<DistributorAccountLevelRate> AccountLevelPolicyRates { get; set; }
        //--
        [NoRender]
        public DistributorAccountLevelRate AccountLevelPolicyRates { get; set; }

        //[Remote("CheckPolicyRate", "Distributor", ErrorMessage = "数值只能在0-100之间，且不能大于上级的提成比例")]
        [RegularExpression(@"^\d{1,2}(\.\d{1,2})?",ErrorMessage="两位小数，谢谢")]
        public decimal PolicyRate
        {
            get
            {
                if (AccountLevelPolicyRates == null)
                {
                    AccountLevelPolicyRates = new DistributorAccountLevelRate() { AccountLevelPolicyId=1,Rate=0, AccountLevelPolicyText="1" };
                }
                return AccountLevelPolicyRates.Rate;
            }
            set
            {
                if (AccountLevelPolicyRates == null)
                {
                    AccountLevelPolicyRates = new DistributorAccountLevelRate() { AccountLevelPolicyId = 1, Rate = value, AccountLevelPolicyText = "1" };
                }
                else
                {
                    AccountLevelPolicyRates.Rate = value;
                }

            }
        }

        //---------------
        /// <summary>
        /// 手机
        /// </summary>
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

        [StringLength(30, ErrorMessage = "经销商地址长度不可超过 30")]
        public string Address
        {
            get { return InnerObject.Address; }
            set { InnerObject.Address = value; }
        }
        //----------------
        [StringLength(10, ErrorMessage = "商户描述长度不可超过 10")]
        public string Description
        {
            get { return InnerObject.Description; }
            set { InnerObject.Description = value; }
        }
        [StringLength(20, ErrorMessage = "银行账户长度不可超过 20")]
        public string BankAccountName
        {
            get { return InnerObject.BankAccountName; }
            set { InnerObject.BankAccountName = value; }
        }
        [StringLength(20, ErrorMessage = "经销商账户用户名长度不可超过 20")]
        public string BankUserName
        {
            get { return InnerObject.BankUserName; }
            set { InnerObject.BankUserName = value; }
        }

        [StringLength(100, ErrorMessage = "银行支行描述长度不可超过 100")]
        public string BankPoint
        {
            get { return InnerObject.BankPoint; }
            set { InnerObject.BankPoint = value; }
        }

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
        //支付方式
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
        private Bounded _distributorLevel;
        public Bounded DistributorLevel
        {
            get
            {
                if (_distributorLevel == null)
                {
                    _distributorLevel = Bounded.CreateEmpty("DistributorLevel", InnerObject.DistributorLevel);
                }
                return _distributorLevel;
            }
            set { _distributorLevel = value; }
        }
        //----------------
       


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
        public IDistributorService DistributorService { get; set; }
        [NoRender, Dependency]
        public IAccountLevelPolicyService AccountLevelPolicyService { get; set; }

        [NoRender, Dependency]
        public RandomCodeHelper CodeHelper { get; set; }

        [NoRender, Dependency]
        public IMembershipService MembershipService { get; set; }

        [NoRender, Dependency]
        public IDealWayService DealWayService { get; set; }


        [NoRender]
        public Distributor InnerObject
        {
            get { return _innerObject; }
        }

        [Required(ErrorMessage = "请输入显示名称")]
        [StringLength(40)]
        public string DisplayName
        {
            get { return Owner.DisplayName; }
            set
            {
                Owner.DisplayName = value.TrimSafty();
                OnDisplayNameChanged(value.TrimSafty());
            }
        }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "两次密码必须一致")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [NoRender]
        internal DistributorUser Owner
        {
            get { return _owner; }
        }

        public void Ready()
        {
            //支付方式
            var idNamePairs = DealWayService.Query().Where(x => x.State == DealWayStates.Normal ).Select(x => new IdNamePair() { Key = x.DealWayId, Name = x.DisplayName });
            this.DealWay.Bind(idNamePairs);
            //银行
            this.Bank.Bind(HostSite.GetBanks());
            //经销商级别
            var user = this.SecurityHelper.GetCurrentUser();
            if (user is AdminUserModel)
            {
                List<IdNamePair> li = new List<IdNamePair>() { new IdNamePair() { Key = 1, Name = "总经销商" } };
                this.DistributorLevel.Items = li;
            }
            else if (user is DistributorUserModel)
            {
                List<IdNamePair> li = new List<IdNamePair>() { new IdNamePair() { Key = 2, Name = "分经销商" } };
                this.DistributorLevel.Items = li;
            }
            
            
            if (_owner != null)
            {
                Mobile.Value1 = _owner.Mobile;
                Mobile.Value2 = _owner.Mobile2;
            }
            Mobile.HasBinding = !string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity);
            PhoneNumber.Value1 = Owner.PhoneNumber;
            PhoneNumber.Value2 = Owner.PhoneNumber2;
            List<Distributor> distributors =
                DistributorService.Query().Where(x => x.DistributorId != InnerObject.DistributorId).ToList();
            List<User> users = MembershipService.GetByIds(distributors.Select(x => x.UserId).ToArray()).ToList();
            List<Ecard.Models.DistributorAccountLevelRate> rateDtos = this.DistributorService.GetAccountLevelPolicyRates(this.InnerObject.DistributorId);
            if (rateDtos.Count > 0)
            {
                this.AccountLevelPolicyRates = new DistributorAccountLevelRate()
                {
                    AccountLevelPolicyId = rateDtos[0].AccountLevelPolicyId,
                    Rate = rateDtos[0].Rate * 100
                };
            }
            else
            {
                this.AccountLevelPolicyRates = new DistributorAccountLevelRate()
                {
                    AccountLevelPolicyId =1,
                    Rate =0

                };
            }
/*
            List<Ecard.Models.DistributorAccountLevelRate> rateDtos = this.DistributorService.GetAccountLevelPolicyRates(this.InnerObject.DistributorId);
            var rates = AccountLevelPolicyService.Query().Where(x => x.State == AccountLevelPolicyStates.Normal).ToList();
            this.AccountLevelPolicyRates = (from x in rates
                                            let rateDto = rateDtos.FirstOrDefault(d => d.DistributorId == InnerObject.DistributorId && d.AccountLevelPolicyId == x.AccountLevelPolicyId)
                                            select new DistributorAccountLevelRate
                                                       {
                                                           AccountLevelPolicyId = x.AccountLevelPolicyId,
                                                           Rate = rateDto == null ? 0 : rateDto.Rate * 100,
                                                           AccountLevelPolicyText = x.DisplayName,
                                                       }).FirstOrDefault();
 * */
        }

        protected void SetInnerObject(Distributor distributor, DistributorUser owner)
        {
            _owner = owner;
            _innerObject = distributor;
        }

        protected virtual void OnDisplayNameChanged(string value)
        {
        }
        /// <summary>
        /// 保存时短信验证绑定
        /// </summary>
        /// <param name="distributor"></param>
        /// <param name="owner"></param>
        protected void OnSave(Distributor distributor, DistributorUser owner)
        {
            if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity))
            {
                var code = CodeHelper.GetObject<string>("sms", false);
                var mobile = CodeHelper.GetObject<string>("sms_mobile", false);
                // 校验成功
                if (!string.IsNullOrWhiteSpace(code) && code == Mobile.Code)
                {
                    // 校验成功，并且提交号码和校验号码相同，则为绑定
                    if (Mobile.Value1 == mobile)
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
                owner.Mobile = Mobile.Value1;
                owner.IsMobileAvailable = !string.IsNullOrWhiteSpace(owner.Mobile);
            }
            
            owner.Mobile2 = Mobile.Value2;
            owner.PhoneNumber = PhoneNumber.Value1;
            owner.PhoneNumber2 = PhoneNumber.Value2;
            distributor.Address = Address;
            distributor.Bank = HostSite.GetBank(this.Bank);
            distributor.BankAccountName = BankAccountName;
            distributor.BankPoint = BankPoint;
            distributor.BankUserName = BankUserName;
            distributor.DealWayId = DealWay;
            distributor.Description = Description;
            
            //----提成比例;
            //----
        }

        protected void UpdateAccountLevelPolicy(Distributor distributor)
        {
            //var rateas = from x in this.AccountLevelPolicyRates
            //            select new Ecard.Models.DistributorAccountLevelRate
            //                       {
            //                           AccountLevelPolicyId = x.AccountLevelPolicyId,
            //                           DistributorId = distributor.DistributorId,
            //                           Rate = x.Rate / 100
            //                       };
            var rates = new Ecard.Models.DistributorAccountLevelRate()
            {
                AccountLevelPolicyId = AccountLevelPolicyRates.AccountLevelPolicyId,
                DistributorId = distributor.DistributorId,
                Rate = AccountLevelPolicyRates.Rate / 100
            };
            DistributorService.UpdateAccountLevelPolicy(distributor.DistributorId, rates);
        }
    }
}