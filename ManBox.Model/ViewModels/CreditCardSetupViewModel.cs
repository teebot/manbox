using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class CreditCardSetupViewModel
    {
        public string PaymillPublicKey { get; set; }

        public PaymentParametersViewModel PaymentParameters { get; set; }
    }
}
