using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Resources;
using ManBox.Common.Security;
using ManBox.Model;
using ManBox.Model.ViewModels.BackOffice;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace ManBox.Business.BackOffice
{
    public class ShipmentRepository : IShipmentRepository
    {
        private ILogger logger;
        private IMailService mailService;

        public ShipmentRepository(ILogger loggerInject, IMailService mailServiceInject)
        {
            logger = loggerInject;
            mailService = mailServiceInject;
        }

        public ShipmentListViewModel GetShipmentsList()
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                ent.Configuration.LazyLoadingEnabled = true;

                var deliveries = (from sd in ent.SubscriptionDeliveries.Include(d => d.Subscription.User)
                                  where sd.Subscription.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed
                                  && sd.DeliveryStateCV != CodeValues.DeliveryState.Dropped
                                  && sd.DeliveryStateCV != CodeValues.DeliveryState.New
                                  && sd.DeliveryStateCV != CodeValues.DeliveryState.Sent
                                  orderby sd.DeliveryDate
                                  select sd).ToList();

                var shipmentList = new ShipmentListViewModel()
                {
                    Deliveries = deliveries
                };

                return shipmentList;
            }
        }

        public ShipmentDetailViewModel GetDeliveryDetails(int deliveryId)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var shipmentDetail = (from sd in ent.SubscriptionDeliveries
                                      where sd.SubscriptionDeliveryId == deliveryId
                                      join s in ent.Subscriptions on sd.SubscriptionId equals s.SubscriptionId
                                      join a in ent.Addresses on s.ShippingAddressId equals a.AddressId
                                      join u in ent.Users on s.UserId equals u.UserId
                                      select new ShipmentDetailViewModel()
                                      {
                                          Delivery = sd,
                                          DeliveryId = sd.SubscriptionDeliveryId,
                                          DeliveryState = sd.DeliveryStateCV,
                                          DeliveryPaymentStatus = sd.DeliveryPaymentStatusCV,
                                          FirstName = a.FirstName,
                                          LastName = a.LastName,
                                          Street = a.Street,
                                          Email = u.Email,
                                          PostalCode = a.PostalCode,
                                          City = a.City,
                                          PayPalPreApprovalKey = s.PayPalPreapprovalKey,
                                          PayPalSenderEmail = s.PayPalSenderEmail,
                                          UserLanguage = u.Language.IsoCode,
                                          Country = ent.Countries.FirstOrDefault(c => c.CountryId == a.CountryId).Name,
                                          OrderedProducts = (from dm in sd.SubscriptionDeliveryModels
                                                             join m in ent.Models on dm.ModelId equals m.ModelId
                                                             where dm.SubscriptionDeliveryId == sd.SubscriptionDeliveryId
                                                             select new ShipmentModelViewModel()
                                                             {
                                                                 ModelId = dm.ModelId,
                                                                 ModelName = dm.Model.Name,
                                                                 ModelReference = dm.Model.Reference,
                                                                 BrandName = dm.Model.Product.Brand.Name,
                                                                 ProductName = (from tt in ent.TranslationTexts
                                                                                where tt.LanguageId == u.LanguageId
                                                                                 && tt.TranslationId == dm.Model.Product.TitleTrId
                                                                                select tt.Text).FirstOrDefault(),
                                                                 AmountInStock = dm.Model.AmountInStock.Value,
                                                                 Quantity = dm.Quantity,
                                                                 UnitPrice = dm.Model.ShopPrice,
                                                                 PackName = dm.Pack == null ? null : (from tt in ent.TranslationTexts
                                                                                                      where tt.LanguageId == u.LanguageId
                                                                                                      && tt.TranslationId == dm.Pack.TitleTrId
                                                                                                      select tt.Text).FirstOrDefault()
                                                             })
                                      }).First();

                decimal beforeDiscountPrice = 0;
                shipmentDetail.OrderedProducts.ToList().ForEach(dm => beforeDiscountPrice += (dm.UnitPrice * dm.Quantity));

                var deliveryTotal = Utilities.GetDeliveryTotal(shipmentDetail.Delivery);
                var shippingFee = Utilities.CalculateShippingFee(deliveryTotal);
                var couponAmount = Utilities.CalculateCouponAmount(deliveryTotal, shipmentDetail.Delivery);
                var total = deliveryTotal + shippingFee - couponAmount;

                shipmentDetail.BeforeDiscountPrice = beforeDiscountPrice;
                shipmentDetail.Amount = deliveryTotal;
                shipmentDetail.TotalAmount = total;
                shipmentDetail.ShippingFee = shippingFee;
                shipmentDetail.CouponAmount = couponAmount;

                return shipmentDetail;
            }
        }

        /// <summary>
        /// sets correct delivery amounts 
        /// gets the preapproval key of the paypal sender
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <returns></returns>
        public ShipmentPaymentViewModel GetDeliveryPaymentInfo(int deliveryId)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var delivery = ent.SubscriptionDeliveries.Single(d => d.SubscriptionDeliveryId == deliveryId);

                decimal deliveryAmount = 0;
                delivery.SubscriptionDeliveryModels.ToList().ForEach(dm => deliveryAmount += (dm.Model.ShopPrice * dm.Quantity));

                var shippingFee = Utilities.CalculateShippingFee(deliveryAmount);
                var couponAmount = Utilities.CalculateCouponAmount(deliveryAmount, delivery);
                var total = deliveryAmount + shippingFee - couponAmount;

                delivery.Amount = deliveryAmount;
                delivery.ShippingFee = shippingFee;
                delivery.CouponAmount = couponAmount;
                ent.SaveChanges();

                var shipmentPaymentModel = new ShipmentPaymentViewModel()
                {
                    Total = total,
                    TotalInt = Convert.ToInt32(total * 100),
                    PayPalSenderEmail = delivery.Subscription.PayPalSenderEmail,
                    PayPalPreapprovalKey = delivery.Subscription.PayPalPreapprovalKey,
                    PaymillPayId = delivery.Subscription.PaymillPayId
                };

                return shipmentPaymentModel;
            }
        }

        public void ConfirmShipmentSent(int deliveryId, bool notify)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var delivery = ent.SubscriptionDeliveries.Single(d => d.SubscriptionDeliveryId == deliveryId);
                delivery.DeliveryStateCV = CodeValues.DeliveryState.Sent;
                delivery.DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.Paid;

                // add next delivery
                delivery.Subscription.SubscriptionDeliveries.Add(GetCopyPendingDelivery(delivery));
                
                if (notify)
                {
                    SendShipmentConfirmationMail(delivery, ent);
                    ShipmentRepository.StoreDeliveryMessage(delivery, CodeValues.DeliveryMessageType.ShippingConfirmation);
                }

                ent.SaveChanges();

                // updates stock
                UpdateStock(delivery, ent);
            }
        }

        public void UpdateShipmentPaymentStatus(int deliveryId, string deliveryPaymentStatus)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var delivery = ent.SubscriptionDeliveries.Single(d => d.SubscriptionDeliveryId == deliveryId);

                if (CodeValues.DeliveryPaymentStatus.GetAll().Any(s => s == deliveryPaymentStatus))
                {
                    throw new Exception("Unexpected codevalue for deliverypaymentstatus");
                }

                delivery.DeliveryPaymentStatusCV = deliveryPaymentStatus;
                ent.SaveChanges();
            }
        }

        public static void StoreDeliveryMessage(SubscriptionDelivery delivery, string messageTypeCV)
        {
            if (!CodeValues.DeliveryMessageType.GetAll().Any(c => c == messageTypeCV))
            {
                throw new Exception("Unexpected codevalue for deliverymessage");
            }

            delivery.SubscriptionDeliveryMessages.Add(new SubscriptionDeliveryMessage()
            {
                DeliveryMessageTypeCV = messageTypeCV,
                SentDatetime = DateTime.Now
            });
        }

        #region Utilities

        private void UpdateStock(SubscriptionDelivery delivery, ManBoxEntities ent)
        {
            foreach (var sm in delivery.SubscriptionDeliveryModels)
            {
                sm.Model.AmountInStock -= sm.Quantity;
            }

            ent.SaveChanges();
        }

        private SubscriptionDelivery GetCopyPendingDelivery(SubscriptionDelivery delivery)
        {
            var newDelivery = new SubscriptionDelivery()
            {
                DeliveryStateCV = CodeValues.DeliveryState.Pending,
                DeliveryDate = delivery.DeliveryDate.Value.AddMonths(3),
                DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.None,
                QueuedDatetime = DateTime.Now,
                ShippingFee = delivery.ShippingFee
            };

            delivery.SubscriptionDeliveryModels.ToList().ForEach(m =>
                newDelivery.SubscriptionDeliveryModels.Add(new SubscriptionDeliveryModel()
                {
                    ModelId = m.ModelId,
                    Quantity = m.Quantity,
                    PackId = m.PackId
                })
            );

            return newDelivery;
        }

        private void SendShipmentConfirmationMail(SubscriptionDelivery delivery, ManBoxEntities ent)
        {
            try
            {
                var itemsTotal = delivery.Amount;
                var shippingFee = delivery.ShippingFee;
                var total = itemsTotal + shippingFee - delivery.CouponAmount;
                var sub = delivery.Subscription;

                List<MailProduct> mailProducts = new List<MailProduct>();
                delivery.SubscriptionDeliveryModels.ToList().ForEach(m =>
                    mailProducts.Add(
                        new Common.Mail.Models.MailProduct()
                        {
                            Price = m.Model.ShopPrice,
                            Quantity = m.Quantity,
                            ProductName = (from tt in ent.TranslationTexts 
                                           where tt.TranslationId == m.Model.Product.TitleTrId 
                                            && tt.LanguageId == sub.User.LanguageId 
                                           select tt.Text).FirstOrDefault(),
                            ModelName = m.Model.Name,
                            TotalPrice = m.Quantity * m.Model.ShopPrice
                        })
                    );

                var domain = Utilities.GetCountryDomain(sub.User.Country.IsoCode);
                var toEmail = new MailRecipient(sub.User.Email, sub.User.FirstName);
                var fromEmail = new MailRecipient("support@" + domain, "ManBox");

                mailService.SendMail<ShipmentConfirmationMail>(
                    toEmail,
                    fromEmail,
                    new ShipmentConfirmationMail()
                    {
                        RootUrl = "http://" + domain,
                        LanguageIso = sub.User.Language.IsoCode,
                        Date = DateTime.Now,
                        Name = sub.User.FirstName,
                        Subject = UITexts.ResourceManager.GetString("MailShipmentConfirmationSubject", CultureInfo.CreateSpecificCulture(sub.User.Language.IsoCode)),
                        Products = mailProducts,
                        SubTotal = itemsTotal,
                        Total = total,
                        ShippingFee = shippingFee,
                        CouponAmount = delivery.CouponAmount,
                        CouponLabel = Utilities.GetCouponLabel(delivery.Coupon),
                        Address = new MailAddress()
                        {
                            FirstName = sub.Address.FirstName,
                            LastName = sub.Address.LastName,
                            Street = sub.Address.Street,
                            PostalCode = sub.Address.PostalCode,
                            Province = sub.Address.Province,
                            City = sub.Address.City
                        }
                    });
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }

        #endregion
    }
}
