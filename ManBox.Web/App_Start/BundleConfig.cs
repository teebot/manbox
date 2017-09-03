using System.Web;
using System.Web.Optimization;

namespace ManBox.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new LessBundle("~/Content/less/manbox").Include("~/Content/less/manbox.less", "~/Content/less/responsive.less"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui*",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.mapping-latest.js",
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/konami.js",
                "~/Scripts/manbox.js",
                "~/Scripts/retina.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/compose").Include(
                        "~/Scripts/manbox.compose.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/size").Include(
                        "~/Scripts/manbox.size.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/checkout").Include(
                        "~/Scripts/manbox.checkout.js"
                ));
        }
    }
}