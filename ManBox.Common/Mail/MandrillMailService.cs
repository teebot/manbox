using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleInjector;
using MailChimp;
using ManBox.Common.Logging;
using ManBox.Common.Mail.Models;
using ManBox.Common.Properties;
using MandrillMsg = MailChimp.Types.Mandrill.Messages.Message;

namespace ManBox.Common.Mail
{
    public class MandrillMailService : IMailService
    {
        /// <summary>
        /// Sends a templated mail
        /// </summary>
        public void SendMail<T>(MailRecipient toRecipient, MailRecipient fromRecipient, T emailModel) where T : MailModelBase
        {
            // By convention the type name of the mail model is the name of the template file
            var templateName = emailModel.GetType().Name.ToLower();

            // initialize template
            var templatePath = string.Format("{0}{1}.{2}.cshtml", Settings.Default.EmailTemplatesPath, templateName, emailModel.LanguageIso);
            TemplateEngine<T> templateEngine = new TemplateEngine<T>(emailModel, templatePath);

            SendMail(toRecipient, fromRecipient, emailModel.Subject, templateEngine.Render());
        }

        /// <summary>
        /// Full parameters send emthod
        /// </summary>
        public void SendMail(MailRecipient toRecipient, MailRecipient fromRecipient, string subject, string content)
        {
            // init mail service
            MandrillApi man = new MandrillApi(Settings.Default.MandrillApiKey);

            // add recipient to list
            var rec = new List<MailChimp.Types.Mandrill.Messages.Recipient>();
            rec.Add(new MailChimp.Types.Mandrill.Messages.Recipient(toRecipient.Address, toRecipient.Name));

            // sending
            var result = man.Send(new MandrillMsg()
            {
                FromEmail = fromRecipient.Address,
                FromName = fromRecipient.Name,
                Html = content,
                Subject = subject,
                To = rec.ToArray()
#if DEBUG
,
                BccAddress = "thibautnguyen@gmail.com"
#endif
            });

            if (result.First().Status != MailChimp.Types.Mandrill.Messages.Status.Sent)
            {
                var msg = string.Format("error sending mail to {0} from {1}. Status is {2}", toRecipient.Address, fromRecipient.Address, result.First().Status);

                new NLogLogger().Log(LogType.Warn, msg); //TODO: call interface method through the DI 
            }
        }
    }
}