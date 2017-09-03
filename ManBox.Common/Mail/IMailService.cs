using System;
using ManBox.Common.Mail.Models;
namespace ManBox.Common.Mail
{
    public interface IMailService
    {
        void SendMail<T>(MailRecipient toRecipient, MailRecipient fromRecipient, T emailModel) where T : MailModelBase;
        void SendMail(MailRecipient toRecipient, MailRecipient fromRecipient, string subject, string content);
    }
}
