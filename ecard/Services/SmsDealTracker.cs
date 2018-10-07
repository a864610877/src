using System;
using System.Text.RegularExpressions;
using Ecard.Models;
using log4net;
using Microsoft.Practices.Unity;
using System.Linq;

namespace Ecard.Services
{
    public class SmsDealTracker : IDealTracker
    {
        private readonly IAccountDealDal _siteService;
        private readonly SmsHelper _smsHelper;
        private readonly Site _site;
        [Dependency]
        public IOrder1Service OrderService { get; set; }
        public SmsDealTracker(IAccountDealDal siteService, SmsHelper smsHelper, Site site)
        {
            _siteService = siteService;
            _smsHelper = smsHelper;
            _site = site;
        }

        private static ILog _log = log4net.LogManager.GetLogger(typeof(SmsDealTracker));
        public void Notify(Account account, AccountUser owner, DealLog dealItem)
        {
            try
            {
               
                var site = _siteService.GetSite();
                if (site != null && !string.IsNullOrWhiteSpace(site.MessageTemplateOfDeal))
                {
                    if (owner != null && owner.IsMobileAvailable)
                    {
                        var text = "";
                        switch (dealItem.DealType)
                        {
                            case DealTypes.Deal:
                                text = site.MessageTemplateOfDeal;
                                break;
                            case DealTypes.Integral:
                                text = site.MessageTemplateOfDeal;
                                break;
                            case DealTypes.Recharging:
                                text = site.MessageTemplateOfRecharge;
                                break;
                            case DealTypes.DonePrePay:
                                text = site.MessageTemplateOfDonePrePay;
                                break;
                            case DealTypes.PrePay:
                                text = site.MessageTemplateOfPrePay;
                                break;
                            default:
                                break;
                        }
                        if (string.IsNullOrWhiteSpace(text)) return;

                        text = MessageFormator.Format(text, owner);
                        text = MessageFormator.Format(text, dealItem);
                        text = MessageFormator.Format(text, account);
                        text = MessageFormator.Format(text, _site);


                        _smsHelper.Send(owner.Mobile, text);

                        _log.Info("send sms for " + account.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("send message failed!", ex);
            }
        }

        public void NotifyCode(Shop shop, Shop shopTo, ShopDealLog shopDealLog, string shopNameTo)
        {

            try
            {
                var site = _siteService.GetSite();

                if (site != null && !string.IsNullOrWhiteSpace(site.MessageTemplateOfDealCode) && !string.IsNullOrWhiteSpace(shopNameTo) && Regex.IsMatch(shopNameTo, @"^\d{11}$"))
                {
                    var text = site.MessageTemplateOfDealCode;

                    text = MessageFormator.Format(text, shopTo);
                    text = text.Replace("#code#", shopDealLog.Code);

                    _smsHelper.Send(shopNameTo, text);
                    _log.Info("send sms for " + shopNameTo);
                }
            }
            catch (Exception ex)
            {
                _log.Error("send message failed!", ex);
            }
        }
    }
}