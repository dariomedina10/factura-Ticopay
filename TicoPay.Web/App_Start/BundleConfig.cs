using System.Web.Optimization;

namespace TicoPay.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            bundles.Add(
                new StyleBundle("~/Bundles/vendor/css")
                    //.Include("~/Content/themes/base/all.css", new CssRewriteUrlTransform())
                    //.Include("~/Content/bootstrap.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/toastr.css")
                    .Include("~/Scripts/sweetalert/sweet-alert.css")
                .Include("~/Content/flags/famfamfam-flags.css", new CssRewriteUrlTransform())
                .Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
                );

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));


            bundles.Add(new ScriptBundle("~/Bundles/test").Include(
                        "~/Scripts/json2.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/moment-with-locales.min.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/sweetalert/sweet-alert.min.js",
                        "~/Scripts/others/spinjs/spin.js",
                        "~/Scripts/others/spinjs/jquery.spin.js",
                        "~/Abp/Framework/scripts/abp.js",
                        "~/Abp/Framework/scripts/libs/abp.jquery.js",
                        "~/Abp/Framework/scripts/libs/abp.toastr.js",
                        "~/Abp/Framework/scripts/libs/abp.blockUI.js",
                        "~/Abp/Framework/scripts/libs/abp.sweet-alert.js",
                        "~/Abp/Framework/scripts/libs/abp.spin.js"
                    )
                );

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap-toggle.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/json2.min.js",
                        "~/Scripts/plugins/iCheck/icheck.min.js",
                       //"~/Scripts/jquery.unobtrusive-ajax.min.js",
                       "~/Scripts/moment-with-locales.min.js",
                       "~/Scripts/toastr.js",
                       "~/Scripts/plugins/debounce/debounce.js",
                       "~/Scripts/sweetalert/sweet-alert.min.js",
                       "~/Scripts/others/spinjs/spin.js",
                       "~/Scripts/others/spinjs/jquery.spin.js",
                       "~/Abp/Framework/scripts/abp.js",
                       "~/Abp/Framework/scripts/libs/abp.jquery.js",
                       "~/Abp/Framework/scripts/libs/abp.toastr.js",
                       "~/Abp/Framework/scripts/libs/abp.blockUI.js",
                       "~/Abp/Framework/scripts/libs/abp.sweet-alert.js",
                       "~/Abp/Framework/scripts/libs/abp.spin.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                "~/Scripts/app/inspinia.js"));


            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include(
                "~/Scripts/plugins/metisMenu/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/pace").Include(
                "~/Scripts/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/datatable").Include(
                "~/Scripts/plugins/dataTables/jszip.min.js",
                "~/Scripts/plugins/dataTables/pdfmake.min.js",
                "~/Scripts/plugins/dataTables/vfs_fonts.js",
                "~/Scripts/plugins/dataTables/dataTables.min.js",
                "~/Scripts/plugins/dataTables/dataTables.bootstrap.min.js",
                "~/Scripts/plugins/dataTables/dataTables.responsive.min.js",
                "~/Scripts/plugins/dataTables/dataTables.buttons.min.js",
                "~/Scripts/plugins/dataTables/buttons.flash.min.js",
                "~/Scripts/plugins/dataTables/buttons.html5.min.js",
                "~/Scripts/plugins/dataTables/buttons.print.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/grid").Include(
                //  "~/Scripts/grid/grid.js",
                "~/Scripts/grid/grid.min.js",
                 "~/Scripts/grid/messages/messages.bg-bg.js",
                  "~/Scripts/grid/messages/messages.de-de.js",
                 "~/Scripts/grid/messages/messages.fr-fr.js",
                 "~/Scripts/grid/messages/messages.pt-br.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
               "~/Scripts/plugins/flot/jquery.flot.js",
                "~/Scripts/plugins/flot/jquery.flot.pie.js",
                 "~/Scripts/plugins/flot/jquery.flot.resize.js",
                "~/Scripts/plugins/flot/jquery.flot.spline.js",
                 "~/Scripts/plugins/flot/jquery.flot.time.js",
                  "~/Scripts/plugins/flot/jquery.flot.tooltip.min.js",
                "~/Scripts/plugins/flot/jquery.flot.symbol.js"));

            bundles.Add(new ScriptBundle("~/plugins/chosen").Include(
                "~/Scripts/plugins/chosen/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/plugins/cronExpression").Include(
                "~/Scripts/plugins/cronExpression/jquery-cron.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      // "~/Content/bootstrap.min.css",
                      //  "~/Content/bootstrap-theme.min.css",
                      "~/Content/animate.css",
                       "~/Content/switchery.css",
                      "~/Content/plugins/dataTables/datatables.min.css",
                      "~/Content/plugins/dataTables/dataTables.bootstrap.min.css",
                      "~/Content/plugins/dataTables/responsive.dataTables.min.css",
                      "~/Content/plugins/chosen/chosen.css",
                      "~/Content/plugins/cronExpression/jquery-cron.css",
                      "~/Content/grid/grid.min.css",
                      "~/Content/style.css"));


            bundles.Add(new StyleBundle("~/bundles/css").Include(
                     "~/css/main.less.css"));

            bundles.Add(new ScriptBundle("~/plugins/pwstrength").Include(
                "~/Scripts/plugins/pwstrength/i18next.js",
                "~/Scripts/plugins/pwstrength/pwstrength-bootstrap.min.js"
                ));

        }
    }
}