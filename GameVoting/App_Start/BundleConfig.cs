using System.Web;
using System.Web.Optimization;

namespace GameVoting
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/chosen.jquery.js",
                        "~/Scripts/select2.js",
                        "~/Scripts/App/RandomGenerator.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/Index").Include(
                        "~/Scripts/Event/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/Event").Include(
                        "~/Scripts/jquery.signalR-{version}.js",
                        "~/Scripts/highcharts-custom.js",
                        "~/Scripts/Event/Event.js"));

            bundles.Add(new ScriptBundle("~/bundles/SevenWonders").Include(
                        "~/Scripts/SevenWonders/SevenWonders.js"));
            #endregion

            #region Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/chosen.css",
                        "~/Content/select2.css",
                        "~/Content/Site.css"));
            #endregion
        }
    }
}