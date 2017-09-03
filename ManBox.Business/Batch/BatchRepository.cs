using ManBox.Business.BackOffice;
using ManBox.Common;
using ManBox.Common.Injection;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Resources;
using ManBox.Common.Security;
using ManBox.Model;
using ManBox.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ManBox.Business.Batch
{
    public class BatchRepository : IBatchRepository
    {
        private readonly int daysBefore = 7;

        IMailService mailService;
        ILogger logger;

        public BatchRepository()
        {
            mailService = DependencyContainer.GetInstance().Resolve<IMailService>();
            logger = DependencyContainer.GetInstance().Resolve<ILogger>();
        }

        public NotificationResultViewModel SendUpcomingBoxNotifications()
        {
            try
            {
                int notificationsSent = 0;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                using (ManBoxEntities ent = new ManBoxEntities())
                {
                    var upcomingDeliveries = from sd in ent.SubscriptionDeliveries
                                      where sd.DeliveryStateCV == CodeValues.DeliveryState.Pending
                                      && sd.Subscription.GiftId == null 
                                      && sd.Subscription.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed
                                      && EntityFunctions.AddDays(sd.DeliveryDate.Value, -daysBefore).Value < DateTime.Now
                                      && !sd.SubscriptionDeliveryMessages.Any(m => m.DeliveryMessageTypeCV == CodeValues.DeliveryMessageType.Upcoming)
                                      select new
                                      {
                                          SubscriptionDelivery = sd,
                                          Name = sd.Subscription.User.FirstName,
                                          Token = sd.Subscription.Token,
                                          Email = sd.Subscription.User.Email,
                                          LangIso = sd.Subscription.User.Language.IsoCode,
                                          CountryIso = sd.Subscription.User.Country.IsoCode,
                                          Address = sd.Subscription.Address,
                                          Products = from m in sd.SubscriptionDeliveryModels
                                                     select new
                                                     {
                                                         ProductName = (from tt in ent.TranslationTexts where tt.TranslationId == m.Model.Product.TitleTrId && tt.LanguageId == sd.Subscription.User.LanguageId select tt.Text).FirstOrDefault(),
                                                         ModelName = m.Model.Name,
                                                         Quantity = m.Quantity,
                                                         Price = m.Model.ShopPrice,
                                                         TotalPrice = m.Model.ShopPrice * m.Quantity
                                                     }
                                      };

                    var notificationMails = new List<UpcomingBoxNotificationMail>();

                    foreach (var del in upcomingDeliveries)
                    {
                        var itemsTotal = Utilities.GetDeliveryTotal(del.SubscriptionDelivery);
                        var shippingFee = Utilities.CalculateShippingFee(itemsTotal);
                        var couponAmount = Utilities.CalculateCouponAmount(itemsTotal, del.SubscriptionDelivery);
                        var total = itemsTotal + shippingFee - couponAmount;
                        var couponLabel = Utilities.GetCouponLabel(del.SubscriptionDelivery.Coupon);

                        var products = new List<MailProduct>();
                        var notificationMail = new UpcomingBoxNotificationMail()
                        {
                            Name = del.Name,
                            Email = del.Email,
                            Token = del.Token,
                            LanguageIso = del.LangIso,
                            CountryIso = del.CountryIso,
                            Address = new MailAddress()
                            {
                                City = del.Address.City,
                                Street = del.Address.Street,
                                Province = del.Address.Province,
                                PostalCode = del.Address.PostalCode
                            },
                            SubTotal = itemsTotal,
                            Total = total,
                            ShippingFee = shippingFee,
                            CouponAmount = couponAmount,
                            CouponLabel = couponLabel
                        };

                        foreach (var prod in del.Products)
                        {
                            products.Add(new MailProduct()
                            {
                                ModelName = prod.ModelName,
                                ProductName = prod.ProductName,
                                Price = prod.Price,
                                Quantity = prod.Quantity,
                                TotalPrice = prod.TotalPrice
                            });
                        }

                        notificationMail.Products = products;

                        SendMail(notificationMail);
                        ShipmentRepository.StoreDeliveryMessage(del.SubscriptionDelivery, CodeValues.DeliveryMessageType.Upcoming);

                        notificationsSent++;
                    }

                    sw.Stop();
                    ent.SaveChanges();
                }

                return new NotificationResultViewModel()
                {
                    NotificationsSent = notificationsSent,
                    EndedDateTime = DateTime.Now,
                    ElapsedTime = sw.Elapsed,
                    MessageType = CodeValues.DeliveryMessageType.Upcoming
                };
            }
            catch (Exception e)
            {
                logger.Log(e);
                throw;
            }
        }

        private void SendMail(UpcomingBoxNotificationMail notificationMail)
        {
            var domain = Utilities.GetCountryDomain(notificationMail.CountryIso);
            var toEmail = new MailRecipient(notificationMail.Email, notificationMail.Name);
            var fromEmail = new MailRecipient("thibaut@" + domain, "ManBox");
            notificationMail.Subject = UITexts.ResourceManager.GetString("UpcomingBoxNotificationMailSubject", new System.Globalization.CultureInfo(notificationMail.LanguageIso));

            var encryptedToken = HttpUtility.UrlEncode(TokenEncrypt.EncryptTokenAsExpiring(notificationMail.Token, DateTime.Now.AddDays(1)));
            notificationMail.LinkToken = string.Format("http://{0}/{1}/Account/TokenLogin?token={2}", domain, notificationMail.LanguageIso, encryptedToken);

            mailService.SendMail<UpcomingBoxNotificationMail>(toEmail, fromEmail, notificationMail);
        }

        
    }
}
