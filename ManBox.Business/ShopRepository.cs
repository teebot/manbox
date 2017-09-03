using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Security;
using ManBox.Model;
using ManBox.Model.ViewModels;
using ManBox.Common.Extensions;
using ManBox.Common.Resources;

namespace ManBox.Business
{
    public class ShopRepository : IShopRepository
    {
        private readonly List<string> sizeOrder = new List<string>() { "XXS", "XS", "S", "M", "L", "XL", "XXL", "XXXL" };

        private readonly double defaultNextDeliveryInDays = 2;
        private readonly int maxModelQuantity = 10;

        private ILogger logger;
        private IMailService mailService;

        public ShopRepository(ILogger loggerInject, IMailService mailServiceInject)
        {
            logger = loggerInject;
            mailService = mailServiceInject;
        }

        public Dictionary<string, int> GetAvailableLanguages()
        {
            var languages = new Dictionary<string, int>();
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                ent.Languages.ToList().ForEach(l => languages.Add(l.IsoCode, l.LanguageId));
            }
            return languages;
        }

        public ProductOverviewViewModel GetProductOverview()
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var user = IdHelper.CurrentUser;
                ent.Configuration.LazyLoadingEnabled = false;

                var cats = from c in ent.Categories
                           where c.IsVisible
                           orderby c.Position descending
                           select new CategoryViewModel
                           {
                               CategoryId = c.CategoryId,
                               Title = (from tt in ent.TranslationTexts where tt.TranslationId == c.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                               HasSizeChart = c.HasSizeChart,
                               Products = (from p in ent.Products
                                           let stockSum = p.Models.Sum(m => m.AmountInStock)
                                           where p.CategoryId == c.CategoryId
                                           && p.IsVisible
                                           && stockSum > 0
                                           orderby p.Position descending
                                           select new ProductViewModel()
                                           {
                                               Brand = p.Brand != null ? p.Brand.Name : null,
                                               ProductId = p.ProductId,
                                               Reference = p.Reference,
                                               Title = (from tt in ent.TranslationTexts where tt.TranslationId == p.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                               Description = (from tt in ent.TranslationTexts where tt.TranslationId == p.DescriptionTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                               DescriptionDetail = (from tt in ent.TranslationTexts where tt.TranslationId == p.DescriptionDetailTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                               Models = (from m in ent.Models
                                                         where m.ProductId == p.ProductId
                                                         && m.AmountInStock > 0
                                                         select new ModelViewModel()
                                                         {
                                                             ModelId = m.ModelId,
                                                             Name = m.Name,
                                                             ShopPrice = m.ShopPrice
                                                         }),
                                               Price = (from m in ent.Models where m.ProductId == p.ProductId orderby m.ShopPrice select m).FirstOrDefault().ShopPrice, // lowest price
                                               AdvisedPrice = (from m in ent.Models where m.ProductId == p.ProductId orderby m.ShopPrice select m).FirstOrDefault().AdvisedPrice // lowest price
                                           })
                           };

                var packs = from pa in ent.Packs
                            where !pa.IsAGift
                            orderby pa.Position descending, pa.ShopPrice
                            select new PackViewModel
                            {
                                PackId = pa.PackId,
                                Title = (from tt in ent.TranslationTexts where tt.TranslationId == pa.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                Description = (from tt in ent.TranslationTexts where tt.TranslationId == pa.DescrTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                Price = pa.ShopPrice,
                                AdvisedPrice = pa.AdvisedPrice,
                                Products = (from p in pa.ProductPacks
                                            select new ProductViewModel
                                            {
                                                Quantity = p.Quantity,
                                                Brand = p.Product.Brand != null ? p.Product.Brand.Name : null,
                                                ProductId = p.ProductId,
                                                Reference = p.Product.Reference,
                                                Title = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                Description = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.DescriptionTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                DescriptionDetail = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.DescriptionDetailTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                Models = (from m in ent.Models
                                                          where m.ProductId == p.ProductId
                                                          orderby m.Name
                                                          select new ModelViewModel()
                                                          {
                                                              ModelId = m.ModelId,
                                                              Name = m.Name,
                                                              ShopPrice = m.ShopPrice
                                                          }),
                                                AdvisedPrice = (from m in ent.Models where m.ProductId == p.ProductId orderby m.ShopPrice select m).FirstOrDefault().AdvisedPrice // lowest price
                                            })
                            };

                // make the data ready for view consumption
                var viewCats = cats.ToList();
                viewCats.ForEach(c =>
                {
                    c.TitleStd = Normalize(c.Title);
                    c.Products.ToList().ForEach(p =>
                    {
                        p.PriceCurrency = p.Price.ToCurrency();
                        p.AdvisedPriceCurrency = p.AdvisedPrice.ToCurrency();
                        p.Models = p.Models.ToList().OrderBy(m => m.Name).OrderBy(m => sizeOrder.IndexOf(m.Name)); // first order by size reference then default order
                    });
                });

                var viewPacks = packs.ToList();
                viewPacks.ForEach(pa =>
                {
                    pa.Products.ToList().ForEach(p =>
                    {
                        p.PriceCurrency = p.Price.ToCurrency();
                        p.AdvisedPriceCurrency = p.AdvisedPrice.ToCurrency();
                        p.Models = p.Models.ToList().OrderBy(m => m.Name).OrderBy(m => sizeOrder.IndexOf(m.Name));
                    });
                    pa.PriceCurrency = pa.Price.ToCurrency();
                    pa.AdvisedPriceCurrency = pa.AdvisedPrice.ToCurrency();
                    pa.FreeShipping = pa.Price > 29;
                    pa.SavingCurrency = (0 - (pa.AdvisedPrice - pa.Price)).ToCurrency();
                });

                return new ProductOverviewViewModel()
                {
                    ProductsCategories = viewCats,
                    Packs = viewPacks
                };
            }
        }

        public SubscriptionSelectionViewModel GetActiveSelection()
        {
            string token = IdHelper.CurrentUser.Token;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = null;

                if (!string.IsNullOrEmpty(token))
                {
                    sub = ent.Subscriptions.FirstOrDefault(s => s.Token == token && s.IsActive);
                }

                if (string.IsNullOrEmpty(token) || sub == null || (sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed && !IdHelper.CurrentUser.IsAuthenticated))
                {
                    // init defaults for empty cart
                    return new SubscriptionSelectionViewModel()
                    {
                        ItemsTotal = 0M.ToCurrency(),
                        SelectedProducts = new List<ProductSelectionViewModel>(),
                        ShippingFee = 0M.ToCurrency(),
                        SelectionTotal = 0M.ToCurrency(),
                        SubscriptionId = 0,
                        Token = string.Empty,
                        IsSubscribed = false,
                        DeliveryState = CodeValues.DeliveryState.New,
                        CartNotificationMessage = GetCartNotificationMessage(sub ?? null)
                    };
                }

                return GetDeliveryCart(ent, sub);
            }
        }

        /// <summary>
        /// controls which notification message must be displayed in the cart
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="delivery"></param>
        /// <returns></returns>
        private string GetCartNotificationMessage(Subscription sub, SubscriptionDelivery delivery = null)
        {
            string message = string.Empty;

            if (sub == null)
            {
                return UITexts.CartEmptyNotif;
            }

            if (sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed)
            {
                if (IdHelper.CurrentUser.IsAuthenticated && delivery != null)
                {
                    if (delivery.DeliveryStateCV == CodeValues.DeliveryState.Processing)
                    {
                        return UITexts.CartFrozenNotif;
                    }
                    else
                    {
                        return string.Format("{0} {1}", UITexts.CartModificationNotif, delivery.DeliveryDate.Value.ToString("d MMMM yyyy"));
                    }
                }
                else
                {
                    return string.Format(UITexts.WelcomeBackLogIn, sub.User.FirstName);
                }
            }
            else if (delivery == null || !delivery.SubscriptionDeliveryModels.Any())
            {
                return UITexts.CartEmptyNotif;
            }

            return string.Empty;
        }

        /// <summary>
        /// Removes from subscription
        /// Creates a NEW delivery if modifying a subscribed selection
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="token"></param>
        /// <returns>Updated subscription</returns>
        public SubscriptionSelectionViewModel RemoveFromSubscription(int modelId)
        {
            string token = IdHelper.CurrentUser.Token;
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = GetCurrentCart(token, ent);

                var delivery = Utilities.GetActiveDelivery(sub);
                var toRemove = delivery.SubscriptionDeliveryModels.FirstOrDefault(m => m.ModelId == modelId);
                if (toRemove != null)
                {
                    delivery.SubscriptionDeliveryModels.Remove(toRemove);
                    ent.SaveChanges();
                }

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == token && s.IsActive);

                if (sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed && !IdHelper.CurrentUser.IsAuthenticated)
                {
                    throw new Exception("cannot remove an item from an existing subscription when logged out");
                }

                return GetDeliveryCart(ent, sub);
            }
        }

        public SubscriptionSelectionViewModel RemovePackFromSubscription(int packId)
        {
            string token = IdHelper.CurrentUser.Token;
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = GetCurrentCart(token, ent);

                var delivery = Utilities.GetActiveDelivery(sub);
                var packItems = delivery.SubscriptionDeliveryModels.Where(m => m.PackId == packId).ToList();

                foreach (var i in packItems)
                {
                    delivery.SubscriptionDeliveryModels.Remove(i);
                }
                ent.SaveChanges();

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == token && s.IsActive);

                if (sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed && !IdHelper.CurrentUser.IsAuthenticated)
                {
                    throw new Exception("cannot remove an item from an existing subscription when logged out");
                }

                return GetDeliveryCart(ent, sub);
            }
        }

        public SubscriptionSelectionViewModel ApplyCouponToDelivery(string code, string token)
        {
            Subscription sub = null;
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var coupon = (from c in ent.Coupons
                              where c.Code == code.ToLower()
                              && c.Enabled
                              && c.NumberAvailable > 0
                              && c.ExpirationDate > DateTime.Now
                              select c).FirstOrDefault();

                if (coupon != null)
                {
                    if (coupon.Percentage == null ^ coupon.Amount == null)
                    {
                        sub = ent.Subscriptions.FirstOrDefault(s => s.Token == token);
                        var delivery = sub.SubscriptionDeliveries.Single(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);

                        delivery.CouponId = coupon.CouponId;
                        ent.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Trying to add an invalid coupon. Coupons must have an amount OR a percentage but never both");
                    }
                }

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == token && s.IsActive);

                return GetDeliveryCart(ent, sub);
            }
        }

        /// <summary>
        /// Add to subscription
        /// Creates a NEW delivery if modifying a subscribed selection
        /// Creates a new subscription if the user is anonymous (no token yet) and returns a new token in this case
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="quantity"></param>
        /// <param name="token"></param>
        /// <returns>Updated subscription</returns>
        public SubscriptionSelectionViewModel AddToSubscription(int modelId, int quantity)
        {
            string token = IdHelper.CurrentUser.Token;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                //ent.Configuration.LazyLoadingEnabled = false;
                Subscription sub = GetCurrentCart(token, ent);

                var activeDelivery = Utilities.GetActiveDelivery(sub);

                var selectionModel = activeDelivery.SubscriptionDeliveryModels.FirstOrDefault(m => m.ModelId == modelId);
                if (selectionModel != null)
                {
                    if ((selectionModel.Quantity + quantity) <= maxModelQuantity)
                    {
                        selectionModel.Quantity += quantity;
                    }
                }
                else
                {
                    activeDelivery.SubscriptionDeliveryModels.Add(new SubscriptionDeliveryModel() { ModelId = modelId, Quantity = quantity });
                }

                ent.SaveChanges();

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == sub.Token && s.IsActive);

                return GetDeliveryCart(ent, sub);
            }
        }

        public SubscriptionSelectionViewModel ChangeSelectionModel(int newModelId, int replacedModelId, int? packId)
        {
            string token = IdHelper.CurrentUser.Token;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                //ent.Configuration.LazyLoadingEnabled = false;
                Subscription sub = GetCurrentCart(token, ent);

                var activeDelivery = Utilities.GetActiveDelivery(sub);

                var oldModel = activeDelivery.SubscriptionDeliveryModels.FirstOrDefault(m => m.ModelId == replacedModelId && m.PackId == packId);
                var newModel = new SubscriptionDeliveryModel() { ModelId = newModelId, PackId = oldModel.PackId, Quantity = oldModel.Quantity };

                // merge quantities in case of duplicate
                var duplicate = activeDelivery.SubscriptionDeliveryModels.FirstOrDefault(m => m.ModelId == newModel.ModelId && m.PackId == newModel.PackId);
                if (duplicate != null)
                {
                    activeDelivery.SubscriptionDeliveryModels.Remove(duplicate);
                    ent.SaveChanges();

                    newModel.Quantity += duplicate.Quantity;
                }

                activeDelivery.SubscriptionDeliveryModels.Remove(oldModel);
                ent.SaveChanges();

                activeDelivery.SubscriptionDeliveryModels.Add(newModel);
                ent.SaveChanges();

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == sub.Token && s.IsActive);

                return GetDeliveryCart(ent, sub);
            }
        }


        /// <summary>
        /// retrieve customer by token
        /// if token null empty or no subscription ==> create a new subscription 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        private Subscription GetCurrentCart(string token, ManBoxEntities ent)
        {
            Subscription sub = null;

            if (token != null)
            {
                sub = ent.Subscriptions
                    .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                    .FirstOrDefault(s => s.Token == token && s.IsActive);

                // subscribed user requesting a modification delivery 
                if (sub != null && sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed)
                {
                    if (!IdHelper.CurrentUser.IsAuthenticated)
                    {
                        // reset cart for existing subscribed token but logged out 
                        sub = InitializeAnonymousSubscription(ent);

                        //throw new Exception("cannot modify a processing delivery when logged out");
                    }

                    if (sub.SubscriptionDeliveries.Any(d => d.DeliveryStateCV == CodeValues.DeliveryState.Processing))
                    {
                        throw new Exception("cannot modify a processing delivery");
                    }
                    // create a new delivery if not any yet
                    else if (!sub.SubscriptionDeliveries.Any(d => d.DeliveryStateCV == CodeValues.DeliveryState.New))
                    {
                        var pendingDelivery = sub.SubscriptionDeliveries.FirstOrDefault(s => s.DeliveryStateCV == CodeValues.DeliveryState.Pending);
                        sub.SubscriptionDeliveries.Add(GetCopyNewDelivery(pendingDelivery));
                        ent.SaveChanges();
                    }
                }
            }

            if (sub == null) // create anonymous subscription
            {
                sub = InitializeAnonymousSubscription(ent);
            }
            return sub;
        }

        public SubscriptionSelectionViewModel AddPackToSubscription(int packId)
        {
            string token = IdHelper.CurrentUser.Token;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub = GetCurrentCart(token, ent);

                var activeDelivery = Utilities.GetActiveDelivery(sub);

                // you cannot have more than one pack
                if (activeDelivery.SubscriptionDeliveryModels.Any(m => m.PackId != null))
                {
                    var packItems = activeDelivery.SubscriptionDeliveryModels.Where(m => m.PackId != null).ToList();
                    foreach (var pi in packItems)
                    {
                        ent.SubscriptionDeliveryModels.Remove(pi);
                    }
                }

                var pack = ent.Packs.Single(p => p.PackId == packId);

                foreach (var p in pack.ProductPacks) {
                    var duplicate = activeDelivery.SubscriptionDeliveryModels.FirstOrDefault(m => m.Model.ProductId == p.ProductId);
                    if (duplicate != null)
                    {
                        activeDelivery.SubscriptionDeliveryModels.Remove(duplicate);
                        ent.SaveChanges();
                    }

                    activeDelivery.SubscriptionDeliveryModels.Add(new SubscriptionDeliveryModel() { Model = p.Product.Models.First(), Quantity = p.Quantity, PackId = packId });
                }

                ent.SaveChanges();

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == sub.Token && s.IsActive);

                return GetDeliveryCart(ent, sub);
            }
        }


        public SubscriptionSelectionViewModel AddGift(int giftPackId)
        {
            string token = IdHelper.CurrentUser.Token;
            
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                Subscription sub;
                if (token == null || !IdHelper.CurrentUser.IsAuthenticated)
                {
                    sub = InitializeAnonymousSubscription(ent);
                }
                else {
                    sub = InitializeGiftOrderForSubscribedCustomer(ent, token);   
                }
                
                var activeDelivery = Utilities.GetActiveDelivery(sub);

                // you cannot have other subscription items 
                if(activeDelivery.SubscriptionDeliveryModels.Any(m => m.PackId == null))
                {
                    return new SubscriptionSelectionViewModel() { CartNotificationMessage = "Vous ne pouvez combiner un paquet cadeau avec un abonnement." };
                }

                // you cannot have more than one gift pack
                if (activeDelivery.SubscriptionDeliveryModels.Any(m => m.PackId != null))
                {
                    var packItems = activeDelivery.SubscriptionDeliveryModels.Where(m => m.PackId != null).ToList();
                    foreach (var pi in packItems)
                    {
                        ent.SubscriptionDeliveryModels.Remove(pi);
                    }
                }

                var pack = ent.Packs.Single(p => p.PackId == giftPackId);

                if (!pack.IsAGift)
                {
                    throw new Exception("Expected a gift pack");
                }

                foreach (var p in pack.ProductPacks)
                {
                    activeDelivery.SubscriptionDeliveryModels.Add(new SubscriptionDeliveryModel() { Model = p.Product.Models.First(), Quantity = p.Quantity, PackId = pack.PackId });
                }

                ent.SaveChanges();

                // requery with updated data
                sub = ent.Subscriptions
                       .Include(s => s.SubscriptionDeliveries.Select(d => d.SubscriptionDeliveryModels.Select(m => m.Model.Product)))
                       .FirstOrDefault(s => s.Token == sub.Token && s.IsActive);

                return GetDeliveryCart(ent, sub);
            }
        }

        public ProductVisualsViewModel GetProductVisuals(string productReference)
        {
            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var product = ent.Products.FirstOrDefault(p => p.Reference == productReference);
                if (product != null)
                {
                    return new ProductVisualsViewModel
                    {
                        ProductReference = productReference,
                        VineUrl = product.VineUrl,
                        NumberOfPictures = product.NumberOfPictures
                    };
                }

                return null;
            }
        }

        public ReScheduleDeliveryViewModel ReScheduleDelivery(int deliveryId, bool rushNow, int? snoozeWeeks)
        {
            DateTime newDateTime;

            bool shippableTomorrow = true;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var pendingDelivery = ent.SubscriptionDeliveries.Single(d => d.SubscriptionDeliveryId == deliveryId
                    && d.DeliveryStateCV == CodeValues.DeliveryState.Pending
                    && d.Subscription.User.Email == IdHelper.CurrentUser.Email);

                if (rushNow)
                {
                    newDateTime = DateTime.Now.AddDays(1);

                    // check stocks if it's to be shipped within 7 days
                    shippableTomorrow = pendingDelivery.SubscriptionDeliveryModels.ToList().All(m => m.Model.AmountInStock > 1);
                    /// if asked to rush and stock is insufficient, set a close date
                    if (!shippableTomorrow)
                    {
                        newDateTime = DateTime.Now.AddDays(7);
                    }
                }
                else
                {
                    if (pendingDelivery.DeliveryDate > DateTime.Now.AddMonths(1))
                    {
                        return new ReScheduleDeliveryViewModel() { CannotSnoozeYet = true };
                    }
                    newDateTime = pendingDelivery.DeliveryDate.Value.AddDays(7 * snoozeWeeks.Value);
                }

                pendingDelivery.DeliveryDate = newDateTime;
                ent.SaveChanges();

                return new ReScheduleDeliveryViewModel()
                {
                    ShippableTomorrow = shippableTomorrow,
                    NewDate = newDateTime
                };
            }
        }

        public GiftPacksViewModel GetGiftPacks()
        {
            var model = new GiftPacksViewModel();
            var user = IdHelper.CurrentUser;

            using (ManBoxEntities ent = new ManBoxEntities())
            {
                var packs = from pa in ent.Packs
                            where pa.IsAGift
                            orderby pa.Position descending, pa.ShopPrice
                            select new PackViewModel
                            {
                                PackId = pa.PackId,
                                Title = (from tt in ent.TranslationTexts where tt.TranslationId == pa.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                Description = (from tt in ent.TranslationTexts where tt.TranslationId == pa.DescrTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                Price = pa.ShopPrice,
                                GiftVoucherValue = pa.GiftVoucherValue,
                                AdvisedPrice = pa.AdvisedPrice,
                                Products = (from p in pa.ProductPacks
                                            select new ProductViewModel
                                            {
                                                Quantity = p.Quantity,
                                                Brand = p.Product.Brand != null ? p.Product.Brand.Name : null,
                                                ProductId = p.ProductId,
                                                Reference = p.Product.Reference,
                                                Title = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.TitleTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                Description = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.DescriptionTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                DescriptionDetail = (from tt in ent.TranslationTexts where tt.TranslationId == p.Product.DescriptionDetailTrId && tt.LanguageId == user.LanguageId select tt.Text).FirstOrDefault(),
                                                Models = (from m in ent.Models
                                                          where m.ProductId == p.ProductId
                                                          orderby m.Name
                                                          select new ModelViewModel()
                                                          {
                                                              ModelId = m.ModelId,
                                                              Name = m.Name,
                                                              ShopPrice = m.ShopPrice
                                                          }),
                                                AdvisedPrice = (from m in ent.Models where m.ProductId == p.ProductId orderby m.ShopPrice select m).FirstOrDefault().AdvisedPrice // lowest price
                                            })
                            };

                model.GiftPacks = packs.ToList();
            }

            return model;
        }

        #region Utilities

        /// <summary>
        /// Get a subscription delivery summary with products and updated totals
        /// </summary>
        private SubscriptionSelectionViewModel GetDeliveryCart(ManBoxEntities ent, Subscription sub)
        {
            var activeDelivery = Utilities.GetActiveDelivery(sub);

            decimal itemsTotal = Utilities.GetDeliveryTotal(activeDelivery);
            decimal shippingFee = Utilities.CalculateShippingFee(itemsTotal);
            decimal couponAmount = Utilities.CalculateCouponAmount(itemsTotal, activeDelivery);
            decimal selectionTotal = itemsTotal + shippingFee - couponAmount;

            return new SubscriptionSelectionViewModel()
            {
                Token = sub.Token,
                SelectedProducts = GetSubscriptionDeliveryProducts(activeDelivery, ent), 
                SubscriptionId = sub.SubscriptionId,
                ItemsTotal = itemsTotal.ToCurrency(),
                ShippingFee = itemsTotal > 0 ? shippingFee.ToCurrency() : 0M.ToCurrency(),
                CouponAmount = couponAmount > 0 ? (0 - couponAmount).ToCurrency() : string.Empty,
                CouponLabel = Utilities.GetCouponLabel(activeDelivery.Coupon),
                SelectionTotal = itemsTotal > 0 ? selectionTotal.ToCurrency() : 0M.ToCurrency(),
                NextDeliveryDate = activeDelivery.DeliveryDate.Value.ToString("d MMMM yyyy"),
                IsSubscribed = sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed,
                DeliveryState = activeDelivery.DeliveryStateCV,
                CartNotificationMessage = GetCartNotificationMessage(sub, activeDelivery),
                Pack = GetSubscriptionDeliveryPack(activeDelivery, ent)
            };
        }

        private PackSelectionViewModel GetSubscriptionDeliveryPack(SubscriptionDelivery delivery, ManBoxEntities ent)
        {
            int langId = IdHelper.CurrentUser.LanguageId;
            List<ProductSelectionViewModel> selectedProducts = new List<ProductSelectionViewModel>();

            // note: there can only be one pack
            var packItems = delivery.SubscriptionDeliveryModels.Where(m => m.PackId != null).OrderByDescending(m => m.ModelId);
            if (!packItems.Any())
            {
                return new PackSelectionViewModel();
            }

            var pack = packItems.First().Pack;
            var packViewModel = new PackSelectionViewModel()
            {
                PackId = pack.PackId,
                Price = pack.ShopPrice.ToCurrency(),
                SelectedProducts = new List<ProductSelectionViewModel>(),
                Title = (from tt in ent.TranslationTexts where tt.LanguageId == langId && tt.TranslationId == pack.TitleTrId select tt.Text).FirstOrDefault() + " Pack"
            };

            packItems.ToList().ForEach(i => packViewModel.SelectedProducts.Add(
                new ProductSelectionViewModel()
                {
                    ModelId = i.ModelId,
                    ModelName = i.Model.Name,
                    ProductId = i.Model.ProductId,
                    ProductReference = i.Model.Product.Reference,
                    ProductTitle = (from tt in ent.TranslationTexts where tt.LanguageId == langId && tt.TranslationId == i.Model.Product.TitleTrId select tt.Text).FirstOrDefault(),
                    Quantity = i.Quantity,
                    SubTotalPrice = null, // price is at pack level
                    UnitPrice = null,
                    Models = (from m in ent.Models
                              where m.ProductId == i.Model.ProductId
                              select new ModelViewModel()
                              {
                                  ModelId = m.ModelId,
                                  Name = m.Name,
                                  ShopPrice = m.ShopPrice,
                                  ProductId = m.ProductId,
                                  PackId = pack.PackId
                              }).ToList()
                })
            );

            // order model sizes
            packViewModel.SelectedProducts.ForEach(p => p.Models = p.Models.OrderBy(m => m.Name).OrderBy(m => sizeOrder.IndexOf(m.Name)));
            return packViewModel;
        }

        private List<ProductSelectionViewModel> GetSubscriptionDeliveryProducts(SubscriptionDelivery delivery, ManBoxEntities ent)
        {
            int langId = IdHelper.CurrentUser.LanguageId;
            List<ProductSelectionViewModel> selectedProducts = new List<ProductSelectionViewModel>();
            delivery.SubscriptionDeliveryModels.Where(m => m.PackId == null).ToList().ForEach(m => selectedProducts.Add(new ProductSelectionViewModel()
            {
                ModelId = m.ModelId,
                ModelName = m.Model.Name,
                ProductId = m.Model.ProductId,
                ProductTitle = (from tt in ent.TranslationTexts where tt.LanguageId == langId && tt.TranslationId == m.Model.Product.TitleTrId select tt.Text).FirstOrDefault(),
                ProductReference = m.Model.Product.Reference,
                Quantity = m.Quantity,
                UnitPrice = m.Model.ShopPrice.ToCurrency(),
                SubTotalPrice = (m.Quantity * m.Model.ShopPrice).ToCurrency(),
                Models = (from mo in ent.Models
                          where mo.ProductId == m.Model.ProductId
                          select new ModelViewModel()
                          {
                              ModelId = mo.ModelId,
                              Name = mo.Name,
                              ShopPrice = mo.ShopPrice,
                              ProductId = mo.ProductId
                          }).ToList()
            }));

            // order model sizes
            selectedProducts.ForEach(p => p.Models = p.Models.OrderBy(m => m.Name).OrderBy(m => sizeOrder.IndexOf(m.Name)));
            return selectedProducts;
        }

        private Subscription InitializeAnonymousSubscription(ManBoxEntities ent)
        {
            var newSubscription = new Subscription()
            {
                Token = Guid.NewGuid().ToString(),
                CreatedDatetime = DateTime.Now,
                IsActive = true,
                IsPaused = false,
                SubscriptionStateCV = CodeValues.SubscriptionState.InCart
            };

            var newSubscriptionDelivery = new SubscriptionDelivery()
            {
                DeliveryDate = DateTime.Now.AddDays(defaultNextDeliveryInDays),
                DeliveryStateCV = CodeValues.DeliveryState.New,
                QueuedDatetime = DateTime.Now,
                DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.None
            };

            newSubscription.SubscriptionDeliveries.Add(newSubscriptionDelivery);
            ent.Subscriptions.Add(newSubscription);
            ent.SaveChanges();

            return newSubscription;
        }

        private Subscription InitializeGiftOrderForSubscribedCustomer(ManBoxEntities ent, string existingSubToken)
        {
            var existingSub = ent.Subscriptions.Single(s => s.Token == existingSubToken);

            var newGiftOrder = new Subscription()
            {
                Token = Guid.NewGuid().ToString(),
                CreatedDatetime = DateTime.Now,
                IsActive = true,
                IsPaused = false,
                SubscriptionStateCV = CodeValues.SubscriptionState.InCart,
                Gift = new Gift(),
                User = existingSub.User
            };

            var newSubscriptionDelivery = new SubscriptionDelivery()
            {
                DeliveryDate = DateTime.Now.AddDays(defaultNextDeliveryInDays),
                DeliveryStateCV = CodeValues.DeliveryState.New,
                QueuedDatetime = DateTime.Now,
                DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.None
            };

            newGiftOrder.SubscriptionDeliveries.Add(newSubscriptionDelivery);
            ent.Subscriptions.Add(newGiftOrder);
            ent.SaveChanges();

            return newGiftOrder;
        }

        private SubscriptionDelivery GetCopyNewDelivery(SubscriptionDelivery delivery)
        {
            var newDelivery = new SubscriptionDelivery()
            {
                DeliveryStateCV = CodeValues.DeliveryState.New,
                DeliveryDate = delivery.DeliveryDate,
                DeliveryPaymentStatusCV = CodeValues.DeliveryPaymentStatus.None,
                QueuedDatetime = delivery.QueuedDatetime,
                ShippingFee = 0
            };

            delivery.SubscriptionDeliveryModels.ToList().ForEach(m =>
                newDelivery.SubscriptionDeliveryModels.Add(new SubscriptionDeliveryModel()
                {
                    ModelId = m.ModelId,
                    Quantity = m.Quantity
                })
            );

            return newDelivery;
        }

        /// <summary>
        /// removes accents and return lower case
        /// </summary>
        /// <param name="stIn"></param>
        /// <returns></returns>
        private static string Normalize(string stIn)
        {
            if (string.IsNullOrEmpty(stIn))
            {
                return string.Empty;
            }


            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC)).ToLower();
        }

        #endregion
    }
}
