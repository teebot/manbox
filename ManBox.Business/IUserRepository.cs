using System;
using ManBox.Model.ViewModels;
namespace ManBox.Business
{
    public interface IUserRepository
    {
        void SubscribeNewsletterEmail(string email);
        UserViewModel GetUserByEmail(string email);
        UserViewModel Login(string email, string password, string subscrToken);
        UserViewModel Register(UserViewModel user, string signinType, string subscrToken);
        void SendSupportMail(ContactFormViewModel contactForm);
        void Unsubscribe(string email);
        OrdersOverviewViewModel GetOrdersOverview();
        UserViewModel TokenLogin(string encryptedToken);
        bool SendPasswordReset(string email);
        void UpdateAddress(AddressViewModel newAddress);
        void UpdatePassword(string newPassword);
        AddressViewModel GetCurrentUserAddress();
    }
}
