using System.Configuration;
using Segmentio;

namespace ManBox.Web.App_Start
{
    public static class AnalyticsConfig
    {
        public static void Register()
        {
            var segmentIoId = ConfigurationManager.AppSettings[ManBox.Common.AppConstants.AppSettingsKeys.SegmentIO];
            Analytics.Initialize(segmentIoId);
        }
    }
}