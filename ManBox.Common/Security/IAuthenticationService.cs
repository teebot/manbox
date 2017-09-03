using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Security
{
    public interface IAuthenticationService
    {
        void DoAuth(string username, bool remember);
        void SignOut();
    }
}
