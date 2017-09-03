using System;
using ManBox.Model.ViewModels;
namespace ManBox.Business
{
    public interface ICheckoutRepository
    {
        CheckoutShippingViewModel GetSubscriptionCheckoutInfo(string userToken);
        void StorePreapprovalAndCharge(PayPalResponseViewModel response, string thankYouUrl, string cancelUrl);
        void ConfirmPaymillPayment(string subscrToken, string paymillToken, string paymillClientId, string paymillPayId);
        PaymentParametersViewModel SaveShippingInfo(ManBox.Model.ViewModels.CheckoutShippingViewModel shippingInfo, string token);
        PaymentParametersViewModel GetPaymentInfo(string subscrToken);
        ModificationResponseViewModel ConfirmModification(string token);
        void CancelModification(string token);
        void SaveGiftPersonalization(string tk, string guestName, string giftMsg);
    }
}
