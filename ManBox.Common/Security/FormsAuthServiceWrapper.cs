using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ManBox.Common.Security
{
    public class FormsAuthServiceWrapper : IAuthenticationService
    {
        public void DoAuth(string email, bool remember)
        {
            FormsAuthentication.SetAuthCookie(email, remember);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

    }
}
