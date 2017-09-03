using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class PasswordUpdateViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordRequired")]
        [StringLength(16, MinimumLength = 5, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordFormat")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordRequired")]
        [StringLength(16, MinimumLength = 5, ErrorMessageResourceType = typeof(ManBox.Common.Resources.CheckoutMetadata), ErrorMessageResourceName = "ErrorPasswordFormat")]
        [Compare("NewPassword")]
        public string RepeatNewPassword { get; set; }
    }
}
