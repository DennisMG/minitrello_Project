// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="">
//   Copyright � 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.MiniTrello.Web
{
    using System.Web;
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/css/app").Include("~/content/app.css", "~/content/toastr.css"));

            bundles.Add(new ScriptBundle("~/js/jquery").Include("~/scripts/vendor/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/app").Include(
                "~/scripts/vendor/angular-ui-router.js",
                "~/scripts/filters.js",
                "~/scripts/services.js",
                "~/scripts/AccountServices.js",
                "~/scripts/LaneServices.js",
                "~/scripts/OrganizationServices.js",
                "~/scripts/BoardServices.js",
                "~/scripts/directives.js",
                "~/scripts/bootstrap-collapse.js",
                "~/scripts/BoardController.js",
                "~/scripts/toastr.js",
                "~/scripts/toastr.min.js",
                "~/scripts/controllers.js",
                "~/scripts/app.js"));
        }
    }
}
