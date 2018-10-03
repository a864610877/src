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
                    return new DataAjaxResult(Localize("nonTemplateOfDeal", "请先设置消费打印模板"));
                var shopDealLog = this.ShopDealLogService.GetById(Id);
                if (shopDealLog == null)
                    return new DataAjaxResult(Localize("nonShopDealLog", "指定商户交易未找到"));
                var deallog = this.DealLogService.GetById(shopDealLog.Addin);
                if (deallog == null)
                    return new DataAjaxResult(Localize("nonShopDealLog", "指定会员交易未找到"));
                var account = this.AccountService.GetById(deallog.AccountId);
                if (account == null)
                    return new DataAjaxResult(Localize("nonAccount", "指定会员未找到"));
                if (User.SaltAndHash(password, account.PasswordSalt) != account.Password)
                    return new DataAjaxResult(Localize("invalidPassword", "密码错误"));
                var shop = this.ShopService.GetById(deallog.ShopId);
                if (shop == null)
                    return new DataAjaxResult(Localize("nonShop", "指定商户未找到"));
                var dealWay = this.DealWayService.GetById(shop.DealWayId);
                if (dealWay == null)
                    return new DataAjaxResult(Localize("nonDealWay", "指定支付渠道未找到"));

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