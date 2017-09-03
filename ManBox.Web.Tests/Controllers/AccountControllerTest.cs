using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ManBox.Business;
using ManBox.Common;
using ManBox.Model;
using ManBox.Model.ViewModels;
using ManBox.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using ManBox.Common.Security;
using ManBox.Common.Mail.Models;
using System.Web;
using ManBox.Common.UnitTesting;

namespace ManBox.Web.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest : ManBoxTestBase
    {
        private AccountController controller;

        [TestInitialize]
        public void Setup()
        {
            var repo = DependencyResolver.Current.GetService<IUserRepository>();
            var shopRepo = DependencyResolver.Current.GetService<IShopRepository>();
            controller = new AccountController(repo, shopRepo);
        }

        [TestMethod]
        public void submit_valid_contact_form_sends_mail_and_redirect_to_compose()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);
            var contactForm = new ContactFormViewModel(){
                Email = "test@test.com",
                FirstName = "chucky",
                Message = "love vampires"
            };

            // Act
            var redirectResult = controller.SendSupportMail(contactForm) as RedirectToRouteResult;

            // Assert 
            Assert.IsNotNull(contactForm.Subject);
            Assert.AreEqual("Compose", redirectResult.RouteValues["Action"]); // verify redirect
            Assert.IsTrue(!string.IsNullOrEmpty(controller.TempData["Message"] as string)); // verify confirmation message
        }

        [TestMethod]
        public void submit_invalid_contact_form_sends_mail_displays_error_message()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);
            var contactForm = new ContactFormViewModel()
            {
                Email = "harry@hogwarts.edu",
                FirstName = null,
                Message = "I want to know more about witchcraft."
            };

            // Act
            controller.ModelState.AddModelError("FirstName", "A firstname is required");
            var redirectResult = controller.SendSupportMail(contactForm) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("Contact", redirectResult.RouteValues["Action"]);
            Assert.IsTrue(!string.IsNullOrEmpty(controller.TempData["Message"] as string)); // verify confirmation message
        }

        [TestMethod]
        public void check_past_orders_returns_correct_set() 
        {
            // arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken), new RouteData(), controller);

            // act
            var result = controller.OrdersOverview() as ViewResult;
            var model = result.Model as OrdersOverviewViewModel;

            // assert
            Assert.AreEqual(58, model.Orders.First().DeliveryId);
            Assert.AreEqual(CodeValues.DeliveryState.Sent, model.Orders.First().DeliveryState);
        }

        [TestMethod]
        public void authenticate_using_encrypted_non_expired_token_user_is_loggedin()
        { 
            // arrange 
            var encryptedToken = ManBox.Common.Security.TokenEncrypt.EncryptTokenAsExpiring(MockHelpers.SubscribedToken, DateTime.Now.AddDays(1));
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // act
            var result = controller.TokenLogin(encryptedToken) as ActionResult;

            // assert
            Assert.AreEqual(true, MockAuthenticationService.LoggedIn);
            Assert.AreEqual(MockHelpers.SubscribedToken, controller.CurrentSubscriptionToken);
        }

        [TestMethod]
        public void authenticate_using_encrypted_expired_token_user_is_not_loggedin()
        {
            // arrange 
            var encryptedToken = ManBox.Common.Security.TokenEncrypt.EncryptTokenAsExpiring(MockHelpers.SubscribedToken, DateTime.Now.AddDays(-1));
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // act
            var result = controller.TokenLogin(encryptedToken) as ActionResult;

            // assert
            Assert.AreEqual(false, MockAuthenticationService.LoggedIn);
        }

        
        [TestMethod]
        public void registered_user_without_subscription_logs_in_without_token_cookie()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act
            controller.Login(new LoginViewModel() { Email = "test4@test4.com", Password = "test123" });

            // Assert
            Assert.AreEqual(true, MockAuthenticationService.LoggedIn);
            Assert.IsNotNull(controller.CurrentSubscriptionToken);
        }

        [TestMethod]
        public void registered_user_with_subscription_logs_in_without_token_cookie()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act
            controller.Login(new LoginViewModel() { Email = "ez32ffze@e321r.gr", Password = "test123" });

            // Assert
            Assert.AreEqual(true, MockAuthenticationService.LoggedIn);
            Assert.AreEqual("b8eb1533-a688-4aac-aa52-19b445399c51", controller.CurrentSubscriptionToken);
        }

        [TestMethod]
        public void password_reset_sends_encrypted_token_in_mail()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act
            controller.PasswordReset(new ForgotPasswordViewModel() { Email = "ez32ffze@e321r.gr" });
            var model = MockMailService.EmailModel as PasswordResetMail;

            // Assert
            
            Assert.IsNotNull(model.LinkToken);
            Assert.IsTrue(model.LinkToken.Length > 10);
        }

        [TestMethod]
        public void token_login_with_valid_encrypted_token_logs_user_in() 
        { 
            // Arrange 
            var token = TokenEncrypt.EncryptTokenAsExpiring(MockHelpers.SubscribedToken, DateTime.Now.AddDays(1));
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act 
            controller.TokenLogin(token);

            // Assert
            Assert.AreEqual(true, MockAuthenticationService.LoggedIn);
            Assert.AreEqual(MockHelpers.SubscribedToken, controller.CurrentSubscriptionToken);
        }

        [TestMethod]
        public void token_login_success_redirects_to_orders_overview()
        {
            // Arrange 
            var token = TokenEncrypt.EncryptTokenAsExpiring(MockHelpers.SubscribedToken, DateTime.Now.AddDays(1));
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act 
            var redirectResult = controller.TokenLogin(token) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("OrdersOverview", redirectResult.RouteValues["Action"]); // verify redirect
            Assert.AreEqual("Account", redirectResult.RouteValues["Controller"]); // verify redirect
        }

        [TestMethod]
        public void token_login_with_expired_encrypted_token_does_not_log_user_in()
        {
            // Arrange 
            var token = TokenEncrypt.EncryptTokenAsExpiring(MockHelpers.SubscribedToken, DateTime.Now.AddDays(-1));
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act 
            controller.TokenLogin(token);

            // Assert
            Assert.AreEqual(false, MockAuthenticationService.LoggedIn);
            Assert.IsNull(controller.CurrentSubscriptionToken);
        }
    }
}
