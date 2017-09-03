using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq.Expressions;

namespace ManBox.Common.Security
{
    public class CookieContainer : ICookieContainer
    {
        public string this[string key] {
            get {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                if (httpContext.Request.Cookies.AllKeys.Any(k => k == key))
                {
                    return httpContext.Request.Cookies[key].Value;
                }
                return null;
            }
        }

        public void SetCookie(string key, string value, DateTime expirationDate)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            var newTokenCookie = new HttpCookie(key, value);
            newTokenCookie.Expires = expirationDate;
            if (value == null)
            {
                newTokenCookie.Expires = DateTime.Now.AddDays(-1);
            }
            httpContext.Response.Cookies.Add(newTokenCookie);
        }
    }
}
