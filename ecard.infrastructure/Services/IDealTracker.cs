using Ecard.Models;

namespace Ecard.Services
{
    public interface IDealTracker
    {
        void Notify(Account account, AccountUser owner, DealLog dealItem);
        void NotifyCode(Shop shop, Shop shopTo, ShopDealLog shopDealLog, string shopNameTo);
    }
}
