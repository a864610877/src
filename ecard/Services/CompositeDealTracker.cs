using System.Collections.Generic;
using Ecard.Models;
using Microsoft.Practices.Unity;
using System.Linq;
using System;

namespace Ecard.Services
{
    public class CompositeDealTracker : IDealTracker
    {
        private readonly IEnumerable<IDealTracker> _trackers;

        public CompositeDealTracker(IEnumerable<IDealTracker> trackers)
        {
            _trackers = trackers;
        }
        
        public void Notify(Account account, AccountUser owner, DealLog dealItem)
        {
            
            foreach (var tracker in _trackers)
            {
                tracker.Notify(account, owner, dealItem);
            }
        }

        public void NotifyCode(Shop shop, Shop shopTo, ShopDealLog shopDealLog, string shopNameTo)
        {
            foreach (var tracker in _trackers)
                tracker.NotifyCode(shop, shopTo, shopDealLog, shopNameTo);
        }
    }
}