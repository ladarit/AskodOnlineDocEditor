using System;
using System.Net;
using System.Reflection;
using System.Web.Http;
using AskodOnline.Editor.Authentication.Filters;
using AskodOnline.Editor.Helpers;
using DBConnectionInspector;

namespace AskodOnline.Editor.Controllers
{
	public class DashboardController : ApiController
	{
		protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[JwtAuthentication]
		[Route("api/dashboard/dbinspector")]
		public IHttpActionResult Get()
		{
			try
			{
				var result = DbConnectionInspector.TestDbConnection(DbType.Oracle, System.Configuration.ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString);
				return Ok(result);
			}
			catch (Exception e)
			{
				Log.Error(e);
				return Content(HttpStatusCode.InternalServerError, e.Message);
			}
		}
	}
}
