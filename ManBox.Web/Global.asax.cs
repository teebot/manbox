using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ManBox.Business;
using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Web.App_Start;

namespace ManBox.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            SimpleInjectorInitializer.Initialize();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AnalyticsConfig.Register();
        }

        protected void Application_BeginRequest(Object source, EventArgs e)
        {
            // once per app domain
            if (ManBoxHostInfo.Instance.HostCountryId == null)
            {
                HttpApplication app = (HttpApplication)source;

                ConfigureAppDomain(app);
            }
        }

        private static void ConfigureAppDomain(HttpApplication app)
        {
            var shopRepo = DependencyResolver.Current.GetService<IShopRepository>();
            var logger = DependencyResolver.Current.GetService<ILogger>();


            ManBoxHostInfo.Instance.AvailLanguages = shopRepo.GetAvailableLanguages();

            if (app.Context.Request != null
                && !string.IsNullOrEmpty(app.Context.Request.Url.Host))
            {
                if (app.Context.Request.Url.Host.Contains(".be"))
                {
                    ConfigureCulture(AppConstants.Countries.IsoBE, "fr", AppConstants.Countries.BE, true);
                }
                else if (app.Context.Request.Url.Host.Contains(".nl"))
                {
                    ConfigureCulture(AppConstants.Countries.IsoNL, "nl", AppConstants.Countries.NL, false); 
                }
                else if (app.Context.Request.Url.Host.Contains(".fr"))
                {
                    ConfigureCulture(AppConstants.Countries.IsoFR, "fr", AppConstants.Countries.FR, false);
                }
                else
                {
                    ConfigureCulture(AppConstants.Countries.IsoBE, "nl", AppConstants.Countries.BE, true);
                }

                logger.Log(LogType.Info, string.Format("App Pool was initialized with default culture {0}. Domain: {1}", CultureInfo.DefaultThreadCurrentCulture, app.Context.Request.Url.Host));
            }
            else {
                logger.Log(LogType.Error, "No URL host was detected");
            }
        }

        private static void ConfigureCulture(string countryIso, string defaultCountryLangIso, int countryId, bool isMultiLangCountry)
        {
            // some authentication providers such as facebook are domain dependant
            AuthConfig.RegisterAuth(countryIso);

            // instance configuration
            ManBoxHostInfo.Instance.HostCountryId = countryId;
            ManBoxHostInfo.Instance.CountryIso = countryIso;
            ManBoxHostInfo.Instance.IsMultiLangCountry = isMultiLangCountry;
            ManBoxHostInfo.Instance.DefaultCountryLanguageIso = defaultCountryLangIso;
            
            // set default thread culture
            var culture = CultureInfo.CreateSpecificCulture(string.Format("{0}-{1}", defaultCountryLangIso, countryIso));
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }



    /// <summary>
    /// Singleton application info initialized at first request (per app instance / cycle)
    /// </summary>
    public class ManBoxHostInfo
    {
        private static ManBoxHostInfo _instance;
        private ManBoxHostInfo() { }

        public static ManBoxHostInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ManBoxHostInfo();
                }

                return _instance;
            }
        }

        public int? HostCountryId { get; set; }
        
        public bool IsMultiLangCountry { get; set; }

        public string CountryIso { get; set; }

        public Dictionary<string, int> AvailLanguages { get; set; }

        public string DefaultCountryLanguageIso { get; set; }
    }
}