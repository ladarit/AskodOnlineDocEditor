using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;

namespace AskodOnline.Editor
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			BundleTable.EnableOptimizations = true;

			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
					"~/Scripts/vendor/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
					"~/Scripts/vendor/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
					"~/Scripts/vendor/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					"~/Scripts/vendor/bootstrap.js"));

			bundles.Add(new StyleBundle("~/Content/Admin_css").Include(
					"~/Content/adminPage.css"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					"~/Content/vendor/bootstrap.css",
					"~/Content/site.css",
					"~/Content/SpreadSheet.css"));

			var reactCssBundle = new StyleBundle("~/Content/reactCss").Include(
				"~/Content/SPA_bundle.css");

			reactCssBundle.Transforms.Clear();

			var reactJsBundle = new ScriptBundle("~/bundles/reactApp").Include(
					"~/Scripts/build/*.js");

			reactJsBundle.Orderer = new ReactFilesBundleOrderer();

			bundles.Add(reactCssBundle);
			bundles.Add(reactJsBundle);
		}
	}

	class ReactFilesBundleOrderer : IBundleOrderer
	{
		public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
		{
			var bundleFiles = files.ToList();
			var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
			bundleFiles.ForEach(f => f.IncludedVirtualPath = urlHelper.ContentVersioned(f.IncludedVirtualPath));
			var indexFile = bundleFiles.FirstOrDefault(f => f.VirtualFile.Name != null && f.VirtualFile.Name.StartsWith("vendor"));
			if (indexFile != null)
			{
				bundleFiles.Remove(indexFile);
				bundleFiles.Insert(0, indexFile);
			}
			return bundleFiles;
		}
	}
}
