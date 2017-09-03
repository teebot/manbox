using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManBox.Common;
using ManBox.Model;

namespace ManBox.Business
{
    public class Utilities
    {
        private static readonly decimal defaultShippingFee = 3;
        private static readonly decimal freeShippingTreshold = 29;

        public static decimal CalculateShippingFee(decimal total)
        {
            if (total < freeShippingTreshold)
            {
                return defaultShippingFee;
            }

            return 0;
        }

        public static decimal GetDeliveryTotal(SubscriptionDelivery delivery)
        {
            decimal total = 0;
            if (delivery != null)
            {
                // count individual items
                delivery.SubscriptionDeliveryModels.Where(m => m.PackId == null).ToList().ForEach(m => total += (m.Quantity * m.Model.ShopPrice));

                // count packs
                foreach (var packedSub in delivery.SubscriptionDeliveryModels.Where(m => m.PackId != null).GroupBy(m => m.PackId))
                {
                    total += packedSub.Where(p => p.Pack != null).First().Pack.ShopPrice;
                }
            }
            return total;
        }

        public static SubscriptionDelivery GetActiveDelivery(Subscription sub)
        {
            if (sub.SubscriptionStateCV == CodeValues.SubscriptionState.Subscribed)
            {
                // if a subscribed user has a New 
                var newDelivery = sub.SubscriptionDeliveries
                    .OrderByDescending(d => d.SubscriptionDeliveryId)
                    .FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);

                if (newDelivery != null)
                {
                    return newDelivery;
                }
                else
                {
                    return sub.SubscriptionDeliveries
                        .OrderByDescending(d => d.SubscriptionDeliveryId)
                        .FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.Pending || d.DeliveryStateCV == CodeValues.DeliveryState.Processing);
                }
            }
            else
            {
                // unsubscribed user
                return sub.SubscriptionDeliveries
                    .OrderByDescending(d => d.SubscriptionDeliveryId)
                    .FirstOrDefault(d => d.DeliveryStateCV == CodeValues.DeliveryState.New);
            }
        }

        public static string GetCountryDomain(string countryIso)
        {
            switch (countryIso.ToLower())
            {
                case "fr":
                    return "manbox.fr";
                case "nl":
                    return "manbox.nl";
                default:
                case "be":
                case "lu":
                    return "manbox.be";
            }
        }

        public static decimal CalculateCouponAmount(decimal itemsTotal, SubscriptionDelivery delivery)
        {
            if (delivery == null || delivery.Coupon == null) 
            {
                return 0;
            }

            if (delivery.Coupon.Amount == null ^ delivery.Coupon.Percentage == null)
            {
                if (delivery.Coupon.Amount.HasValue) 
                {
                    return delivery.Coupon.Amount.Value;
                }
                else if (delivery.Coupon.Percentage.HasValue)
                {
                    return (delivery.Coupon.Percentage.Value * itemsTotal) / 100;
                }
            }
            
            throw new Exception("Trying to calculate an invalid coupon.");
        }

        public static string GetCouponLabel(Coupon coupon)
        {
            var couponText = string.Empty;
            if (coupon != null)
            {
                couponText = coupon.Code;
                if (coupon.Percentage != null && coupon.Percentage > 0)
                {
                    couponText += string.Format(" - {0} %", coupon.Percentage);
                }
            }

            return couponText;
        }
    }
}
