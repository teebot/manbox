using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Mail.Models
{
    public class UpcomingBoxNotificationMail : MailModelBase
    {
        public List<MailProduct> Products { get; set; }

        public decimal Total { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingFee { get; set; }

        public MailAddress Address { get; set; }

        public decimal CouponAmount { get; set; }

        public string CouponLabel { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string LinkToken { get; set; }

        public string CountryIso { get; set; }
    }

    public class PasswordResetMail : MailModelBase
    {
        public string LinkToken { get; set; }
    }

    public class SubscriptionConfirmationMail : MailModelBase
    {
        public List<MailProduct> Products { get; set; }

        public decimal Total { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingFee { get; set; }

        public MailAddress Address { get; set; }

        public decimal CouponAmount { get; set; }

        public string CouponLabel { get; set; }
    }

    public class GiftConfirmationMail : MailModelBase
    {
        public List<MailProduct> Products { get; set; }

        public decimal Total { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingFee { get; set; }

        public MailAddress Address { get; set; }

        public decimal CouponAmount { get; set; }

        public string CouponLabel { get; set; }

        public string GiftMessage { get; set; }

        public string GuestName { get; set; }
    }

    public class ShipmentConfirmationMail : MailModelBase
    {
        public List<MailProduct> Products { get; set; }

        public decimal Total { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingFee { get; set; }

        public MailAddress Address { get; set; }

        public decimal CouponAmount { get; set; }

        public string CouponLabel { get; set; }
    }

    public class ModificationConfirmationMail : MailModelBase
    {
        public List<MailProduct> Products { get; set; }

        public decimal Total { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingFee { get; set; }

        public MailAddress Address { get; set; }
    }

    public class MailProduct {

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; }

        public decimal TotalPrice { get; set; }

        public string ModelName { get; set; }
    }

    public class MailAddress {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }

        public string City { get; set; }
    }
}
