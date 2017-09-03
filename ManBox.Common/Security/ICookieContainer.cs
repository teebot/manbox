using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Security
{
    public interface ICookieContainer
    {
        string this[string key] { get; }

        void SetCookie(string key, string value, DateTime expirationDate);
    }
}
