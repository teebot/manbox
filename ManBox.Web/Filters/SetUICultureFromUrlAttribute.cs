using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ManBox.Web.Controllers;

namespace ManBox.Common.Web
{
    public class SetUICultureFromUrlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values.ContainsKey("lang"))
            {
                string langIso = (string)filterContext.RouteData.Values["lang"];
                if (langIso != null)
                {
                    var user = ((ManBoxControllerBase)filterContext.Controller).CurrentUser;
                    
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(langIso);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(langIso);
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
