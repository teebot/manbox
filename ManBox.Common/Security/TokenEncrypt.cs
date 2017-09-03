using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Security
{
    public static class TokenEncrypt
    {
        private static string dateFormat = "yyyyddMMHHmm";
        private static byte[] entropy = new byte[] { 0x11, 0x09, 0x22, 0x03, 0x21, 0x01, 0x02 };

        /// <summary>
        /// Will return a token only if its date is still valid
        /// </summary>
        /// <param name="tokenDateToDecrypt"></param>
        /// <returns></returns>
        public static string DecryptExpiringToken(string tokenDateToDecrypt)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(tokenDateToDecrypt);
            byte[] unprotectedData = ProtectedData.Unprotect(encodedDataAsBytes, entropy, DataProtectionScope.CurrentUser);

            string decoded = Encoding.Unicode.GetString(unprotectedData);

            string[] tokenDateParts = decoded.Split('%');
            DateTime validDate = DateTime.ParseExact(tokenDateParts[0], dateFormat, System.Globalization.CultureInfo.InvariantCulture);

            if (DateTime.Now < validDate)
            {
                return tokenDateParts[1];
            }

            return null;
        }

        /// <summary>
        /// Encrypt a token and append a validity date to it
        /// </summary>
        /// <param name="tokenToEncrypt"></param>
        /// <param name="expirationDate"></param>
        /// <returns></returns>
        public static string EncryptTokenAsExpiring(string tokenToEncrypt, DateTime expirationDate)
        {
            var date = expirationDate.ToString(dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            var toEncrypt = string.Format("{0}%{1}", date, tokenToEncrypt);

            byte[] sensitiveData = Encoding.Unicode.GetBytes(toEncrypt);
            byte[] protectedData = ProtectedData.Protect(sensitiveData, entropy, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(protectedData);
        }
    }
}
