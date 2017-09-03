using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Security
{
    public class ManBoxIdentity : GenericIdentity
    {
        public ManBoxUser User { get; set; }

        public ManBoxIdentity(ManBoxUser user)
            : base(user.Email ?? String.Empty)
        {
            user.IsAuthenticated = base.IsAuthenticated;
            User = user;
        }

        public override string Name
        {
            get
            {
                return User.Email;
            }
        }

        // TODO: remove this dirty hack , only done foir unit test stub
        public void SetAuthentication(bool isAuthenticated) 
        {
            this.User.IsAuthenticated = isAuthenticated;
        }
    }

    public class ManBoxUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int LanguageId { get; set; }
        public string LanguageIsoCode { get; set; }
        public int CountryId { get; set; }
        public string CountryIsoCode { get; set; }

        public bool IsAuthenticated { get; set; }
    }

    public class ManBoxPrincipal : IPrincipal
    {
        private ManBoxIdentity _identity;

        public ManBoxPrincipal(ManBoxIdentity identity)
        {
            _identity = identity;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public ManBoxUser ManBoxUser
        {
            get { return _identity.User; }
        }

        public bool IsInRole(string role)
        {
            return false;
        }
        #endregion
    }

    public static class IdHelper
    {
        public static ManBoxUser CurrentUser
        {
            get
            {
                var id = System.Threading.Thread.CurrentPrincipal.Identity as ManBoxIdentity;
                if (id != null)
                {
                    return id.User;
                }
                return new ManBoxUser();
            }
        }
    }
}
