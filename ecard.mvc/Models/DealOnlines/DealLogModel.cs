using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.DealOnlines
{
    public class DealLogModel
    {
        private string _serverSerialNo;
        public string ServerSerialNo
        {
            get { return _serverSerialNo.TrimSafty(); }
            set { _serverSerialNo = value; }
        }

        public DealLog DealLog { get; set; }
        public void Ready()
        {
            var id = 0;
            if (!int.TryParse(ServerSerialNo, out id))
                return;

            DealLog = DealLogService.GetById(id);
            if (DealLog == null)
                return;
            if (DealLog.State != DealLogStates.Normal || DealLog.DealType != DealTypes.Deal)
            {
                DealLog = null;
                return;
            }
            var user = SecurityHelper.GetCurrentUser().CurrentUser;
            AdminUser adminUser = user as AdminUser;
            if (adminUser != null)
            {
                if (DealLog.SourceShopName != Constants.SystemShopName)
                {
                    DealLog = null;
                    return;
                }
            }
            ShopUser shopUser = user as ShopUser;
            if (shopUser != null)
            {
                var shop = ShopService.GetById(shopUser.ShopId);
                if (DealLog.SourceShopName != shop.Name)
                {
                    DealLog = null;
                    return;
                }
            }
        }

        [Dependency]
        public IDealLogService DealLogService { get; set; }

        [Dependency]
        public IAccountService AccountService { get; set; }
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
    }
}