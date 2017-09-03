using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class PaymentParametersViewModel
    {
        public int SubscriptionId { get; set; }

        public decimal Total { get; set; }

        public string TotalStr
        {
            get
            { 
                return Total.ToString("F", new System.Globalization.CultureInfo("en-US"));
            }
        }

        public int TotalInt { 
            get {
                return Convert.ToInt32(Total * 100);
            } 
        }

        public string Account { get; set; }

        public bool AlreadyMember { get; set; }

        public string GatewayUrl { get; set; }

        public bool IsModification { get; set; }

        public UserViewModel User { get; set; }
    }
}
