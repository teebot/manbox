using ManBox.Common.Mail;
using ManBox.Common.Mail.Models;
using ManBox.Common.Properties;

namespace ManBox.Common.UnitTesting
{
    public class MockMailService : IMailService
    {
        public static object EmailModel;
        public static string EmailContent;

        public void SendMail<T>(MailRecipient toRecipient, MailRecipient fromRecipient, T emailModel) where T : MailModelBase
        {
            // By convention the type name of the mail model is the name of the template file
            var templateName = emailModel.GetType().Name.ToLower();

            // initialize template
            var templatePath = string.Format("{0}{1}.{2}.cshtml", Settings.Default.EmailTemplatesPath, templateName, emailModel.LanguageIso);
            TemplateEngine<T> templateEngine = new TemplateEngine<T>(emailModel, templatePath);

            EmailModel = emailModel;
            EmailContent = templateEngine.Render();
        }

        public void SendMail(MailRecipient toRecipient, MailRecipient fromRecipient, string subject, string content)
        {
            
        }
    }
}
