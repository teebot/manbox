using ManBox.Common;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using ManBox.Model.ViewModels;
using ManBox.Common.Logging;
using System.Web;
using System.Configuration;

namespace ManBox.Web.Controllers
{
    public class HomeController : ManBoxControllerBase
    {
        public ActionResult Index(string language = null, bool redirect = false)
        {
            //return RedirectToRoute(WebConstants.RouteNames.LocalizedRoute, new { action = "Compose", controller = "Box", language = this.CurrentLanguageIso });

            if (string.IsNullOrEmpty(language) && ManBoxHostInfo.Instance.IsMultiLangCountry)
            {
                language = this.CurrentLanguageIso;
                return RedirectToRoute(WebConstants.RouteNames.RootLanguage, new { action = "Index", controller = "Home", language = language });
            }

            if (redirect && !string.IsNullOrEmpty(this.CurrentSubscriptionToken))
            {
                return RedirectToAction("Compose", "Box");
            }

            LandingPageViewModel model = new LandingPageViewModel();

            var blogCacheKey = WebConstants.CacheKeys.BlogFeed + CurrentLanguageIso;

            model.BlogFeed = this.HttpContext.Cache[blogCacheKey] as BlogFeed;

            if (model.BlogFeed == null)
            {
                model.BlogFeed = GetBlogFeed();
                this.HttpContext.Cache.Insert(blogCacheKey, model.BlogFeed);
            }


            //ViewBag.YoutubeOrigin = string.Format("{0}://{1}", this.Request.Url.Scheme, this.Request.Url.Host);
            return View("Landing123", model);
        }

        public ActionResult Facebook()
        {
            return RedirectToAction("Index");
            //return View();
        }

        public ActionResult LandingVideo(string language = null)
        {
            return View();
        }

        public ActionResult LandingSoon(string language = null)
        {
            if (string.IsNullOrEmpty(language) && ManBoxHostInfo.Instance.IsMultiLangCountry)
            {
                language = this.CurrentLanguageIso;
                if (string.IsNullOrEmpty(language))
                {
                    language = ManBoxHostInfo.Instance.DefaultCountryLanguageIso;
                }

                return RedirectToRoute(WebConstants.RouteNames.RootLanguage, new { action = "LandingSoon", controller = "Home", language = language });
            }

            return View();
        }

        private BlogFeed GetBlogFeed()
        {
            BlogFeed feed = new BlogFeed();
            string configKey = AppConstants.AppSettingsKeys.BlogJsonFeedUrl + this.CurrentLanguageIso.ToUpper();
            string jsonFeedUrl = ConfigurationManager.AppSettings[configKey];

            if (string.IsNullOrEmpty(jsonFeedUrl))
            {
                return feed;
            }

            string jsonBlogFeed = string.Empty;
            WebRequest request = WebRequest.Create(jsonFeedUrl);
            request.ContentType = "application/json; charset=utf-8";
            request.Timeout = 6000;

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    jsonBlogFeed = sr.ReadToEnd();
                }

                feed = JsonConvert.DeserializeObject<BlogFeed>(jsonBlogFeed);

                if (feed.Posts != null)
                {
                    feed.Posts.ForEach(p => p.Excerpt = HttpUtility.HtmlDecode(p.Excerpt));
                }
            }
            catch
            {
                this.Log(LogType.Warn, "There was a problem retrieving blog json feed");
            }

            return feed;
        }


    }
}
