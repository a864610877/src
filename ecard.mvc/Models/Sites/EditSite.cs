using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.Sites
{
    public class EditSite
    {
        [Dependency, NoRender]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency, NoRender]
        public ISiteService SiteService { get; set; }
        [Dependency, NoRender]
        public ICacheService CacheService { get; set; }

        [Dependency, NoRender]
        public Site Site { get; set; }
        Site GetSite()
        {
            return Site ?? InnerDto;
        }
        [NoRender]
        public Site InnerDto { get; set; }

        public EditSite()
        {
            InnerDto = new Site();
        }

        public string Name
        {
            get { return GetSite().Name; }
            set { InnerDto.Name = value; }
        }
        public string CopyRight
        {
            get { return GetSite().CopyRight; }
            set { InnerDto.CopyRight = value; }
        } 
        public string Description
        {
            get { return GetSite().Description; }
            set { InnerDto.Description = value; }
        }
         
        public string AccountToken
        {
            get { return GetSite().AccountToken; }
            set { InnerDto.AccountToken = value; }
        }

        //public int CancelSystemDealTime
        //{
        //    get { return GetSite().TimeOutOfCancelSystemDeal; }
        //    set { InnerDto.TimeOutOfCancelSystemDeal = value; }
        //}     
         
         
        public string DisplayName
        {
            get { return GetSite().DisplayName; }
            set { InnerDto.DisplayName = value; }
        }

        //[UIHint("richtext")]
        //public string Banks
        //{
        //    get { return GetSite().Banks; }
        //    set { InnerDto.Banks = value; }
        //}
        //[UIHint("richtext")]
        //[StringLength(2000, ErrorMessage = "\"" + "{0}" + "\"最大长度为{1}")]
        //[Display(Name = "支付渠道")]
        //public string HowToDeals
        //{
        //    get { return GetSite().HowToDeals; }
        //    set { InnerDto.HowToDeals = value; }
        //}
        //[Range(0, 100)]
        //public decimal ShopDealLogChargeRate
        //{
        //    get { return GetSite().ShopDealLogChargeRate * 100; }
        //    set { InnerDto.ShopDealLogChargeRate = value / 100m; }
        //}
        //public decimal? SaleCardFee
        //{
        //    get { return GetSite().SaleCardFee; }
        //    set { InnerDto.SaleCardFee = value; }
        //}

        //public bool IsRechargingApprove
        //{
        //    get { return GetSite().IsRechargingApprove; }
        //    set { InnerDto.IsRechargingApprove = value; }
        //}
        //public bool IsLimiteAmountApprove
        //{
        //    get { return GetSite().IsLimiteAmountApprove; }
        //    set { InnerDto.IsLimiteAmountApprove = value; }
        //}

        //public decimal? ChangeCardFee
        //{
        //    get { return GetSite().ChangeCardFee; }
        //    set { InnerDto.ChangeCardFee = value; }
        //}

        //private Bounded _printerTypeBounded;

        //public Bounded PrinterType
        //{
        //    get
        //    {
        //        if (_printerTypeBounded == null)
        //        {
        //            _printerTypeBounded = Bounded.CreateEmpty("PrinterTypeId", (GetSite().PrinterType ?? "").GetHashCode());
        //        }
        //        return _printerTypeBounded;
        //    }
        //    set { _printerTypeBounded = value; }
        //}
        //private Bounded _posTypeBounded;

        //public Bounded PosType
        //{
        //    get
        //    {
        //        if (_posTypeBounded == null)
        //        {
        //            _posTypeBounded = Bounded.CreateEmpty("PosTypeId", (GetSite().PosType ?? "").GetHashCode());
        //        }
        //        return _posTypeBounded;
        //    }
        //    set { _posTypeBounded = value; }
        //}

        //private Bounded _passwordTypeBounded;

        //public Bounded PasswordType
        //{
        //    get
        //    {
        //        if (_passwordTypeBounded == null)
        //        {
        //            _passwordTypeBounded = Bounded.CreateEmpty("PasswordTypeId", (GetSite().PasswordType ?? "").GetHashCode());
        //        }
        //        return _passwordTypeBounded;
        //    }
        //    set { _passwordTypeBounded = value; }
        //}
        //private Bounded _authTypeBounded;

        //public Bounded AuthType
        //{
        //    get
        //    {
        //        if (_authTypeBounded == null)
        //        {
        //            _authTypeBounded = Bounded.CreateEmpty("AuthTypeId", (GetSite().AuthType ?? "").GetHashCode());
        //        }
        //        return _authTypeBounded;
        //    }
        //    set { _authTypeBounded = value; }
        //}
        public void Ready()
        {
            //var postTypes = GetPosTypes();
            //var idNamePairs = postTypes.Select(x => new IdNamePair { Key = x.GetHashCode(), Name = x });
            //this.PosType.Bind(idNamePairs);

            //var passTypes = GetPasswordTypes();
            //idNamePairs = passTypes.Select(x => new IdNamePair { Key = x.GetHashCode(), Name = UnityContainer.Resolve<IPasswordService>(x).Name });
            //this.PasswordType.Bind(idNamePairs);

            //var printerTypes = GetPrinterTypes();
            //idNamePairs = printerTypes.Select(x => new IdNamePair { Key = x.GetHashCode(), Name = UnityContainer.Resolve<IPrinterService>(x).Name });
            //this.PrinterType.Bind(idNamePairs);

            //var authTypes = GetAuthTypes();
            //idNamePairs = authTypes.Select(x => new IdNamePair { Key = x.GetHashCode(), Name = UnityContainer.Resolve<IAuthenticateService>(x).Name });
            //this.AuthType.Bind(idNamePairs);
        }

        private static string[] GetPosTypes()
        {
            return SiteViewModel.GetPosTypes();
        }

        private static string[] GetPasswordTypes()
        {
            return new[]
                       {
                           "none", 
            // delete for publish source code
                           "sle902r"
                       };
        }

        private static string[] GetPrinterTypes()
        {
            return SiteViewModel.GetPrinterTypes();
        }
        private static string[] GetAuthTypes()
        {
            return new[] { "password", "ikeyandpassword" };
        }

        public void Save()
        {
            Site.Name = InnerDto.Name;
            Site.DisplayName = InnerDto.DisplayName;
            Site.ShopDealLogChargeRate = InnerDto.ShopDealLogChargeRate;
            Site.Description = InnerDto.Description;
            Site.SaleCardFee = InnerDto.SaleCardFee;
            Site.ChangeCardFee = InnerDto.ChangeCardFee;
            Site.TimeOutOfCancelSystemDeal = InnerDto.TimeOutOfCancelSystemDeal;
            //Site.IsEditMobileWithBinding = InnerDto.IsEditMobileWithBinding;
            Site.MessageTemplateOfDeal = InnerDto.MessageTemplateOfDeal;
            Site.MessageTemplateOfBirthDate = InnerDto.MessageTemplateOfBirthDate;
            Site.MessageTemplateOfOpenReceipt = InnerDto.MessageTemplateOfOpenReceipt;
            Site.MessageTemplateOfRecharge = InnerDto.MessageTemplateOfRecharge;
            Site.MessageTemplateOfUnIdentity = InnerDto.MessageTemplateOfUnIdentity;
            //Site.IsLimiteAmountApprove = InnerDto.IsLimiteAmountApprove;
            //Site.IsRechargingApprove = InnerDto.IsRechargingApprove;
           // Site.HowToDeals = InnerDto.HowToDeals;
            Site.CopyRight = InnerDto.CopyRight;
            Site.Banks = InnerDto.Banks;
            Site.AccountToken = InnerDto.AccountToken;
            //Site.PosType = GetPosTypes().FirstOrDefault(x => x.GetHashCode() == this.PosType.Key);
            //Site.PasswordType = GetPasswordTypes().FirstOrDefault(x => x.GetHashCode() == this.PasswordType.Key);
            //Site.PrinterType = GetPrinterTypes().FirstOrDefault(x => x.GetHashCode() == this.PrinterType.Key);
            //Site.AuthType = GetAuthTypes().FirstOrDefault(x => x.GetHashCode() == this.AuthType.Key); 
            SiteService.Update(Site);
            CacheService.Refresh(CacheKeys.SiteKey);
        }
    }
}