using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ManBox.Web.Controllers;
using Segmentio;
using Segmentio.Model;

namespace ManBox.Web.Filters
{
    // TODO: make this async
    public class AnalyticAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            TrackAction(filterContext);
        }

        private void TrackAction(ActionExecutingContext filterContext)
        {
            var manboxController = filterContext.Controller as ManBoxControllerBase;

            if (manboxController != null && !string.IsNullOrEmpty(manboxController.CurrentSubscriptionToken))
            {
                Properties properties = new Properties();
                filterContext.ActionParameters.ToList().ForEach(p => properties.Add(p.Key, p.Value));
                Analytics.Client.Track(manboxController.CurrentSubscriptionToken, filterContext.ActionDescriptor.ActionName, properties);
            }
        }
    }
}