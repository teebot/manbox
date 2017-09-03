using ManBox.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.UnitTesting
{
    public class MockAuthenticationService : IAuthenticationService
    {
        public static bool LoggedIn;

        public MockAuthenticationService()
        {
            LoggedIn = false;
        }

        public void DoAuth(string username, bool remember)
        {
            LoggedIn = true;
        }

        public void SignOut()
        {
            LoggedIn = false;
        }
    }
}
