using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ManBox.Common;
using ManBox.Web.Utilities;
using LowercaseRoutesMVC4;

namespace ManBox.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.LowercaseUrls = true;

            var localizingRoute = new AutoLocalizingRoute("{language}/{controller}/{action}/{id}/{id2}",
                                    new { id = UrlParameter.Optional, id2 = UrlParameter.Optional }, new { language = "^[a-z]{2}$" }, true);
            routes.Add(WebConstants.RouteNames.LocalizedRoute, localizingRoute);

            routes.MapRouteLowercase(
                WebConstants.RouteNames.RootLanguage, // Route name
                "{language}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, id2 = UrlParameter.Optional, language = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRouteLowercase(
                WebConstants.RouteNames.Default, // Route name
                "{controller}/{action}/{id}/{id2}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, id2 = UrlParameter.Optional } // Parameter defaults
            );

        }
    }
}