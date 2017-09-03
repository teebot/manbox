using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public int LanguageId { get; set; }

        public string LanguageIsoCode { get; set; }
    }
}
