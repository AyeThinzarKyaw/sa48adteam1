using System.Web;
using System.Web.Optimization;

namespace LUSSIS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/admin-lte/css/AdminLTE.css",
                      "~/admin-lte/css/skins/_all-skins.min.css",
                      "~/admin-lte/css/skins/skin-blue.css",
                      "~/admin-lte/plugins/AdminLTE/bower_components/Ionicons/css/ionicons.min.css",
                      "~/admin-lte/plugins/AdminLTE/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/admin-lte/plugins/AdminLTE/plugins/iCheck/all.css",
                      "~/admin-lte/plugins/AdminLTE/bower_components/datatables.net-bs/css/dataTables.bootstrap.min.css"
                      ));

            bundles.Add(new ScriptBundle("~/admin-lte/js").Include(
                "~/admin-lte/js/app.js",
                "~/admin-lte/js/adminlte.min.js",
                "~/admin-lte/plugins/AdminLTE/bower_components/fastclick/fastclick.js",
                "~/admin-lte/plugins/AdminLTE/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/admin-lte/plugins/AdminLTE/bower_components/datatables.net/js/jquery.dataTables.min.js",
                "~/admin-lte/plugins/AdminLTE/plugins/iCheck/icheck.min.js",
                "~/admin-lte/plugins/AdminLTE/bower_components/datatables.net-bs/js/dataTables.bootstrap.min.js"
                ));
        }
    }
}
