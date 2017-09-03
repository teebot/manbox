using ManBox.Business;
using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Resources;
using ManBox.Model.ViewModels;
using PaymillWrapper;
using PaymillWrapper.Models;
using PaymillWrapper.Service;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ManBox.Web.Controllers
{

    [RequireHttps]
    public class CheckoutController : ManBoxControllerBase
    {
        private readonly ICheckoutRepository checkoutRepository;

        public CheckoutController(ICheckoutRepository checkoutRepoInject)
        {
            checkoutRepository = checkoutRepoInject;
        }

        public ActionResult Size()
        {
            var model = checkoutRepository.GetSubscriptionCheckoutInfo(this.CurrentSubscriptionToken);
            return View(model);
        }

        public ActionResult GiftConfiguration()
        {
            var model = checkoutRepository.GetSubscriptionCheckoutInfo(this.CurrentSubscriptionToken);

            if (string.IsNullOrEmpty(model.GiftMsg))
            {
                model.GiftMsg = UITexts.GiftDefaultMessage;
            }

            return View(model);
        }

        public void GoToPayPal(PaymentParametersViewModel paypalInfo)
        {
            try
            {
                var thankYouUrl = Url.Action("ThankYou", "Checkout", null, "https");
                var cancelUrl = Url.Action("Compose", "Box", null, "http");

                PreapprovalRequest req = new PreapprovalRequest()
                {
                    requestEnvelope = new RequestEnvelope("en_US"),
                    cancelUrl = cancelUrl,
                    currencyCode = "EUR",
                    returnUrl = thankYouUrl,
                    startingDate = DateTime.Now.ToString(PayPalConstants.Formatting.ZuluDate),
                    displayMaxTotalAmount = false,
                    endingDate = DateTime.Now.AddYears(1).AddDays(-1).ToString(PayPalConstants.Formatting.ZuluDate),
                    maxTotalAmountOfAllPayments = 1000,
                    ipnNotificationUrl = Url.Action("ProcessPayPalPreapproval", "Checkout", new { subscriptionId = paypalInfo.SubscriptionId }, "https"),
                    clientDetails = new ClientDetailsType() { customerId = paypalInfo.User.UserId.ToString(), }
                };


                this.Log(LogType.Info, "Paypal IPN url :" + req.ipnNotificationUrl);

                var service = new AdaptivePaymentsService();
                var response = service.Preapproval(req);

                if (response.error != null && response.error.Any())
                {
                    var errorSummary = new StringBuilder();
                    response.error.ForEach(e => errorSummary.AppendLine(string.Format("message: {0} - param: {1}", e.message, e.parameter)));
                    throw new Exception(errorSummary.ToString());
                }

                var preApprovalKey = response.preapprovalKey;

                paypalInfo.Account = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PayPalAccount] as string;
                paypalInfo.GatewayUrl = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PayPalUrl] as string;
                paypalInfo.GatewayUrl += "?cmd=_ap-preapproval&preapprovalkey=" + preApprovalKey;

                Response.Redirect(paypalInfo.GatewayUrl, true);
            }
            catch (Exception e)
            {
                this.Log(LogType.Error, e.Message);
            }
        }

        
        
        public ActionResult Shipping()
        {
            var tk = this.CurrentSubscriptionToken;

            CheckoutShippingViewModel model = null;
            if (string.IsNullOrEmpty(tk))
            {
                this.Log(LogType.Warn, "checkout shipping page was reached without token");
            }
            else
            {
                model = checkoutRepository.GetSubscriptionCheckoutInfo(tk);
                if (!model.IsSubscriptionValid)
                {
                    DisplayLocalizedMessage("InvalidSubscription");
                    return RedirectToAction("Compose", "Box");
                }
            }

            var nextDelivery = DateTime.Now.AddMonths(3).ToString("d MMMM yyyy");

            if (model == null)
            {
                model = new CheckoutShippingViewModel();
            }

            model.CountryId = ManBoxHostInfo.Instance.HostCountryId.Value;
            model.SubscriptionToken = tk;
            model.CountryList = new List<SelectListItem>() {
                    new SelectListItem(){ Text = UITexts.CountryFR, Value = AppConstants.Countries.FR.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryBE, Value = AppConstants.Countries.BE.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryNL, Value = AppConstants.Countries.NL.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryLU, Value = AppConstants.Countries.LU.ToString() }
                };
            model.NextDelivery = nextDelivery;
            model.PaymillPublicKey = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PaymillPublicKey];

            var months = new List<SelectListItem>();
            Enumerable.Range(1, 12).ToList().ForEach(i => months.Add(new SelectListItem(){ Text = i.ToString().PadLeft(2, '0'), Value = i.ToString().PadLeft(2, '0') }));

            var years = new List<SelectListItem>();
            Enumerable.Range(DateTime.Now.Year, 20).ToList().ForEach(i => years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() }));
            years[1].Selected = true;

            model.CCMonthsList = months;
            model.CCYearsList = years;

            return View(model);
        }


        [HttpPost]
        public ActionResult GiftShipping(string guestName, string giftMsg)
        {
            var tk = this.CurrentSubscriptionToken;

            CheckoutShippingViewModel model = null;
            if (string.IsNullOrEmpty(tk))
            {
                this.Log(LogType.Warn, "checkout shipping page was reached without token");
            }
            else
            {
                model = checkoutRepository.GetSubscriptionCheckoutInfo(tk);
                checkoutRepository.SaveGiftPersonalization(tk, guestName, giftMsg);

                if (!model.IsSubscriptionValid)
                {
                    DisplayLocalizedMessage("InvalidSubscription");
                    return RedirectToAction("Compose", "Box");
                }
            }

            if (model == null)
            {
                model = new CheckoutShippingViewModel();
            }

            model.CountryId = ManBoxHostInfo.Instance.HostCountryId.Value;
            model.SubscriptionToken = tk;
            model.CountryList = new List<SelectListItem>() {
                    new SelectListItem(){ Text = UITexts.CountryFR, Value = AppConstants.Countries.FR.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryBE, Value = AppConstants.Countries.BE.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryNL, Value = AppConstants.Countries.NL.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryLU, Value = AppConstants.Countries.LU.ToString() }
                };
            model.PaymillPublicKey = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PaymillPublicKey];
            model.ShippingFirstName = guestName;

            var months = new List<SelectListItem>();
            Enumerable.Range(1, 12).ToList().ForEach(i => months.Add(new SelectListItem() { Text = i.ToString().PadLeft(2, '0'), Value = i.ToString().PadLeft(2, '0') }));

            var years = new List<SelectListItem>();
            Enumerable.Range(DateTime.Now.Year, 20).ToList().ForEach(i => years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() }));
            years[1].Selected = true;

            model.CCMonthsList = months;
            model.CCYearsList = years;

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveShippingInfoCreditCard(CheckoutShippingViewModel shippingInfo)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                ModelState.Remove("Email");
                ModelState.Remove("Password");
                shippingInfo.Email = this.User.Identity.Name.ToLower();
            }

            if (ManBoxHostInfo.Instance.HostCountryId.Value == AppConstants.Countries.BE)
            {
                ModelState.Remove("Province");
                shippingInfo.Province = string.Empty;
            }

            if (this.ModelState.IsValid)
            {
                var payParams = checkoutRepository.SaveShippingInfo(shippingInfo, CurrentSubscriptionToken);

                if (payParams.AlreadyMember)
                {
                    return Json(new { success = false, alreadyMember = true });
                }

                if (!this.User.Identity.IsAuthenticated)
                {
                    this.Authenticate(payParams.User);
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult SaveShippingInfo(CheckoutShippingViewModel shippingInfo)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                ModelState.Remove("Email");
                ModelState.Remove("Password");
                shippingInfo.Email = this.User.Identity.Name.ToLower();
            }

            if (ManBoxHostInfo.Instance.HostCountryId.Value == AppConstants.Countries.BE)
            {
                ModelState.Remove("Province");
                shippingInfo.Province = string.Empty;
            }

            if (this.ModelState.IsValid)
            {
                var payParams = checkoutRepository.SaveShippingInfo(shippingInfo, CurrentSubscriptionToken);

                if (payParams.AlreadyMember)
                {
                    DisplayLocalizedMessage("CheckoutAlreadyMemberMessage");
                    return RedirectToAction("Shipping", "Checkout", new { tk = shippingInfo.SubscriptionToken });
                }

                if (!this.User.Identity.IsAuthenticated)
                {
                    this.Authenticate(payParams.User);
                }


                if (shippingInfo.PaymentMethod == AppConstants.PaymentMethods.PayPal)
                {
#if DEBUG
                    this.ProcessPayPalPreapproval(new PayPalResponseViewModel() { Preapproval_Key = "paykey-99", Sender_Email = shippingInfo.Email, SubscriptionId = payParams.SubscriptionId });
                    return RedirectToAction("ThankYou");
#else
                    GoToPayPal(payParams);
                    return new EmptyResult(); // never reach GoToPayPal redirects
#endif
                }
                else 
                {
                    return RedirectToAction("CreditCardSetup");
                }
            }
            else
            {
                DisplayLocalizedMessage("CheckoutValidationFailedMessage");
                return RedirectToAction("Shipping", "Checkout", new { tk = shippingInfo.SubscriptionToken });
            }
        }

        [HttpPost]
        public void ProcessPayPalPreapproval(PayPalResponseViewModel response)
        {
            var thankYouUrl = Url.Action("ThankYou", "Checkout", null, "https");
            var cancelUrl = Url.Action("Compose", "Box", null, "http");

            checkoutRepository.StorePreapprovalAndCharge(response, thankYouUrl, cancelUrl);
        }

        public ActionResult ThankYou()
        {
            var model = checkoutRepository.GetSubscriptionCheckoutInfo(this.CurrentSubscriptionToken);
            return View(model);
        }

        public ActionResult ThankYouModification()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CancelModification()
        {
            checkoutRepository.CancelModification(this.CurrentSubscriptionToken);
            return RedirectToAction("Compose", "Box");
        }

        [HttpPost]
        public ActionResult ConfirmModification()
        {
            var modificationResponse = checkoutRepository.ConfirmModification(this.CurrentSubscriptionToken);

            if (modificationResponse.IsPaymentNeeded)
            {
                // redirect to paypal
                TempData[WebConstants.TempDataKeys.PaymentParams] = modificationResponse.ModificationPaymentParameters;
                return RedirectToAction("GoToPayPal");
            }
            else
            {
                // confirm that the change has been done
                return RedirectToAction("ThankYouModification");
            }
        }

        public ActionResult CreditCardSetup()
        {
            var model = new CreditCardSetupViewModel()
            {
                PaymentParameters = checkoutRepository.GetPaymentInfo(this.CurrentSubscriptionToken),
                PaymillPublicKey = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PaymillPublicKey]
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult PaymillPay(string paymillToken)
        {
            try
            {
                PaymentParametersViewModel paymentParams = checkoutRepository.GetPaymentInfo(this.CurrentSubscriptionToken);

                Paymill.ApiKey = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PaymillApiKey];
                Paymill.ApiUrl = ConfigurationManager.AppSettings[AppConstants.AppSettingsKeys.PaymillApiUrl];

                TransactionService transactionService = Paymill.GetService<TransactionService>();
                Transaction transaction = new Transaction();
                transaction.Token = paymillToken;
                transaction.Amount = paymentParams.TotalInt;
                transaction.Currency = "EUR";
                transaction.Description = "ManBox Subscription";

                var result = transactionService.AddTransaction(transaction);

                if (result.Status == Transaction.TypeStatus.CLOSED)
                {
                    checkoutRepository.ConfirmPaymillPayment(this.CurrentSubscriptionToken, paymillToken, result.Client.Id, result.Payment.Id);
                    return RedirectToAction("ThankYou");
                }
                else
                {
                    this.Log(LogType.Warn, "Failed Paymill payment");
                    this.Log(LogType.Warn, string.Format("Failed Paymill payment, status is {0}", result.Status));
                }
            }
            catch (Exception e)
            {
                this.Log(e);
            }

            DisplayMessage("There was a problem with your payment. Please check your credit card number and expiry date.");
            return RedirectToAction("Shipping");
        }
    }
}
