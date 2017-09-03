using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ManBox.Model.ViewModels
{
    public class AddressViewModel
    {
        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorFirstNameTooLong")]
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorFirstNameRequired", ErrorMessage = null)]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelFirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorLastNameRequired")]
        [StringLength(64, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorLastNameTooLong")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelLastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorStreetRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelStreet")]
        public string Street { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorCityRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelCity")]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorProvinceRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelProvince")]
        public string Province { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPostalCodeRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelPostalCode")]
        public string PostalCode { get; set; }

        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelCountry")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; }
    }
}
