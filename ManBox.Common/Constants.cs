
namespace ManBox.Common
{
    public class CodeValues
    {
        public class SubscriptionState 
        {
            public const string Subscribed = "Subscribed";
            public const string InCart = "InCart";
            public const string Checkout = "Checkout";
            public const string Cancelled = "Cancelled";
        }

        public class SignInType
        {
            public const string EmailPass = "EmailPass";
            public const string Facebook = "Facebook";
        }

        public class DeliveryState
        {
            public const string New = "New";
            public const string Dropped = "Dropped";
            public const string Pending = "Pending";
            public const string Processing = "Processing";
            public const string Sent = "Sent";
            public const string Confirmed = "Confirmed";
            public const string Cancelled = "Cancelled";
        }

        public class DeliveryPaymentStatus {
            public const string None = "None";
            public const string Refused = "Refused";
            public const string Failed = "Failed";
            public const string Paid = "Paid";

            public static string[] GetAll() {
                return new string[] { 
                    None, Refused, Failed, Paid
                };
            }
        }

        public class DeliveryMessageType {
            public const string Upcoming = "Upcoming";
            public const string ShippingConfirmation = "ShippingConfirmation";

            public static string[] GetAll()
            {
                return new string[] { 
                    Upcoming, ShippingConfirmation
                };
            }
        }
    }

    public class WebConstants
    {
        public class CacheKeys {
            public const string CatalogOverview = "CatalogOverview";
            public const string BlogFeed = "BlogFeed";
        }

        public class SessionKeys
        {
            public const string User = "User";
            public const string FbAccessToken = "FbAccessToken";
            public const string Lang = "Lang";
        }

        public class TempDataKeys
        {
            public const string PaymentParams = "PaymentParams";
        }

        public class ViewDataKeys 
        {
            public const string IsHome = "IsHome";
        }

        public class RouteNames 
        {
            public const string Default = "Default";
            public const string LocalizedRoute = "LocalizedRoute";
            public const string RootLanguage = "RootLanguage";
        }

        public class PartialViews
        {
            public const string LanguageSwitch = "_LanguageSwitch";
            public const string Footer = "_Footer";
            public const string HeaderMessage = "_HeaderMessage";
            public const string LoginBox = "_LoginBox";
            public const string HowItWorksModal = "_HowItWorksModal";
            public const string WelcomeModal = "_WelcomeModal";
        }

        public class Cookies
        {
            public const string Token = "Token";
        }
    }

    public class PayPalConstants
    {
        public class PaymentExecStatus 
        {
            public const string Completed = "COMPLETED";    
        }

        public class PayerStatus
        {
            public const string Verified = "Verified";
        }

        public class Period3
        {
            public const string ThreeMonths = "3 M";
        }

        public class Formatting
        {
            public const string ZuluDate = "yyyy'-'MM'-'dd'Z'";
        }
    }

    public class AppConstants
    {
        public class Application
        {
            public const string Version = "0.8";
            public const string AppName = "ManBox";
        }

        public class Countries
        {
            public const int FR = 1;
            public const int BE = 2;
            public const int NL = 3;
            public const int LU = 4;

            public const string IsoBE = "BE";
            public const string IsoFR = "FR";
            public const string IsoNL = "NL";
            public const string IsoLU = "LU";
        }

        public class Languages 
        {
            public const string IsoFrench = "fr";
            public const string IsoDutch = "nl";
        }

        public class AppSettingsKeys
        {
            public const string SegmentIO = "segmentIO";
            public const string FbAppId = "fbAppId";
            public const string FbAppSecret = "fbAppSecret";
            public const string PayPalAccount = "paypalAccount";
            public const string PayPalUrl = "paypalUrl";
            public const string CdnImagePath = "cdnImagePath";
            public const string PaymillPublicKey = "paymillPublicKey";
            public const string PaymillApiKey = "paymillApiKey";
            public const string PaymillApiUrl = "paymillApiUrl";
            public const string BlogJsonFeedUrl = "blogJsonFeedUrl";
        }

        public class PaymentMethods
        {
            public const string PayPal = "PayPal";
            public const string CreditCard = "CreditCard";
        }
    }
}

