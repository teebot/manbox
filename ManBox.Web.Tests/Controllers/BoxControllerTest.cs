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
using ManBox.Common.Extensions;
using System;
using Moq;
using System.Web;
using ManBox.Web.Filters;
using System.Collections.Generic;

namespace ManBox.Web.Tests.Controllers
{
    [TestClass]
    public class BoxControllerTest : ManBoxTestBase
    {
        private readonly decimal defaultShippingFee = 3;
        private BoxController controller;

        [TestInitialize]
        public void Setup(){
            var repo = DependencyResolver.Current.GetService<IShopRepository>();
            controller = new BoxController(repo);
        }

        [TestMethod]
        public void compose_returns_view_with_valid_json()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);
            
            // Act
            ViewResult result = controller.Compose() as ViewResult;

            // Assert
            dynamic json = JObject.Parse(((CatalogOverviewViewModel)result.Model).JsonOverviewData);
            string categoryTitle = json.ProductsCategories[0].Title;
            string productTitle = json.ProductsCategories[0].Products[0].Title;
            string brand = json.ProductsCategories[0].Products[0].Brand;
            string packTitle = json.Packs[0].Title;
            
            Assert.AreEqual("Boxers", categoryTitle);
            Assert.AreEqual("Boxer 95/5", productTitle);
            Assert.AreEqual("Survival Kit", packTitle);
        }

        [TestMethod]
        public void compose_returns_view_with_correct_language()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(langIso: "nl"), new RouteData(), controller);

            // Act
            ViewResult result = controller.Compose() as ViewResult;

            // Assert
            dynamic json = JObject.Parse(((CatalogOverviewViewModel)result.Model).JsonOverviewData);
            string categoryTitle = json.ProductsCategories[0].Title;
            string productTitle = json.ProductsCategories[0].Products[0].Title;

            Assert.AreEqual("Boxers", categoryTitle);
            Assert.AreEqual("Boxer 95/5", productTitle);
        }

        [TestMethod]
        public void add_to_subscription_returns_new_token_and_cart()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);
            
            // Act
            JsonResult result = controller.AddToSubscription(4, 1);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual(1, model.SelectedProducts.Count());
            Assert.AreEqual((9 + defaultShippingFee).ToCurrency(), model.SelectionTotal);
            Assert.IsInstanceOfType(model.SubscriptionId, typeof(int));
            Assert.AreEqual(36, model.Token.Length);
        }

        [TestMethod]
        public void logged_out_user_add_to_subscription_returns_new_token_and_cart()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            JsonResult result = controller.AddToSubscription(4, 1);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual(1, model.SelectedProducts.Count());
            Assert.AreEqual((9 + defaultShippingFee).ToCurrency(), model.SelectionTotal);
            Assert.IsInstanceOfType(model.SubscriptionId, typeof(int));
            Assert.AreEqual(36, model.Token.Length);
        }

        [TestMethod]
        public void apply_coupon_percentage_updates_total()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            JsonResult result = controller.ApplyCoupon("mario");
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual((45.90M).ToCurrency(), model.SelectionTotal);
        }

        [TestMethod]
        public void apply_coupon_amount_updates_total()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            JsonResult result = controller.ApplyCoupon("yop");
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual((46M).ToCurrency(), model.SelectionTotal);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Trying to add an invalid coupon. Coupons must have an amount OR a percentage but never both")]
        public void apply_invalid_coupon_throws_exception()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            JsonResult result = controller.ApplyCoupon("wrongcoupon");
        }

        [TestMethod]
        public void apply_nonexisting_coupon_does_not_update_total()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            JsonResult result = controller.ApplyCoupon("whatever coupon");
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual((51M).ToCurrency(), model.SelectionTotal);
        }

        [TestMethod]
        public void add_less_than_shippingtreshold_displays_default_shipping_fee()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);

            // Act
            JsonResult result = controller.AddToSubscription(4, 1);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual(defaultShippingFee.ToCurrency(), model.ShippingFee);
        }

        [TestMethod]
        public void add_more_than_shippingtreshold_displays_free_shipping()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            controller.AddToSubscription(8, 3);
            JsonResult result = controller.AddToSubscription(4, 1);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual(0M.ToCurrency(), model.ShippingFee);
        }

        [TestMethod]
        public void getsubscription_with_unsubscribed_user_returns_cart()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            JsonResult result = controller.GetSubscription();
            SubscriptionSelectionViewModel model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.AreEqual(5, model.SelectedProducts.Count());
            Assert.AreEqual("BoxerDim", model.SelectedProducts.First().ProductReference);
        }

        [TestMethod]
        public void getsubscription_with_subscribed_user_returns_pending_selection()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            JsonResult result = controller.GetSubscription();
            SubscriptionSelectionViewModel model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            Assert.IsTrue(model.IsSubscribed);
            Assert.AreEqual(3, model.SelectedProducts.Count());
            Assert.IsTrue(model.SelectedProducts.Any(p => p.ProductReference == "SocksIns"));
        }

        [TestMethod]
        public void logged_in_subscribed_adds_to_cart_creates_a_new_delivery()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            controller.AddToSubscription(8, 3);
            JsonResult result = controller.AddToSubscription(4, 1);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedToken);
                Assert.IsTrue(sub.SubscriptionDeliveries.Any(d => d.DeliveryStateCV == CodeValues.DeliveryState.New));
                Assert.IsTrue(model.IsSubscribed);
            }
        }

        [TestMethod]
        public void logged_in_subscribed_removes_from_cart_creates_a_new_delivery()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            JsonResult result = controller.RemoveFromSubscription(4);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedToken);
                var newDelivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
                Assert.IsFalse(newDelivery.SubscriptionDeliveryModels.Any(m => m.ModelId == 4));
            }
        }

        [TestMethod]
        public void logged_in_subscribed_adds_pack_creates_a_new_delivery()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            var redirectResult = controller.AddPackToSubscription(1) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("Size", redirectResult.RouteValues["Action"]); // verify redirect

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedToken);
                var newDelivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
                Assert.IsTrue(newDelivery.SubscriptionDeliveryModels.Any(m => m.PackId == 1));
            }
        }

        [TestMethod]
        public void logged_in_subscribed_adds_gift_pack_creates_a_new_one_time_sub()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedToken;

            // Act
            var redirectResult = controller.AddGift(5) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("GiftConfiguration", redirectResult.RouteValues["Action"]); // verify redirect

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedToken).User;
                var giftSub = user.Subscriptions.FirstOrDefault(s => s.Gift != null);

                Assert.IsNotNull(giftSub);

                // TODO: please fix, mock cookie container probably not working 
                //JsonResult result = controller.GetSubscription();
                //var model = result.Data as SubscriptionSelectionViewModel;
                //Assert.AreNotEqual(model.Token, MockHelpers.SubscribedToken); 

                var newDelivery = giftSub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
                Assert.IsTrue(newDelivery.SubscriptionDeliveryModels.Any(m => m.PackId == 5));
            }
        }

        [TestMethod]
        public void new_customer_adds_gift_pack_goes_to_checkout()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(), new RouteData(), controller);
           
            // Act
            var redirectResult = controller.AddGift(5) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("GiftConfiguration", redirectResult.RouteValues["Action"]); // verify redirect
        }

        [TestMethod]
        public void logged_in_user_in_modification_removes_from_new_delivery()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedInModificationDifferentAmountToken, isAuthenticated: true), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedInModificationDifferentAmountToken;

            // Act
            JsonResult result = controller.RemoveFromSubscription(4);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedInModificationDifferentAmountToken);
                var newDelivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
                Assert.IsFalse(newDelivery.SubscriptionDeliveryModels.Any(m => m.ModelId == 4));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "cannot remove an item from an existing subscription when logged out")]
        public void logged_out_user_in_modification_removes_throws_exception()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.SubscribedInModificationDifferentAmountToken, isAuthenticated: false), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.SubscribedInModificationDifferentAmountToken;

            // Act
            JsonResult result = controller.RemoveFromSubscription(4);
            var model = result.Data as SubscriptionSelectionViewModel;

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.SubscribedInModificationDifferentAmountToken);
                var newDelivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
            }
        }

        [TestMethod]
        public void add_pack_redirects_to_size()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.NonSubscribedToken;

            // Act
            var redirectResult = controller.AddPackToSubscription(1) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("Size", redirectResult.RouteValues["Action"]); // verify redirect

            JsonResult result = controller.GetSubscription();
            var model = result.Data as SubscriptionSelectionViewModel;

            Assert.AreEqual(2, model.Pack.SelectedProducts.Count());
            Assert.AreEqual((24.90M).ToCurrency(), model.Pack.Price);
            Assert.AreEqual((75.90M).ToCurrency(), model.SelectionTotal);
            Assert.IsInstanceOfType(model.SubscriptionId, typeof(int));
        }

        [TestMethod]
        public void add_pack_and_duplicate_single_product_removes_duplicate()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.EmptyAnonymousToken), new RouteData(), controller);
            controller.CurrentSubscriptionToken = MockHelpers.EmptyAnonymousToken;

            // Act
            controller.AddToSubscription(229, 1);
            controller.AddPackToSubscription(1);
            

            // Assert 
            JsonResult result = controller.GetSubscription();
            var model = result.Data as SubscriptionSelectionViewModel;

            Assert.AreEqual(2, model.Pack.SelectedProducts.Count());
            Assert.IsTrue(model.SelectedProducts.Count() == 0);
            Assert.AreEqual((24.90M).ToCurrency(), model.Pack.Price);
            Assert.AreEqual((27.90M).ToCurrency(), model.SelectionTotal); // pack + single product 
            Assert.IsInstanceOfType(model.SubscriptionId, typeof(int));
        }

        [TestMethod]
        public void add_a_pack_replaces_any_previous_pack()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);

            // Act
            controller.AddPackToSubscription(1);
            var redirectResult = controller.AddPackToSubscription(2) as RedirectToRouteResult;

            // Assert 
            Assert.AreEqual("Size", redirectResult.RouteValues["Action"]); // verify redirect

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == MockHelpers.NonSubscribedToken);
                var newDelivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                Assert.IsNotNull(newDelivery);
                Assert.IsTrue(newDelivery.SubscriptionDeliveryModels.Any(m => m.PackId == 2));
                Assert.IsFalse(newDelivery.SubscriptionDeliveryModels.Any(m => m.PackId == 1));
            }
        }

        [TestMethod]
        public void removing_pack_from_subscription_removes_pack_from_delivery()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext(MockHelpers.GetFakeContext(MockHelpers.NonSubscribedToken), new RouteData(), controller);

            // Act
            var firstResult = controller.AddPackToSubscription(1);
            JsonResult result = controller.RemovePackFromSubscription(1);
            var model = result.Data as SubscriptionSelectionViewModel;

            Assert.IsNull(model.Pack.SelectedProducts);
        }
    }
}
