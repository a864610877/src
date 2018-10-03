using System.Collections.Generic;
using System.Linq;
using Ecard.Models;
using Ecard.Mvc.Models.PointPolicies;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PointRebateLogs
{
    public class ListPointRebateLogs : EcardModelListRequest<ListPointRebateLog>
    {
        public ListPointRebateLogs()
        {
            OrderBy = "submittime desc";
        }

        [Dependency, NoRender]
        public IPointRebateLogService PointRebateLogService { get; set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IPointRebateService PointRebateService { get; set; }

        public void Ready()
        {
        } 

        public void Query()
        {
            var query = this.PointRebateLogService.Query(new PointRebateLogRequest());
            // fill condition
            List = query.ToList(this, x => new ListPointRebateLog(x));
            var accounts = AccountService.GetByIds(List.Select(x => x.InnerObject.AccountId).ToArray()).ToList();
            foreach (var item in List)
            {
                var account = accounts.FirstOrDefault(x => x.AccountId == item.InnerObject.AccountId);
                if(account!=null)
                {
                    item.AccountName = account.Name;
                }
            }
            var users = MembershipService.GetByIds(List.Select(x => x.InnerObject.UserId).ToArray()).ToList();
            foreach (var item in List)
            {
                var user = users.FirstOrDefault(x => x.UserId == item.InnerObject.UserId);
                if (user != null)
                {
                    item.UserName = user.Name;
                }
            }

            var ids = List.Select(x => x.InnerObject.PointRebateId).ToList();
            var rebates = PointRebateService.Query().Where(x=>ids.Contains(x.PointRebateId)).ToList();
            foreach (var item in List)
            {
                var rebate = rebates.FirstOrDefault(x => x.PointRebateId == item.InnerObject.PointRebateId);
                if(rebate != null)
                {
                    item.PointRebateName = rebate.DisplayName;
                }
            }

            var shops = ShopService.GetByIds(accounts.Select(x => x.ShopId).ToArray()).ToList();
            shops.Add(Shop.Default);
            foreach (var item in List)
            {
                var account = accounts.FirstOrDefault(x => x.AccountId == item.InnerObject.AccountId);
                if(account==null) continue;
                var shop = shops.FirstOrDefault(x => x.ShopId == account.ShopId);
                if(shop == null) continue;

                item.AccountShopName = shop.DisplayName;
            }
        } 

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        { 
            yield break;
        }

        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPointRebateLog item)
        { 
            yield break;
        } 
    }
}
