using System.Configuration;
using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth;
using System.Linq;
using ManBox.Web.Helpers;

namespace ManBox.Web
{
    public static class AuthConfig
    {
        private static readonly string FacebookClientKey = "Facebook";

        public static void RegisterAuth(string countryIso)
        {
            if (!OAuthWebSecurity.RegisteredClientData.Any(c => c.DisplayName == FacebookClientKey))
            {
                var fbAppId = ConfigurationManager.AppSettings[ManBox.Common.AppConstants.AppSettingsKeys.FbAppId + countryIso];
                var fbAppSecret = ConfigurationManager.AppSettings[ManBox.Common.AppConstants.AppSettingsKeys.FbAppSecret + countryIso];
                OAuthWebSecurity.RegisterClient(new FacebookScopedClient(fbAppId, fbAppSecret, "email"), FacebookClientKey, null);
            }
        }
    }
}