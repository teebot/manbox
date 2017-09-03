using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManBox.Business.Batch;
using ManBox.Common.UnitTesting;
using ManBox.Common.Mail.Models;
using ManBox.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManBox.Common;
using ManBox.Common.Injection;

namespace ManBox.Business.Tests
{
    [TestClass]
    public class BatchServiceTests : ManBoxBusinessTestBase
    {
        private IBatchRepository _repo = null;

        [TestInitialize]
        public void Setup()
        {
            _repo = DependencyContainer.GetInstance().Resolve<IBatchRepository>();
        }

        [TestMethod]
        public void upcoming_notification_mail_send_is_stored_in_message_history()
        {
            // Act
            _repo.SendUpcomingBoxNotifications();

            // Assert
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                // message history entry was created
                var msg = ent.SubscriptionDeliveryMessages.FirstOrDefault(m => m.SubscriptionDeliveryId == 123);
                Assert.AreEqual(CodeValues.DeliveryMessageType.Upcoming, msg.DeliveryMessageTypeCV);
            }
        }

        [TestMethod]
        public void upcoming_notification_mail_not_sent_if_already_sent()
        {
            // Act
            var firstRunResult = _repo.SendUpcomingBoxNotifications();
            var secondRunResult =_repo.SendUpcomingBoxNotifications();

            // Assert
            Assert.IsTrue(firstRunResult.NotificationsSent > 0);
            Assert.IsTrue(secondRunResult.NotificationsSent == 0);

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                // message history entry was created
                var msg = ent.SubscriptionDeliveryMessages.First(m => m.SubscriptionDeliveryId == 123 && m.DeliveryMessageTypeCV == CodeValues.DeliveryMessageType.Upcoming);
                Assert.IsNotNull(msg);
            }


        }
    }
}
