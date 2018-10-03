using Ecard.Models;
using Ecard.Mvc;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        //[Dependency]
        //public static IProvinceService ProvinceService { get; set; }
        //[Dependency]
        //public static ICityService CityService { get; set; }

        //[Dependency]
        //public SecurityHelper _securityHelper { get; set; }
        
    }


    public static class ExtensionsHelper
    {
        public static T Resolve<T>()
        {
            var container = (IUnityContainer)HttpContext.Current.Application["container"];
            return container.Resolve<T>();
        }

        //public static string Localize(string objectKey, string category, string defaultValue)
        //{
        //    var container = (IUnityContainer)HttpContext.Current.Application["container"];
        //    var manager = container.Resolve<I18NManager>();
        //    return manager.Get(objectKey, category, defaultValue);
        //}

        //public static string Message(string objectKey)
        //{
        //    return Localize("messages", objectKey, objectKey);
        //}
    }
}