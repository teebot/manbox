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
    public class ContactFormViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.AccountMetadata), ErrorMessageResourceName = "ErrorEmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ManBox.Common.Resources.AccountMetadata), ErrorMessageResourceName = "ErrorEmailFormat", ErrorMessage = null)]
        [Display(ResourceType = typeof(ManBox.Common.Resources.AccountMetadata), Name = "LabelEmail")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.AccountMetadata), ErrorMessageResourceName = "ErrorFirstNameRequired", ErrorMessage = null)]
        [Display(ResourceType = typeof(ManBox.Common.Resources.AccountMetadata), Name = "LabelFirstName")]
        public string FirstName { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManBox.Common.Resources.AccountMetadata), ErrorMessageResourceName = "ErrorMessageRequired", ErrorMessage = null)]
        [Display(ResourceType = typeof(ManBox.Common.Resources.AccountMetadata), Name = "LabelMessage")]
        public string Message { get; set; }
    }
}
