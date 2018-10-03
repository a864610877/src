using System.Linq;
using System.Web.Mvc;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc
{
    public class CurrentShopAttribute : CustomModelBinderAttribute, IModelBinder
    {
        public override IModelBinder GetBinder()
        {
            return this;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var membershipService = EcardContext.Container.Resolve<IMembershipService>();
            var shopService = EcardContext.Container.Resolve<IShopService>();
            var helper = EcardContext.Container.Resolve<SecurityHelper>();

            var user = helper.GetCurrentUser();
            if (user == null) return null;
            var shopUser = membershipService.GetUserById(user.CurrentUser.UserId) as ShopUser;
            if (shopUser != null)
            {
                return shopService.GetById(shopUser.ShopId);
            }
            return null;
        }
    }
}