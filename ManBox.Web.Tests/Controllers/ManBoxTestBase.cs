using ManBox.Business;
using ManBox.Business.BackOffice;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Security;
using ManBox.Common.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System.Transactions;
using System.Web.Mvc;

namespace ManBox.Web.Tests.Controllers
{
    [TestClass]
    public class ManBoxTestBase
    {
        TransactionScope _trans;

        [TestInitialize]
        public void Init()
        {
            // Setup transaction scope
            _trans = new TransactionScope();

            ////setup dependency injection
            var container = new Container();

            container.Register<ILogger, MockLogger>();
            container.Register<IMailService, MockMailService>();

            container.Register<IShopRepository, ShopRepository>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<ICheckoutRepository, CheckoutRepository>();
            container.Register<IShipmentRepository, ShipmentRepository>();

            container.Register<IAuthenticationService, MockAuthenticationService>();

            container.Register<ICookieContainer, MockCookieContainer>(Lifestyle.Singleton);

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            ManBoxHostInfo.Instance.HostCountryId = 1;
        }

        [TestCleanup]
        public void CleanUp()
        {
            _trans.Dispose();
        }
    }
}
