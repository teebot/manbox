using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Resources;
using ManBox.Common.Security;
using ManBox.Model;
using ManBox.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManBox.Business
{
    public class UserRepository : IUserRepository
    {
        private ILogger logger;
        private IMailService mailService;

        public UserRepository(ILogger loggerInject, IMailService mailServiceInject)
        {
            logger = loggerInject;
            mailService = mailServiceInject;
        }

        public void SubscribeNewsletterEmail(string email)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var existing = ent.Newsletters.Any(n => n.Email == email);

                if (!existing)
                {
                    ent.Newsletters.Add(new Newsletter()
                    {
                        Email = email,
                        Subscribed = true,
                        LanguageId = IdHelper.CurrentUser.LanguageId,
                        CountryId = IdHelper.CurrentUser.CountryId
                    });

                    var couponCode = "man" + Guid.NewGuid().ToString().Substring(0, 6);

                    ent.Coupons.Add(new Coupon()
                    {
                        Amount = 3,
                        Code = couponCode,
                        Enabled = true,
                        ExpirationDate = DateTime.Now.AddMonths(12),
                        NumberAvailable = 1
                    });

                    ent.SaveChanges();

                    SendNewsletterWelcomeMail(email, couponCode);
                }
            }
        }

        /// <summary>
        /// unsubscribes from newsletter any registered user or mailing list entry
        /// </summary>
        /// <param name="email"></param>
        public void Unsubscribe(string email)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var newsletterUser = ent.Newsletters.FirstOrDefault(u => u.Email == email);

                if (newsletterUser != null)
                {
                    newsletterUser.Subscribed = false;
                }

                var registeredUser = ent.Users.FirstOrDefault(u => u.Email == email);
                if (registeredUser != null)
                {
                    registeredUser.IsOptin = false;
                }

                ent.SaveChanges();
            }
        }

        public UserViewModel GetUserByEmail(string email)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = (from u in ent.Users
                            where u.Email == email
                            select u).FirstOrDefault();

                if (user == null)
                    return null;


                return new UserViewModel()
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = user.Subscriptions.FirstOrDefault().Token,
                        LanguageId = user.LanguageId,
                        LanguageIsoCode = user.Language.IsoCode
                    };
            }
        }

        public UserViewModel Register(UserViewModel user, string signinType, string subscrToken)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = null;

                if (string.IsNullOrEmpty(user.Password) && signinType != CodeValues.SignInType.Facebook)
                {
                    throw new Exception("Password cannot be empty");
                }

                var existingUser = ent.Users.FirstOrDefault(u => u.Email == user.Email);
                
                // if it's an existing facebook user, retrieve his subscription or create a new one
                if (existingUser != null)
                {
                    if (signinType == CodeValues.SignInType.Facebook)
                    {
                        sub = existingUser.Subscriptions.FirstOrDefault();

                        if (sub == null)
                        {
                            sub = new Subscription()
                                {
                                    Token = Guid.NewGuid().ToString(),
                                    CreatedDatetime = DateTime.Now,
                                    IsActive = true,
                                    IsPaused = false,
                                    SubscriptionStateCV = CodeValues.SubscriptionState.InCart,
                                    UserId = existingUser.UserId
                                };
                            ent.Subscriptions.Add(sub);
                        }

                        return new UserViewModel()
                        {
                            Email = existingUser.Email,
                            Token = sub.Token,
                            FirstName = existingUser.FirstName,
                            LastName = existingUser.LastName,
                            UserId = existingUser.UserId
                        };
                    }

                    return null;
                }

                var newUser = new User()
                {
                    Email = user.Email,
                    Password = user.Password,
                    CreatedDatetime = DateTime.Now,
                    CountryId = IdHelper.CurrentUser.CountryId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsOptin = true,
                    SignInTypeCV = signinType,
                    LanguageId = IdHelper.CurrentUser.LanguageId
                };
                ent.Users.Add(newUser);

                sub = ent.Subscriptions.FirstOrDefault(u => u.Token == subscrToken);

                // assign current anonymous subscription to the new user
                if (sub != null)
                {
                    sub.User = newUser;
                }

                ent.SaveChanges();

                return new UserViewModel()
                {
                    Email = newUser.Email,
                    Token = subscrToken,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    UserId = newUser.UserId
                };
            }
        }

        public UserViewModel Login(string email, string password, string subscrToken)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = ent.Users.FirstOrDefault(u => u.Email == email.ToLower() && u.Password == password);

                if (user != null)
                {
                    // first try to load an active sub for the logged in user
                    var activeSub = user.Subscriptions.FirstOrDefault();

                    // if he does not have any sub yet
                    if (activeSub == null)
                    {
                        // retrieve anonymous sub or previously used
                        if (!string.IsNullOrEmpty(subscrToken))
                        {
                            activeSub = ent.Subscriptions.FirstOrDefault(s => s.Token == subscrToken && (s.UserId == user.UserId || s.UserId == null));
                        }
                        // if no token create a subscription for this user
                        else
                        {
                            activeSub = ent.Subscriptions.Add(new Subscription()
                            {
                                Token = Guid.NewGuid().ToString(),
                                CreatedDatetime = DateTime.Now,
                                IsActive = true,
                                IsPaused = false,
                                SubscriptionStateCV = CodeValues.SubscriptionState.InCart
                            });
                        }

                        // at this point if we still don't have a sub, something failed really bad
                        if (activeSub == null)
                        {
                            throw new Exception("Login failed, no active subscription could be found or created");
                        }

                        // link the user to the anonymous sub if any
                        activeSub.User = user;
                        ent.SaveChanges();
                    }

                    return new UserViewModel()
                    {
                        Email = user.Email,
                        Token = activeSub.Token,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserId = user.UserId
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public UserViewModel TokenLogin(string encryptedToken)
        {
            var subscrToken = TokenEncrypt.DecryptExpiringToken(encryptedToken);
            if (string.IsNullOrEmpty(subscrToken))
            {
                return null;
            }

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.Single(s => s.Token == subscrToken);
                if (sub == null)
                {
                    throw new Exception("Could not find subscription for token login");
                }

                return new UserViewModel()
                {
                    Email = sub.User.Email,
                    Token = subscrToken,
                    FirstName = sub.User.Email,
                    LastName = sub.User.Email,
                    UserId = sub.User.UserId,
                };
            }
        }

        public void SendSupportMail(ContactFormViewModel contactForm)
        {
            var manboxRecipient = new MailRecipient("support@manbox.be", "Support ManBox");

            var content = string.Format(
                            @"From: {0}
                            <br />
                            Message: {1}",
                            contactForm.Email, contactForm.Message);

            mailService.SendMail(manboxRecipient, manboxRecipient, contactForm.Subject, content);
        }

        public void UpdatePassword(string newPassword)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = ent.Users.Single(u => u.Email == IdHelper.CurrentUser.Email);
                user.Password = newPassword;
                ent.SaveChanges();
            }
        }

        public void UpdateAddress(AddressViewModel newAddress)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var sub = ent.Subscriptions.FirstOrDefault(s => s.User.Email == IdHelper.CurrentUser.Email);

                sub.Address.City = newAddress.City;
                sub.Address.CountryId = newAddress.CountryId;
                sub.Address.FirstName = newAddress.FirstName;
                sub.Address.LastName = newAddress.LastName;
                sub.Address.PostalCode = newAddress.PostalCode;
                sub.Address.Province = newAddress.Province;
                sub.Address.Street = newAddress.Street;

                ent.SaveChanges();
            }
        }

        public AddressViewModel GetCurrentUserAddress()
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var address = ent.Subscriptions.FirstOrDefault(s => s.User.Email == IdHelper.CurrentUser.Email).Address;

                if (address != null)
                {
                    return new AddressViewModel()
                    {
                        City = address.City,
                        CountryId = address.CountryId,
                        FirstName = address.FirstName,
                        LastName = address.LastName,
                        PostalCode = address.PostalCode,
                        Province = address.Province,
                        Street = address.Street
                    };
                }

                return null;
            }
        }

        public OrdersOverviewViewModel GetOrdersOverview()
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var deliveries = (from d in ent.SubscriptionDeliveries
                                  where d.Subscription.Token == IdHelper.CurrentUser.Token
                                  && d.Subscription.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed
                                  && (d.DeliveryStateCV == CodeValues.DeliveryState.Sent || d.DeliveryStateCV == CodeValues.DeliveryState.Processing || d.DeliveryStateCV == CodeValues.DeliveryState.Pending)
                                  select new
                                  {
                                      Date = d.DeliveryDate,
                                      DeliveryId = d.SubscriptionDeliveryId,
                                      DeliveryState = d.DeliveryStateCV,
                                      ShippingFee = d.ShippingFee,
                                      CouponAmount = d.CouponAmount,
                                      Products = (from dm in ent.SubscriptionDeliveryModels
                                                  where dm.SubscriptionDeliveryId == d.SubscriptionDeliveryId
                                                  select new
                                                  {
                                                      Price = dm.Model.ShopPrice,
                                                      Quantity = dm.Quantity,
                                                      ProductName = (from tt in ent.TranslationTexts
                                                                     where tt.LanguageId == IdHelper.CurrentUser.LanguageId
                                                                      && tt.TranslationId == dm.Model.Product.TitleTrId
                                                                     select tt.Text).FirstOrDefault(),
                                                      ProductReference = dm.Model.Product.Reference,
                                                      ModelName = dm.Model.Name,
                                                      TotalPrice = dm.Quantity * dm.Model.ShopPrice
                                                  })
                                  }).ToList();

                // mapping
                List<OrderDeliveryViewModel> orders = new List<OrderDeliveryViewModel>();
                deliveries.ForEach(d => orders.Add(new OrderDeliveryViewModel()
                    {
                        Amount = (from p in d.Products.ToList() select p.TotalPrice).Sum() + d.ShippingFee - d.CouponAmount,
                        CouponAmount = d.CouponAmount,
                        Date = d.Date,
                        DeliveryId = d.DeliveryId,
                        DeliveryState = d.DeliveryState,
                        ShippingFee = d.ShippingFee,
                        Products = d.Products.ToList().Select(p => new DeliveryProductViewModel()
                        {
                            ModelName = p.ModelName,
                            Price = p.Price,
                            ProductName = p.ProductName,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            ProductReference = p.ProductReference
                        }).ToList()
                    })
                );

                return new OrdersOverviewViewModel() { Orders = orders };
            }
        }

        /// <summary>
        /// sends an expiring login link 
        /// </summary>
        /// <param name="email"></param>
        /// <returns>success</returns>
        public bool SendPasswordReset(string email)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = ent.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    this.logger.Log(LogType.Warn, "Password reset attempt without valid email. Too many attempts is fishy.");
                    return false;
                }

                var encryptedToken = HttpUtility.UrlEncode(TokenEncrypt.EncryptTokenAsExpiring(user.Subscriptions.First().Token, DateTime.Now.AddDays(1)));

                var fromRecipient = new MailRecipient("support@manbox.be", "Support ManBox");
                var toRecipient = new MailRecipient(email, user.FirstName);

                var domain = Utilities.GetCountryDomain(user.Country.IsoCode);
                var linkToken = string.Format("http://{0}/{1}/Account/TokenLogin?token={2}", domain, user.Language.IsoCode, encryptedToken);
                
                mailService.SendMail<PasswordResetMail>(toRecipient,
                    fromRecipient,
                    new PasswordResetMail()
                    {
                        RootUrl = "http://" + domain,
                        LanguageIso = user.Language.IsoCode,
                        Date = DateTime.Now,
                        Name = user.FirstName,
                        Subject = "Password Reset",
                        LinkToken = linkToken
                    });

                return true;
            }
        }

        private void SendNewsletterWelcomeMail(string email, string couponCode)
        {
            var fromRecipient = new MailRecipient("thibaut@manbox.be", "Thibaut");
            var toRecipient = new MailRecipient(email, string.Empty);
            var domain = Utilities.GetCountryDomain(IdHelper.CurrentUser.CountryIsoCode);

            mailService.SendMail<NewsletterWelcomeMail>(toRecipient,
                fromRecipient,
                new NewsletterWelcomeMail()
                {
                    RootUrl = "http://" + domain,
                    LanguageIso = IdHelper.CurrentUser.LanguageIsoCode,
                    Date = DateTime.Now,
                    Subject = UITexts.WelcomeMailCouponSubject,
                    CouponCode = couponCode
                });
        }
    }
}
