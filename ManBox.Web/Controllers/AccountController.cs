using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ManBox.Business;
using ManBox.Common;
using ManBox.Model.ViewModels;
using ManBox.Common.Security;
using ManBox.Common.Resources;

namespace ManBox.Web.Controllers
{
    public class AccountController : ManBoxControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IShopRepository shopRepository;

        public AccountController(IUserRepository userRepoInject, IShopRepository shopRepoInject)
        {
            userRepository = userRepoInject;
            shopRepository = shopRepoInject;
        }

        [HttpPost]
        public ActionResult SubscribeNewsletterEmail(SubscribeNewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                userRepository.SubscribeNewsletterEmail(model.Email);
            }

            DisplayLocalizedMessage("SubscribeSuccess");

            return TryRefererRedirect(fallback: RedirectToAction("Compose", "Box"));
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            UserViewModel user = userRepository.Login(login.Email, login.Password, this.CurrentSubscriptionToken);
            if (user != null)
            {
                this.Authenticate(
                    new UserViewModel()
                    {
                        Email = user.Email,
                        UserId = user.UserId,
                        Token = user.Token
                    }
                );
            }
            else
            {
                DisplayLocalizedMessage("LoginFailedMessage");
            }

            return TryRefererRedirect(fallback: RedirectToAction("Compose", "Box"));
        }

        /// <summary>
        /// Login using an encrypted token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult TokenLogin(string token)
        {
            UserViewModel user = userRepository.TokenLogin(token);
            if (user != null)
            {
                this.Authenticate(
                    new UserViewModel()
                    {
                        Email = user.Email,
                        UserId = user.UserId,
                        Token = user.Token
                    }
                );
            }
            else
            {
                DisplayLocalizedMessage("LoginFailedMessage");
            }

            return RedirectToAction("OrdersOverview", "Account");
        }

        [Authorize]
        public ActionResult Password()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdatePassword(PasswordUpdateViewModel newPasswordModel)
        {
            if (ModelState.IsValid)
            {
                userRepository.UpdatePassword(newPasswordModel.NewPassword);
                DisplayLocalizedMessage("AccountPasswordChangeOK");
            }

            return RedirectToAction("Password");
        }

        [Authorize]
        public ActionResult Address()
        {
            AddressViewModel model = userRepository.GetCurrentUserAddress();

            model.CountryList = new List<SelectListItem>() {
                    new SelectListItem(){ Text = UITexts.CountryFR, Value = AppConstants.Countries.FR.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryBE, Value = AppConstants.Countries.BE.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryNL, Value = AppConstants.Countries.NL.ToString() },
                    new SelectListItem(){ Text = UITexts.CountryLU, Value = AppConstants.Countries.LU.ToString() }
                };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateAddress(AddressViewModel updatedAddress)
        {
            if (ManBoxHostInfo.Instance.HostCountryId.Value == AppConstants.Countries.BE)
            {
                ModelState.Remove("Province");
            }

            if (ModelState.IsValid)
            {
                userRepository.UpdateAddress(updatedAddress);
                DisplayLocalizedMessage("AccountAddressChangeOK");
            }
            return RedirectToAction("Address");
        }

        public ActionResult PasswordReset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordReset(ForgotPasswordViewModel data)
        {
            if (this.ModelState.IsValid)
            {
                bool success = userRepository.SendPasswordReset(data.Email);
                if (success)
                {
                    DisplayLocalizedMessage("PasswordResetMailSent");
                }
                else
                {
                    DisplayMessage("Could not find your email in our database");
                }
            }

            return RedirectToAction("Compose", "Box");
        }

        public ActionResult Unsubscribe(string email)
        {
            userRepository.Unsubscribe(email);

            DisplayLocalizedMessage("UnsubscribedMessage");
            return RedirectToAction("Compose", "Box");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(LoginViewModel signup)
        {
            if (this.ModelState.IsValid)
            {
                var user = userRepository.Register(
                    new UserViewModel()
                    {
                        Email = signup.Email,
                        Password = signup.Password,
                        Token = this.CurrentSubscriptionToken
                    },
                    CodeValues.SignInType.EmailPass,
                    this.CurrentSubscriptionToken
                );

                if (user == null)
                {
                    this.DisplayLocalizedMessage("RegisterAlreadyMember");
                    return RedirectToAction("Compose", "Box");
                }

                this.Authenticate(new UserViewModel()
                {
                    Email = user.Email,
                    UserId = user.UserId,
                    Token = user.Token,
                    LanguageId = user.LanguageId,
                    LanguageIsoCode = user.LanguageIsoCode
                });

                return RedirectToAction("Compose", "Box");
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }

        public ActionResult LogOut()
        {
            var auth = DependencyResolver.Current.GetService<IAuthenticationService>();
            auth.SignOut();

            CurrentAuthUser = null;
            CurrentSubscriptionToken = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Contact()
        {
            ContactFormViewModel model = new ContactFormViewModel();
            if (CurrentAuthUser != null && !string.IsNullOrEmpty(CurrentAuthUser.Email))
            {
                model.Email = CurrentAuthUser.Email;
                model.FirstName = CurrentAuthUser.FirstName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SendSupportMail(ContactFormViewModel contactForm)
        {
            if (CurrentAuthUser != null)
            {
                this.ModelState.Remove("Email");
                this.ModelState.Remove("FirstName");

                contactForm.Email = CurrentAuthUser.Email;
                contactForm.FirstName = CurrentAuthUser.FirstName;
            }

            if (this.ModelState.IsValid)
            {
                contactForm.Subject = "Demande de support";
                userRepository.SendSupportMail(contactForm);

                DisplayLocalizedMessage("ContactFormThx");
                return RedirectToAction("Compose", "Box");
            }
            else
            {
                DisplayLocalizedMessage("ContactFormMissingInfo");
                return RedirectToAction("Contact", "Account");
            }
        }

        [Authorize]
        public ActionResult OrdersOverview()
        {
            var model = userRepository.GetOrdersOverview();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReScheduleDelivery(int deliveryId, bool rushNow, int? snoozeWeeks)
        {
            var result = shopRepository.ReScheduleDelivery(deliveryId, rushNow, snoozeWeeks);
            var message = string.Empty;

            if (rushNow)
            {
                if (result.ShippableTomorrow)
                {
                    message = UITexts.DeliveryRushShippableTomorrow;
                }
                else
                {
                    message = UITexts.DeliveryRushShippableAsap;
                }
            }
            else
            {
                if (result.CannotSnoozeYet)
                {
                    message = UITexts.DeliverySnoozeNotYet;
                }
                else
                {
                    message = string.Format(UITexts.DeliverySnooze, snoozeWeeks, result.NewDate.ToString("d MMMM yyyy"));
                }
            }

            DisplayMessage(message);

            return RedirectToAction("OrdersOverview");
        }
    }
}
