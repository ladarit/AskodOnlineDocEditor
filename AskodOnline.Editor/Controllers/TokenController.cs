using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using AskodOnline.AdminDAL.Objects;
using AskodOnline.Editor.Authentication;
using AskodOnline.Editor.Authentication.Models;
using AskodOnline.Editor.Business.AdminStore;
using AskodOnline.Editor.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace AskodOnline.Editor.Controllers
{
	public class TokenController : ApiController
	{
		protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private IAdminStore<AdminEntity> AdminStore { get; }

		public TokenController(IAdminStore<AdminEntity> adminStore)
		{
			AdminStore = adminStore;
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("api/token")]
		public async Task<IHttpActionResult> GetToken(AuthRequsetModel model)
		{
			var user = await AdminStore.GetUserAsync(model.Username);
			if (user == null || !user.Password.Equals(model.Password))
			{
				return Content(HttpStatusCode.Unauthorized, "Invalid login/password");
			}
			return Ok(new AuthResponseModel { Token = JwtManager.GenerateToken(model.Username), RefreshToken = user.RefreshToken });
		}


		[HttpPost]
		[AllowAnonymous]
		[Route("api/refreshtoken")]
		public async Task<IHttpActionResult> Refresh(RefreshTokenRequestModel model)
		{
			var accessToken = model.Token.Replace("Bearer ", string.Empty);
			var principal = JwtManager.GetPrincipalFromExpiredToken(accessToken);
			var username = principal.Identity.Name;
			var user = await AdminStore.GetUserAsync(username);
			if (user == null)
			{
				return Content(HttpStatusCode.InternalServerError, "can`t find user");
			}
			if (user.RefreshToken != model.RefreshToken)
			{
				return Content(HttpStatusCode.Unauthorized, new SecurityTokenException("Invalid refresh token"));
			}

			var newAccessToken = JwtManager.GenerateToken(username);
			var newRefreshToken = JwtManager.GenerateRefreshToken();
			user.RefreshToken = newRefreshToken;
			await AdminStore.UpdateRecordAsync(user);
			
			return Ok(new AuthResponseModel { Token = newAccessToken, RefreshToken = newRefreshToken });
		}
	}
}
