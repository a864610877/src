using System.Web.Mvc;
using Ecard.Models;
using Ecard.Mvc.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Controllers
{
    public class BaseController : Controller
    {
        [Dependency]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }
        [Dependency]
        public Site TheSite { get; set; }
        [Dependency]
        public IMenuService MenuService { get; set; }
        protected PageModel CreatePageModel()
        {
            PageModel pageModel = new PageModel();
            var user = SecurityHelper.GetCurrentUser();
            if (user != null)
            {
                pageModel.Menus = MenuService.GetMenus(user.CurrentUser);
                pageModel.User = user.CurrentUser;
            }
            pageModel.Site = new SiteViewModel(TheSite, UnityContainer);
            return pageModel;
        }
    }
}