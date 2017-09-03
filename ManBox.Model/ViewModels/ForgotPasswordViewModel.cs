using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorEmailFormat", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorEmailRequired")]
        [Display(ResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), Name = "LabelEmail")]
        public string Email { get; set; }
    }
}
