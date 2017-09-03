using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManBox.Web.Utilities
{
    public class AutoLocalizingRoute : Route
    {
        private bool _lowercaseUrls;

        public AutoLocalizingRoute(string url, object defaults, object constraints, bool lowercaseUrls)
            : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler()) {
                _lowercaseUrls = lowercaseUrls;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            // only set the culture if it's not present in the values dictionary yet
            // this check ensures that we can link to a specific language when we need to (fe: when picking your language)
            if (!values.ContainsKey("language"))
            {
                values["language"] = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            }
            var virtualPath = base.GetVirtualPath(requestContext, values);

            if (_lowercaseUrls)
            {
                virtualPath.VirtualPath = virtualPath.VirtualPath.ToLower();
            }

            return virtualPath;
        }
    }
}