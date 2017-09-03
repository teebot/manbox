using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Resources;
using ManBox.Common.Security;
using ManBox.Model;
using ManBox.Model.ViewModels;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ManBox.Business
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly decimal itemsMinimumValue = 1;

        private ILogger logger;
        private IMailService mailService;

        public CheckoutRepository(ILogger loggerInject, IMailService mailServiceInject)
        {
            logger = loggerInject;
            mailService = mailServiceInject;
        }

        public CheckoutShippingViewModel GetSubscriptionCheckoutInfo(string subscrToken)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription completeSub = null;
                var viewModel = new CheckoutShippingViewModel();
                var sub = ent.Subscriptions.FirstOrDefault(s => s.Token == subscrToken);

                if (sub == null)
                {
                    throw new Exception("Subscription not found from checkout");
                }

                viewModel.IsSubscriptionValid = CheckSubscriptionValidity(sub);
                var user = sub.User;

                if (user != null)
                {
                    completeSub = user.Subscriptions.FirstOrDefault(s => s.Address != null);
                    viewModel.Email = user.Email;
                    viewModel.FirstName = user.FirstName;
                    viewModel.LastName = user.LastName;
                    viewModel.Phone = user.Phone;
                }

                if (completeSub != null)
                {
                    viewModel.City = completeSub.Address.City;
                    viewModel.CountryId = completeSub.Address.CountryId;
                    viewModel.PostalCode = completeSub.Address.PostalCode;
                    viewModel.Province = completeSub.Address.Province;
                    viewModel.Street = completeSub.Address.Street;
                }

                var paymentInfo = GetPaymentInfo(sub, ent);
                viewModel.TotalInt = paymentInfo.TotalInt;
                viewModel.Token = subscrToken;
                viewModel.IsSubscriptionValid = CheckSubscriptionValidity(sub);

                if (sub.Gift != null)
                {
                    viewModel.GuestName = sub.Gift.GuestName;
                    viewModel.GiftMsg = sub.Gift.GiftMessage;
                }

                return viewModel;
            }
        }

        private bool CheckSubscriptionValidity(Subscription sub)
        {
            var delivery = Utilities.GetActiveDelivery(sub);
            var itemsTotal = Utilities.GetDeliveryTotal(delivery);

            return itemsTotal >= itemsMinimumValue;
        }


        public PaymentParametersViewModel SaveShippingInfo(CheckoutShippingViewModel shippingInfo, string subscrToken)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = ent.Users.SingleOrDefault(u => u.Email == shippingInfo.Email);
                Subscription sub = ent.Subscriptions.FirstOrDefault(s => s.Token == subscrToken);

                if (user == null)
                {
                    sub.User = new User()
                    {
                        CountryId = shippingInfo.CountryId,
                        CreatedDatetime = DateTime.Now,
                        Email = shippingInfo.Email.ToLower(),
                        FirstName = shippingInfo.FirstName,
                        IsOptin = true,
                        LastName = shippingInfo.LastName,
                        Password = shippingInfo.Password,
                        SignInTypeCV = CodeValues.SignInType.EmailPass,
                        Phone = shippingInfo.Phone,
                        LanguageId = IdHelper.CurrentUser.LanguageId
                    };
                }
                //else if (user.Subscriptions.Any(s => s.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed))
                //{
                //    return new PaymentParametersViewModel() { AlreadyMember = true };
                //}
                else
                {
                    sub.User = user; // user was already in database but was not subscribed yet
                }

                var address = new ManBox.Model.Address()
                {
                    City = shippingInfo.City,
                    FirstName = shippingInfo.FirstName,
                    LastName = shippingInfo.LastName,
                    PostalCode = shippingInfo.PostalCode,
                    Street = shippingInfo.Street,
                    Province = shippingInfo.Province,
                    CountryId = shippingInfo.CountryId
                };

                var shippingAddress = new ManBox.Model.Address()
                {
                    City = shippingInfo.City,
                    FirstName = string.IsNullOrEmpty(shippingInfo.ShippingFirstName) ? shippingInfo.FirstName : shippingInfo.ShippingFirstName,
                    LastName = string.IsNullOrEmpty(shippingInfo.ShippingLastName) ? shippingInfo.LastName : shippingInfo.ShippingLastName,
                    PostalCode = shippingInfo.PostalCode,
                    Street = shippingInfo.Street,
                    Province = shippingInfo.Province,
                    CountryId = shippingInfo.CountryId
                };

                sub.Address = address;
                sub.ShippingAddress = shippingAddress;
                sub.SubscriptionStateCV = CodeValues.SubscriptionState.Checkout;

                ent.SaveChanges();

                return GetPaymentInfo(sub, ent);
            }
        }

        public PaymentParametersViewModel GetPaymentInfo(string subscrToken)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = ent.Subscriptions.FirstOrDefault(s => s.Token == subscrToken);
                return GetPaymentInfo(sub, ent);
            }
        }

        public void StorePreapprovalAndCharge(PayPalResponseViewModel response, string thankYouUrl, string cancelUrl)
        {
            try
            {
                logger.Log(LogType.Info, "Trying to process response: " + response.ToString());

                if (string.IsNullOrEmpty(response.Sender_Email) || string.IsNullOrEmpty(response.Preapproval_Key))
                {
                    throw new Exception("send mail or preapproval key missing");
                }

                using (ManBoxEntities ent = new ManBoxEntities())
                {
                    var sub = ent.Subscriptions.Single(s => s.SubscriptionId == response.SubscriptionId);
                    sub.PayPalPreapprovalKey = response.Preapproval_Key;
                    sub.PayPalSenderEmail = response.Sender_Email;
                    ent.SaveChanges();

                    bool paymentSuccess = false;
#if DEBUG
                    paymentSuccess = true;
#else
                    paymentSuccess = ChargePayPalSuccess(sub, ent, thankYouUrl, cancelUrl);
#endif

                    if (paymentSuccess)
                    {
                        var delivery = sub.SubscriptionDeliveries
                                        .OrderByDescending(d => d.QueuedDatetime)
                                        .FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);

                        sub.IsActive = true;
                        sub.SubscriptionStateCV = CodeValues.SubscriptionState.Subscribed;
                        delivery.DeliveryStateCV = CodeValues.DeliveryState.Processing;
                        delivery.DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.Paid;

                        if (delivery.Coupon != null)
                        {
                            delivery.Coupon.NumberAvailable--;
                        }

                        ent.SaveChanges();

                        SendSubscriptionConfirmationMail(sub, ent);
                    }
                    else
                    {
                        logger.Log(LogType.Error, "could not successfully charge with the preapproval key");
                    }
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("send mail: {0} - memo: {1} - exception: {2}", response.Sender_Email, response.Memo, e.Message);
                logger.Log(LogType.Error, e.Message + msg);
            }
        }

        public void SaveGiftPersonalization(string tk, string guestName, string giftMsg)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == tk);
                var giftMsgMaxLength = giftMsg.Length > 160 ? 160 : giftMsg.Length;
                var guestNameMaxLength = guestName.Length > 50 ? 50 : guestName.Length;
                giftMsg = giftMsg.Substring(0, giftMsgMaxLength);
                guestName = guestName.Substring(0, guestNameMaxLength);

                if (sub.Gift == null)
                {
                    sub.Gift = new Gift() { GiftMessage = giftMsg, GuestName = guestName };
                }
                else {
                    sub.Gift.GiftMessage = giftMsg;
                    sub.Gift.GuestName = guestName;
                }
                ent.SaveChanges();
            }
        }

        private bool ChargePayPalSuccess(Subscription sub, ManBoxEntities ent, string thankYouUrl, string cancelUrl)
        {
            var payPalAccount = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PayPalAccount] as string;
            var paymentInfo = GetPaymentInfo(sub, ent);

            var receiverList = new ReceiverList();
            receiverList.receiver = new List<Receiver>();
            receiverList.receiver.Add(new Receiver(paymentInfo.Total) { email = payPalAccount });

            var service = new AdaptivePaymentsService();
            PayResponse response = service.Pay(new PayRequest(
                                        new RequestEnvelope("en_US"),
                                        "PAY",
                                        cancelUrl,
                                        "EUR",
                                        receiverList,
                                        thankYouUrl)
            {
                senderEmail = sub.PayPalSenderEmail,
                preapprovalKey = sub.PayPalPreapprovalKey
            });

            if (response == null)
            {
                logger.Log(LogType.Fatal, "No Response was received from PayPal Payrequest service");
            }

            logger.Log(LogType.Info, string.Format("paykey is {0} . exec status is {1}", response.payKey ?? "", response.paymentExecStatus ?? ""));

            // error handling
            if (response.error != null && response.error.FirstOrDefault() != null)
            {
                logger.Log(LogType.Error, string.Format("error {0}", response.error.FirstOrDefault().message));
            }

            // error handling
            if (response.payErrorList != null && response.payErrorList.payError.FirstOrDefault() != null)
            {
                logger.Log(LogType.Error, string.Format("payerror {0}", response.payErrorList.payError.FirstOrDefault().error.message));
            }

            //payment exec status must be : COMPLETED
            if (!string.IsNullOrEmpty(response.paymentExecStatus)
                    && response.paymentExecStatus.ToLower() == PayPalConstants.PaymentExecStatus.Completed.ToLower())
            {
                return true;
            }

            return false;
        }

        public void ConfirmPaymillPayment(string subscrToken, string paymillToken, string paymillClientId, string paymillPayId)
        {
            try
            {
                logger.Log(LogType.Info, "Confirming paymill payment");

                using (ManBoxEntities ent = new ManBoxEntities())
                {
                    var sub = ent.Subscriptions.Single(s => s.Token == subscrToken);
                    sub.PaymillClientId = paymillClientId;
                    sub.PaymillPayId = paymillPayId;
                    sub.PaymillToken = paymillToken;
                    sub.IsActive = true;
                    sub.SubscriptionStateCV = CodeValues.SubscriptionState.Subscribed;

                    var delivery = sub.SubscriptionDeliveries
                                    .OrderByDescending(d => d.QueuedDatetime)
                                    .FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                    delivery.DeliveryStateCV = CodeValues.DeliveryState.Processing;
                    delivery.DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.Paid;

                    if (delivery.Coupon != null)
                    {
                        delivery.Coupon.NumberAvailable--;
                    }

                    ent.SaveChanges();

                    SendSubscriptionConfirmationMail(sub, ent);
                }
            }
            catch (Exception e)
            {
                var msg = string.Format(" could not store paymill data!");
                logger.Log(LogType.Error, e.Message + msg);
            }
        }

        public void CancelModification(string token)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.FirstOrDefault(s => s.Token == token);
                if (sub.SubscriptionStateCV != CodeValues.SubscriptionState.Subscribed)
                {
                    throw new Exception("Error: you cannot cancel a modification if you are not subscribed");
                }

                var newDelivery = sub.SubscriptionDeliveries.FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                newDelivery.DeliveryStateCV = CodeValues.DeliveryState.Dropped;
                ent.SaveChanges();
            }
        }

        public ModificationResponseViewModel ConfirmModification(string token)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.FirstOrDefault(s => s.Token == token);
                if (sub.SubscriptionStateCV != CodeValues.SubscriptionState.Subscribed)
                {
                    throw new Exception("Error: you cannot confirm a modification if you are not subscribed");
                }

                var newDelivery = sub.SubscriptionDeliveries.FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
                var pendingDelivery = sub.SubscriptionDeliveries.FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.Pending);

                var newDeliveryTotal = Utilities.GetDeliveryTotal(newDelivery);
                var pendingDeliveryTotal = Utilities.GetDeliveryTotal(pendingDelivery);

                // save it right away 
                newDelivery.DeliveryStateCV = CodeValues.DeliveryState.Pending;
                pendingDelivery.DeliveryStateCV = CodeValues.DeliveryState.Dropped;
                ent.SaveChanges();

                // return success message
                return new ModificationResponseViewModel()
                {
                    IsPaymentNeeded = false
                };
            }
        }

        private static PaymentParametersViewModel GetPaymentInfo(Subscription sub, ManBoxEntities ent)
        {
            PaymentParametersViewModel payParams = new PaymentParametersViewModel();
            var delivery = Utilities.GetActiveDelivery(sub);
            var itemsTotal = Utilities.GetDeliveryTotal(delivery);
            var couponAmount = Utilities.CalculateCouponAmount(itemsTotal, delivery);
            var shippingFee = Utilities.CalculateShippingFee(itemsTotal);
            var total = itemsTotal + shippingFee - couponAmount;

            delivery.ShippingFee = shippingFee;
            delivery.CouponAmount = couponAmount;
            delivery.Amount = total;

            ent.SaveChanges();

            payParams.SubscriptionId = sub.SubscriptionId;
            payParams.Total = total;

            if (sub.User != null)
            {
                payParams.User = new UserViewModel()
                {
                    Email = sub.User.Email,
                    Token = sub.Token,
                    FirstName = sub.User.FirstName,
                    LastName = sub.User.LastName,
                    UserId = sub.User.UserId
                };
            }
            return payParams;
        }

        private void SendSubscriptionConfirmationMail(Subscription sub, ManBoxEntities ent)
        {
            bool isAGift = sub.Gift != null;

            try
            {
                var delivery = sub.SubscriptionDeliveries.FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.Processing); //because the first delivery is directly enqueued as Processing not Pending
                if (delivery == null)
                {
                    logger.Log(LogType.Error, "Subscription Delivery is null");
                }

                var itemsTotal = Utilities.GetDeliveryTotal(delivery);
                var shippingFee = Utilities.CalculateShippingFee(itemsTotal);
                var couponAmount = Utilities.CalculateCouponAmount(itemsTotal, delivery);
                var total = itemsTotal + shippingFee - couponAmount;

                List<MailProduct> mailProducts = new List<MailProduct>();
                delivery.SubscriptionDeliveryModels.ToList().ForEach(m =>
                    mailProducts.Add(
                        new Common.Mail.Models.MailProduct()
                        {
                            Price = m.Model.ShopPrice,
                            Quantity = m.Quantity,
                            ProductName = (from tt in ent.TranslationTexts
                                           where tt.LanguageId == IdHelper.CurrentUser.LanguageId
                                            && tt.TranslationId == m.Model.Product.TitleTrId
                                           select tt.Text).FirstOrDefault(),
                            ModelName = m.Model.Name,
                            TotalPrice = m.Quantity * m.Model.ShopPrice
                        })
                    );

                var domain = Utilities.GetCountryDomain(sub.User.Country.IsoCode);
                var toEmail = new MailRecipient(sub.User.Email, sub.User.FirstName);
                var fromEmail = new MailRecipient("support@" + domain, "ManBox");

                if (!isAGift)
                {
                    mailService.SendMail<SubscriptionConfirmationMail>(toEmail,
                        fromEmail,
                        new SubscriptionConfirmationMail()
                        {
                            RootUrl = "http://" + domain,
                            LanguageIso = sub.User.Language.IsoCode,
                            Date = DateTime.Now,
                            Name = sub.User.FirstName,
                            Subject = UITexts.MailShipmentConfirmationSubject,
                            Products = mailProducts,
                            SubTotal = itemsTotal,
                            Total = total,
                            ShippingFee = shippingFee,
                            CouponAmount = couponAmount,
                            CouponLabel = Utilities.GetCouponLabel(delivery.Coupon),
                            Address = new MailAddress()
                            {
                                FirstName = sub.ShippingAddress.FirstName,
                                LastName = sub.ShippingAddress.LastName,
                                Street = sub.ShippingAddress.Street,
                                PostalCode = sub.ShippingAddress.PostalCode,
                                Province = sub.ShippingAddress.Province,
                                City = sub.ShippingAddress.City
                            }
                        });
                }
                else {
                    mailService.SendMail<GiftConfirmationMail>(toEmail,
                        fromEmail,
                        new GiftConfirmationMail()
                        {
                            RootUrl = "http://" + domain,
                            LanguageIso = sub.User.Language.IsoCode,
                            Date = DateTime.Now,
                            Name = sub.User.FirstName,
                            Subject = UITexts.MailShipmentConfirmationSubject,
                            Products = mailProducts,
                            SubTotal = itemsTotal,
                            Total = total,
                            ShippingFee = shippingFee,
                            CouponAmount = couponAmount,
                            CouponLabel = Utilities.GetCouponLabel(delivery.Coupon),
                            GuestName = sub.Gift.GuestName,
                            GiftMessage = sub.Gift.GiftMessage,
                            Address = new MailAddress()
                            {
                                FirstName = sub.ShippingAddress.FirstName,
                                LastName = sub.ShippingAddress.LastName,
                                Street = sub.ShippingAddress.Street,
                                PostalCode = sub.ShippingAddress.PostalCode,
                                Province = sub.ShippingAddress.Province,
                                City = sub.ShippingAddress.City
                            }
                        });
                }
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }
    }
}
