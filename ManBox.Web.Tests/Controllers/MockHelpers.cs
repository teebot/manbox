using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ManBox.Common.Security;
using ManBox.Model;
using Moq;
using ManBox.Common;
using System.Web.Mvc;

namespace ManBox.Web.Tests.Controllers
{
    public static class MockHelpers
    {
        public static string SubscribedToken = "a4823d5c-937c-41c9-a63c-ad4b0f325139";
        public static string NonSubscribedToken = "9a506713-9abf-448b-b077-9a9b089b63aa";
        public static string SubscribedUserEmail = "subscribed@subscribed.com";
        public static string NonSubscribedUserEmail = "thibaut.nguyen@gmail.com";
        public static string SubscribedInModificationSameAmountToken = "3d8ad808-6656-42fa-8615-9940d6e6cda9";
        public static string SubscribedInModificationDifferentAmountToken = "f7a402e4-02c0-4431-8275-47539beefa10";
        public static string AnonymousToken = "eaf27224-fd03-4e44-95e1-51747c4cb572";
        public static string NonSubscribedCartWithPackToken = "cfff0632-e158-46c1-bb45-4818e34065f4";
        public static string EmptyAnonymousToken = "addd0632-e158-46c1-bb45-4818e34061a4";

        public static HttpContextBase GetFakeContext(string subToken = null, string emailIdentity = null, string langIso = "fr", string userAgent = null, bool isAuthenticated = false)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            var principal = MockHelpers.SetThreadUser(langIso, subToken, emailIdentity, isAuthenticated);
            context.SetupGet(x => x.User).Returns(principal);

            if (userAgent != null)
            {
                request.Setup(x => x.UserAgent).Returns(userAgent);
            }

            // context cache
            context.SetupGet(x => x.Cache).Returns(HttpRuntime.Cache);

            context.Setup(ct => ct.Request).Returns(request.Object);
            context.Setup(ct => ct.Response).Returns(response.Object);
            
            // context session
            var mockSession = new MockSession();
            context.Setup(ct => ct.Session).Returns(mockSession);
            mockSession[WebConstants.SessionKeys.Lang] = langIso;
            
            context.Setup(ct => ct.Server).Returns(server.Object);

            return context.Object;
        }


        private static IPrincipal SetThreadUser(string langIso, string subToken, string emailIdentity, bool isAuthenticated)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                ManBoxHostInfo.Instance.AvailLanguages = new Dictionary<string, int>() { { "fr", 1 }, { "nl", 2 } };

                var email = string.Empty;

                if (emailIdentity != null)
                {
                    email = ent.Users.Single(u => u.Email == emailIdentity).Email;
                }

                var user = new ManBoxUser()
                    {
                        Email = "",
                        LanguageId = ManBoxHostInfo.Instance.AvailLanguages[langIso],
                        LanguageIsoCode = langIso,
                        Token = subToken ?? null
                    };

                ManBoxIdentity identity = new ManBoxIdentity(user);
                identity.SetAuthentication(isAuthenticated);

                var principal = new ManBoxPrincipal(identity);

                Thread.CurrentThread.CurrentCulture = new CultureInfo(identity.User.LanguageIsoCode);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(identity.User.LanguageIsoCode);
                Thread.CurrentPrincipal = principal;

                return principal;
            }
        }
    }

    public class MockSession : HttpSessionStateBase
    {
        Dictionary<string, object> mSessionStorage = new Dictionary<string, object>();

        public override object this[string name]
        {
            get {

                if (mSessionStorage.ContainsKey(name))
                {
                    return
                    mSessionStorage[name];
                }
                return null;
            }
            set { mSessionStorage[name] = value; }
        }
    }
}
