using ManBox.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManBox.Web.Filters
{
    public class StaticHtmlForCrawlersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var browser = filterContext.HttpContext.Request.Browser;
                var url = filterContext.RequestContext.HttpContext.Request.Url;

                bool isACrawler;
                if (bool.TryParse(browser["crawler"], out isACrawler) && isACrawler)
                {
                    var html = HeadlessBrowser.Agent.GetHtmlFromUrl(url.ToString(), "<body>", "<body class=\"no-javascript\">");
                    filterContext.HttpContext.Response.Write(html);
                    filterContext.Result = new EmptyResult();
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            catch (Exception e)
            {
                // log but fail silently for the user 

                ILogger logger = DependencyResolver.Current.GetService<ILogger>();
                logger.Log(e);

                base.OnActionExecuting(filterContext);
            }
        }
    }
}