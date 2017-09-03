using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Common.Mail.Models
{
    public class NewsletterWelcomeMail : MailModelBase
    {
        public string CouponCode { get; set; }
    }
}
