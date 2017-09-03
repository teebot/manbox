using ManBox.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.UnitTesting
{
    public class MockCookieContainer : ICookieContainer
    {
        private List<LocalCookie> _localStore;

        public MockCookieContainer()
        {
            _localStore = new List<LocalCookie>();
        }

        public string this[string key]
        {
            get
            {
                var cookie = _localStore.FirstOrDefault(c => c.Key == key && c.ExpirationDate > DateTime.Now);
                if (cookie != null)
                {
                    return cookie.Value;
                }
                return null;
            }
        }

        public void SetCookie(string key, string value, DateTime expirationDate)
        {
            var existing = _localStore.FirstOrDefault(l => l.Key == key);

            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                _localStore.Add(new LocalCookie()
                {
                    Key = key,
                    Value = value,
                    ExpirationDate = expirationDate
                });
            }
        }
    }

    public class LocalCookie
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
