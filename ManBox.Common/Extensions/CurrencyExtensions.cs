using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Extensions
{
    public static class CurrencyExtensions
    {
        public static string ToCurrency(this decimal price)
        {
            var result = price.ToString("C");
            result = result.Replace(",00", "");
            return result;
        }

        public static string ToCurrency(this decimal? price)
        {
            if (price == null) {
                return null;
            }

            return price.Value.ToCurrency();
        }
    }
}
