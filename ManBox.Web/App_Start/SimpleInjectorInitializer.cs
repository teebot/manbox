
namespace ManBox.Web.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;
    using ManBox.Business;
    using ManBox.Common.Logging;
    using ManBox.Common.Mail;
    using SimpleInjector;
    using SimpleInjector.Integration.Web.Mvc;
    using ManBox.Business.BackOffice;
    using ManBox.Common.Security;
    
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            var container = new Container();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            //container.RegisterMvcAttributeFilterProvider(); //not used

            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
     
        public static void InitializeContainer(Container container)
        {
            container.Register<IMailService, MandrillMailService>();
            container.Register<ILogger, NLogLogger>();
            container.Register<IAuthenticationService, FormsAuthServiceWrapper>();
            container.Register<ICookieContainer, CookieContainer>(Lifestyle.Singleton);

            container.Register<IShopRepository, ShopRepository>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<ICheckoutRepository, CheckoutRepository>();

            //BackOffice
            container.Register<IShipmentRepository, ShipmentRepository>();
        }
    }
}