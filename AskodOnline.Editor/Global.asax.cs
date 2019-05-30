using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AskodOnline.Editor.Business.Store;
using AskodOnline.Editor.Helpers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FluentScheduler;
using Microsoft.AspNet.SignalR;

namespace AskodOnline.Editor
{
	public class MvcApplication : HttpApplication
	{
		protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private WindsorContainer _windsorContainer;

		protected void Application_Start()
		{
			Log.Info("APP START");
			InitializeWindsor();
			var signalrDependency = new SignalRDependencyResolver(_windsorContainer.Kernel);
			GlobalHost.DependencyResolver = signalrDependency;
			GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(60);
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			JobManager.Initialize(new Sheduler());
		}

		protected void Application_End()
		{
			Log.Info("APP END");
			_windsorContainer?.Dispose();
		}

		private void InitializeWindsor()
		{
			_windsorContainer = new WindsorContainer();
			_windsorContainer.Install(FromAssembly.This());
			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_windsorContainer.Kernel));
			GlobalConfiguration.Configuration.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new WindsorHttpControllerActivator(_windsorContainer));
		}

		protected void Application_AcquireRequestState(object sender, EventArgs e)
		{
			var culture = Request.Form.GetValues("userLang")?.FirstOrDefault();
			if (string.IsNullOrEmpty(culture))
			{
				if (Request.Cookies["AODECulture"] != null && !string.IsNullOrEmpty(Request.Cookies["AODECulture"].Value))
				{
					culture = Request.Cookies["AODECulture"].Value;
					ChangeCulture(culture);
				}
			}
			else
			{
				if (culture != System.Threading.Thread.CurrentThread.CurrentUICulture.Name)
				{
					ChangeCulture(culture);
				}
			}
		}

		private void ChangeCulture(string culture)
		{
			CultureInfo ci = new CultureInfo(culture);
			System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
		}

		protected void Application_Error()
		{
			var ex = Server.GetLastError();
			Log.Error(ex);
		}
	}
}
