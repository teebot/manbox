using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManBox.Common.Mail
{
    public class MailRecipient
    {
        public MailRecipient(string address, string name)
        {
            Address = address;
            Name = name;
        }

        public string Address { get; set; }

        public string Name { get; set; }
    }
}
