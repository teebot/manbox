using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ManBox.Model.ViewModels
{
    public class CheckoutShippingViewModel
    {
        public string SubscriptionToken { get; set; }

        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorFirstNameTooLong")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelFirstName")]
        public string ShippingFirstName { get; set; }

        
        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorLastNameTooLong")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelLastName")]
        public string ShippingLastName { get; set; }

        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorFirstNameTooLong")]
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorFirstNameRequired", ErrorMessage = null)]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelFirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorLastNameRequired")]
        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorLastNameTooLong")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelLastName")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorEmailFormat", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorEmailRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelEmail")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordRequired")]
        [StringLength(16, MinimumLength = 5, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordFormat")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelPassword")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorStreetRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelStreet")]
        public string Street { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorCityRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelCity")]
        public string City { get; set; }

        [StringLength(16, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPhoneTooLong")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelPhone")]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorProvinceRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelProvince")]
        public string Province { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPostalCodeRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelPostalCode")]
        public string PostalCode { get; set; }

        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelCountry")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; }

        public string NextDelivery { get; set; }

        [Required]
        public bool AgreeTerms { get; set; }

        public string PaymentMethod { get; set; }

        public int TotalInt { get; set; }

        public string PaymillPublicKey { get; set; }

        public List<SelectListItem> CCYearsList { get; set; }

        public List<SelectListItem> CCMonthsList { get; set; }

        public bool IsSubscriptionValid { get; set; }

        public string Token { get; set; }

        public string GuestName { get; set; }

        public string GiftMsg { get; set; }
    }

    public class IsTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value.GetType() != typeof(bool)) throw new InvalidOperationException("can only be used on boolean properties.");

            return (bool)value == true;
        }
    }
}
