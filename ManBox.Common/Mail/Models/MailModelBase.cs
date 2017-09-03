using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManBox.Common.Mail.Models
{
    public class MailModelBase
    {
        public virtual string Subject { get; set; }
        public string Name { get; set; }
        public string RootUrl { get; set; }
        public DateTime Date { get; set; }
        public string LanguageIso { get; set; }
    }
}
