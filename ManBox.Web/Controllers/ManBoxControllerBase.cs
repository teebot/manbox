using System;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ManBox.Business;
using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Security;
using ManBox.Model.ViewModels;
using ManBox.Common.Resources;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace ManBox.Web.Controllers
{
    public class ManBoxControllerBase : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();
            logger.Log(filterContext.Exception);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ReAuthenticateNeeded();
            InitializePrincipal(filterContext);
        }

        protected ActionResult TryRefererRedirect(RedirectToRouteResult fallback)
        {
            if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.ToString()))
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return fallback;
        }

        /// <summary>
        /// retrieve and set current user if no more session but auth cookie still present
        /// </summary>
        private void ReAuthenticateNeeded()
        {
            if (CurrentAuthUser == null && HttpContext.User.Identity.IsAuthenticated)
            {
                var userRepository = DependencyResolver.Current.GetService<IUserRepository>();
                var user = userRepository.GetUserByEmail(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    CurrentAuthUser = user;
                    CurrentSubscriptionToken = user.Token;
                }
                else
                {
                    var auth = DependencyResolver.Current.GetService<IAuthenticationService>();
                    auth.SignOut();
                }
            }
        }

        /// <summary>
        /// set the thread principal
        /// </summary>
        private void InitializePrincipal(ActionExecutingContext filterContext)
        {
            ManBoxUser manboxUser = new ManBoxUser();
            string language = filterContext.RouteData.Values["language"] as string;

            if (string.IsNullOrEmpty(language) || language.Length != 2 || !ManBoxHostInfo.Instance.AvailLanguages.ContainsKey(language))
            {
                if (ManBoxHostInfo.Instance.IsMultiLangCountry)
                {
                    language = GetDefaultLang();
                }
                else {
                    language = ManBoxHostInfo.Instance.DefaultCountryLanguageIso;
                }
            }

            manboxUser.Token = CurrentSubscriptionToken ?? null;
            manboxUser.LanguageIsoCode = language;
            manboxUser.LanguageId = ManBoxHostInfo.Instance.AvailLanguages[language];
            manboxUser.CountryId = ManBoxHostInfo.Instance.HostCountryId.Value;
            manboxUser.CountryIsoCode = ManBoxHostInfo.Instance.CountryIso;

            // complete thread user if authenticated session
            if (CurrentAuthUser != null)
            {
                manboxUser.Email = CurrentAuthUser.Email;
                manboxUser.FirstName = CurrentAuthUser.FirstName;
                manboxUser.LastName = CurrentAuthUser.LastName;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
            Thread.CurrentPrincipal = new ManBoxPrincipal(new ManBoxIdentity(manboxUser));
            
            // set session
            this.CurrentLanguageIso = language;
        }

        protected void Log(LogType logType, string message)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();
            logger.Log(logType, message);
        }

        protected void Log(Exception e)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();
            logger.Log(e);
        }

        protected void Authenticate(UserViewModel user)
        {
            var auth = DependencyResolver.Current.GetService<IAuthenticationService>();
            auth.DoAuth(user.Email, true);

            CurrentAuthUser = user;
            CurrentLanguageIso = user.LanguageIsoCode;
            CurrentSubscriptionToken = user.Token;
        }

        protected void DisplayLocalizedMessage(string resourceKey)
        {
            TempData["Message"] = UITexts.ResourceManager.GetString(resourceKey);
        }

        protected void DisplayMessage(string message)
        {
            TempData["Message"] = message;
        }

        protected string GetCdnImagePath()
        {
            return ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.CdnImagePath] as string;
        }

        protected string GetDefaultLang()
        {
            if (!string.IsNullOrEmpty(this.CurrentLanguageIso))
            {
                return this.CurrentLanguageIso;
            }

            var browserLang = this.GetBrowserPreferredLanguage();
            if (!string.IsNullOrEmpty(browserLang))
            {
                return browserLang;
            }

            return ManBoxHostInfo.Instance.DefaultCountryLanguageIso;
        }

        /// <summary>
        /// goes through the list of browser preferred locales
        /// return the preferred one if any of these is in manbox's available languages
        /// </summary>
        /// <returns></returns>
        private string GetBrowserPreferredLanguage()
        {
            if (this.HttpContext.Request != null && this.HttpContext.Request.UserLanguages != null)
            {
                foreach (var l in HttpContext.Request.UserLanguages)
                {
                    var userLang = string.Empty;
                    if (l.Contains(';'))
                    {
                        userLang = l.Substring(0, l.IndexOf(';')).ToLower();
                    }
                    else if (l.Contains('-'))
                    {
                        userLang = l.Substring(0, l.IndexOf('-')).ToLower();
                    }

                    if (ManBoxHostInfo.Instance.AvailLanguages.ContainsKey(userLang))
                    {
                        return userLang;
                    }
                }
            }

            return null;
        }

        #region Current Session Cookie Properties
        public string CurrentLanguageIso
        {
            get
            {
                return HttpContext.Session[WebConstants.SessionKeys.Lang] as string;
            }
            set
            {
                HttpContext.Session[WebConstants.SessionKeys.Lang] = value;
            }
        }

        public UserViewModel CurrentAuthUser
        {
            get
            {
                return HttpContext.Session[WebConstants.SessionKeys.User] as UserViewModel;
            }

            set
            {
                HttpContext.Session[WebConstants.SessionKeys.User] = value;
            }
        }

        public string CurrentSubscriptionToken
        {
            get
            {
                var cookieContainer = DependencyResolver.Current.GetService<ICookieContainer>();
                return cookieContainer[WebConstants.Cookies.Token];
            }

            set
            {
                var cookieContainer = DependencyResolver.Current.GetService<ICookieContainer>();
                cookieContainer.SetCookie(WebConstants.Cookies.Token, value, DateTime.Now.AddMonths(12));
            }
        }

        #endregion
    }
}