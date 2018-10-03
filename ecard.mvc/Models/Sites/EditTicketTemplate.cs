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
    public class EditTicketTemplate
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

        public EditTicketTemplate()
        {
            InnerDto = new Site();
        }
            
        [UIHint("richtext")]
        public string TicketTemplateOfRecharge
        {
            get { return GetSite().TicketTemplateOfRecharge; }
            set { InnerDto.TicketTemplateOfRecharge = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfChangeAccountName
        {
            get { return GetSite().TicketTemplateOfChangeAccountName; }
            set { InnerDto.TicketTemplateOfChangeAccountName = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfDeal
        {
            get { return GetSite().TicketTemplateOfDeal; }
            set { InnerDto.TicketTemplateOfDeal = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfCancelDeal
        {
            get { return GetSite().TicketTemplateOfCancelDeal; }
            set { InnerDto.TicketTemplateOfCancelDeal = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfClose
        {
            get { return GetSite().TicketTemplateOfClose; }
            set { InnerDto.TicketTemplateOfClose = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfRenewAccount
        {
            get { return GetSite().TicketTemplateOfRenewAccount; }
            set { InnerDto.TicketTemplateOfRenewAccount = value; }
        }
        [UIHint("richtext")]
        public string TicketTemplateOfOpen
        {
            get { return GetSite().TicketTemplateOfOpen; }
            set { InnerDto.TicketTemplateOfOpen = value; }
        }        
        [UIHint("richtext")]
        public string TicketTemplateOfTransfer
        {
            get { return GetSite().TicketTemplateOfTransfer; }
            set { InnerDto.TicketTemplateOfTransfer = value; }
        }         
        [UIHint("richtext")]
        public string TicketTemplateOfSuspendAccount
        {
            get { return GetSite().TicketTemplateOfSuspendAccount; }
            set { InnerDto.TicketTemplateOfSuspendAccount = value; }
        }         
        [UIHint("richtext")]
        public string TicketTemplateOfResumeAccount
        {
            get { return GetSite().TicketTemplateOfResumeAccount; }
            set { InnerDto.TicketTemplateOfResumeAccount = value; }
        }         
        public void Ready()
        {
        }
          
        public void Save()
        { 
            Site.TicketTemplateOfRecharge = InnerDto.TicketTemplateOfRecharge;
            Site.TicketTemplateOfClose = InnerDto.TicketTemplateOfClose;
            Site.TicketTemplateOfDeal = InnerDto.TicketTemplateOfDeal;
            Site.TicketTemplateOfCancelDeal = InnerDto.TicketTemplateOfCancelDeal;
            Site.TicketTemplateOfOpen = InnerDto.TicketTemplateOfOpen;
            Site.TicketTemplateOfTransfer = InnerDto.TicketTemplateOfTransfer;
            Site.TicketTemplateOfResumeAccount = InnerDto.TicketTemplateOfResumeAccount;
            Site.TicketTemplateOfSuspendAccount = InnerDto.TicketTemplateOfSuspendAccount;
            Site.TicketTemplateOfChangeAccountName = InnerDto.TicketTemplateOfChangeAccountName;
            Site.TicketTemplateOfRenewAccount = InnerDto.TicketTemplateOfRenewAccount; 
            SiteService.Update(Site);
            CacheService.Refresh(CacheKeys.SiteKey);
        }
    }
}