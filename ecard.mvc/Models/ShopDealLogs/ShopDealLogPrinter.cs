using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.ShopDealLogs
{
    public class ShopDealLogPrinter : ViewModelBase
    {
        public string Password { get; set; }
        [Dependency, NoRender]
        public IShopDealLogService ShopDealLogService { get; set; }
        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IPosEndPointService PosEndPointService { get; set; }
        [Dependency, NoRender]
        public IDealWayService DealWayService { get; set; }
        public int Id { get; set; }
        public DataAjaxResult Print()
        {
            try
            {
                IPasswordService passwordService = UnityContainer.Resolve<IPasswordService>(HostSite.PasswordType);
                var password = passwordService.Decrypto(this.Password);

                if (string.IsNullOrWhiteSpace(this.HostSite.TicketTemplateOfDeal))
                    return new DataAjaxResult(Localize("nonTemplateOfDeal", "�����������Ѵ�ӡģ��"));
                var shopDealLog = this.ShopDealLogService.GetById(Id);
                if (shopDealLog == null)
                    return new DataAjaxResult(Localize("nonShopDealLog", "ָ���̻�����δ�ҵ�"));
                var deallog = this.DealLogService.GetById(shopDealLog.Addin);
                if (deallog == null)
                    return new DataAjaxResult(Localize("nonShopDealLog", "ָ����Ա����δ�ҵ�"));
                var account = this.AccountService.GetById(deallog.AccountId);
                if (account == null)
                    return new DataAjaxResult(Localize("nonAccount", "ָ����Աδ�ҵ�"));
                if (User.SaltAndHash(password, account.PasswordSalt) != account.Password)
                    return new DataAjaxResult(Localize("invalidPassword", "�������"));
                var shop = this.ShopService.GetById(deallog.ShopId);
                if (shop == null)
                    return new DataAjaxResult(Localize("nonShop", "ָ���̻�δ�ҵ�"));
                var dealWay = this.DealWayService.GetById(shop.DealWayId);
                if (dealWay == null)
                    return new DataAjaxResult(Localize("nonDealWay", "ָ��֧������δ�ҵ�"));

                var msg = this.HostSite.TicketTemplateOfDeal.Trim();
                msg = MessageFormator.FormatTickForDeal(msg, HostSite, deallog, account, ShopService.GetById(deallog.ShopId), PosEndPointService.GetById(deallog.SourcePosId), SecurityHelper.GetCurrentUser());
                return new DataAjaxResult() { Data1 = msg };
            }
            catch (System.Exception ex)
            {
                return new DataAjaxResult(ex.Message);
            }
        }
    }
}