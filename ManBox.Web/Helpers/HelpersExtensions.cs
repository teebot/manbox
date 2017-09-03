using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ManBox.Common.Resources;
using System.Configuration;
using ManBox.Common;

namespace ManBox.Web.Helpers
{
    public static class HtmlHelpersExtensions
    {
        public static MvcHtmlString LocalizedText(this HtmlHelper helper, string code)
        {
            return new MvcHtmlString(UITexts.ResourceManager.GetString(code));
        }

        public static MvcHtmlString LocalizedText(this HtmlHelper helper, string code, string[] args)
        {
            var formatted = string.Empty;
            var localeText = UITexts.ResourceManager.GetString(code);

            if (!string.IsNullOrEmpty(localeText))
            {
                formatted = string.Format(localeText, args);
            }

            return new MvcHtmlString(formatted);
        }

        public static MvcHtmlString LocalizedCodeValue(this HtmlHelper helper, string codeValueCategory, string codeValue)
        {
            return new MvcHtmlString(CodeValuesTexts.ResourceManager.GetString(codeValueCategory + codeValue));
        }
    }

    public static class UrlHelpersExtensions
    {
        public static string LocalizedImage(this UrlHelper helper, string image)
        {
            string path = string.Format("~/Content/images/_{0}/{1}", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, image);
            return UrlHelper.GenerateContentUrl(path, helper.RequestContext.HttpContext);
        }

        public static string CdnPath(this UrlHelper helper)
        {
            return ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.CdnImagePath] as string;
        }
    }


}