using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManBox.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManBox.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : ManBoxTestBase
    {
        private HomeController controller;

        [TestInitialize]
        public void Setup()
        {
            controller = new HomeController();
        }

        //[TestMethod]
        public void tokenized_user_on_landing_is_redirected_to_compose()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);

            // Act
            var redirectResult = controller.Index(null, true) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Compose", redirectResult.RouteValues["Action"]);
        }
    }
}