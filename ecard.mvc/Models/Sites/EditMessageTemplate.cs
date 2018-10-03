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
    public class EditMessageTemplate
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

        public EditMessageTemplate()
        {
            InnerDto = new Site();
        }
        [UIHint("richtext")]
        public string MessageTemplateOfAccountRenew
        {
            get { return GetSite().MessageTemplateOfAccountRenew; }
            set { InnerDto.MessageTemplateOfAccountRenew = value; }
        }
        [UIHint("richtext")]
        public string MessageTemplateOfAccountChangeName
        {
            get { return GetSite().MessageTemplateOfAccountChangeName; }
            set { InnerDto.MessageTemplateOfAccountChangeName = value; }
        }


        [UIHint("richtext")]
        public string MessageTemplateOfAccountTransfer
        {
            get { return GetSite().MessageTemplateOfAccountTransfer; }
            set { InnerDto.MessageTemplateOfAccountTransfer = value; }
        }
        [UIHint("richtext")]
        public string MessageTemplateOfDealCode
        {
            get { return GetSite().MessageTemplateOfDealCode; }
            set { InnerDto.MessageTemplateOfDealCode = value; }
        }

        [UIHint("richtext")]
        public string MessageTemplateOfAccountSuspend
        {
            get { return GetSite().MessageTemplateOfAccountSuspend; }
            set { InnerDto.MessageTemplateOfAccountSuspend = value; }
        }


        [UIHint("richtext")]
        public string MessageTemplateOfAccountResume
        {
            get { return GetSite().MessageTemplateOfAccountResume; }
            set { InnerDto.MessageTemplateOfAccountResume = value; }
        }
         
        [UIHint("richtext")]
        public string MessageTemplateOfDeal
        {
            get { return GetSite().MessageTemplateOfDeal; }
            set { InnerDto.MessageTemplateOfDeal = value; }
        }
        [UIHint("richtext")]
        public string MessageTemplateOfShopDeal
        {
            get { return GetSite().MessageTemplateOfShopDeal; }
            set { InnerDto.MessageTemplateOfShopDeal = value; }
        }     
        [UIHint("richtext")]
        public string MessageTemplateOfPrePay
        {
            get { return GetSite().MessageTemplateOfPrePay; }
            set { InnerDto.MessageTemplateOfPrePay = value; }
        }     
        [UIHint("richtext")]
        public string MessageTemplateOfRecharge
        {
            get { return GetSite().MessageTemplateOfRecharge; }
            set { InnerDto.MessageTemplateOfRecharge = value; }
        }     
        [UIHint("richtext")]
        public string MessageTemplateOfBirthDate
        {
            get { return GetSite().MessageTemplateOfBirthDate; }
            set { InnerDto.MessageTemplateOfBirthDate = value; }
        }
        [UIHint("richtext")]
        public string MessageTemplateOfOpenReceipt
        {
            get { return GetSite().MessageTemplateOfOpenReceipt; }
            set { InnerDto.MessageTemplateOfOpenReceipt = value; }
        } 

        [UIHint("richtext")]
        public string MessageTemplateOfIdentity
        {
            get { return GetSite().MessageTemplateOfIdentity; }
            set { InnerDto.MessageTemplateOfIdentity = value; }
        }
        [UIHint("richtext")]
        public string MessageTemplateOfDonePrePay
        {
            get { return GetSite().MessageTemplateOfDonePrePay; }
            set { InnerDto.MessageTemplateOfDonePrePay = value; }
        }

        [UIHint("richtext")]
        public string MessageTemplateOfUnIdentity
        {
            get { return GetSite().MessageTemplateOfUnIdentity; }
            set { InnerDto.MessageTemplateOfUnIdentity = value; }
        }
           
        public void Ready()
        {
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
            Site.MessageTemplateOfDeal = InnerDto.MessageTemplateOfDeal;
            Site.MessageTemplateOfPrePay = InnerDto.MessageTemplateOfPrePay;
            Site.MessageTemplateOfBirthDate = InnerDto.MessageTemplateOfBirthDate;
            Site.MessageTemplateOfOpenReceipt = InnerDto.MessageTemplateOfOpenReceipt;
            Site.MessageTemplateOfRecharge = InnerDto.MessageTemplateOfRecharge;
            Site.MessageTemplateOfUnIdentity = InnerDto.MessageTemplateOfUnIdentity;
            Site.MessageTemplateOfIdentity = InnerDto.MessageTemplateOfIdentity;
            Site.MessageTemplateOfShopDeal = InnerDto.MessageTemplateOfShopDeal;
            Site.MessageTemplateOfDonePrePay = InnerDto.MessageTemplateOfDonePrePay;

            Site.MessageTemplateOfAccountResume = InnerDto.MessageTemplateOfAccountResume;
            Site.MessageTemplateOfAccountSuspend = InnerDto.MessageTemplateOfAccountSuspend;
            Site.MessageTemplateOfAccountTransfer = InnerDto.MessageTemplateOfAccountTransfer;
            Site.MessageTemplateOfAccountChangeName = InnerDto.MessageTemplateOfAccountChangeName;
            Site.MessageTemplateOfAccountRenew = InnerDto.MessageTemplateOfAccountRenew;
            Site.MessageTemplateOfDealCode = InnerDto.MessageTemplateOfDealCode;


            SiteService.Update(Site);
            CacheService.Refresh(CacheKeys.SiteKey);
        }
    }
}