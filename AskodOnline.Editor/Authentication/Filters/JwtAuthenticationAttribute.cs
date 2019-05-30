using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using AskodOnline.Editor.Authentication.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AskodOnline.Editor.Authentication.Filters
{
	public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		public string Realm { get; set; }
		public bool AllowMultiple => false;

		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			var request = context.Request;
			var authorization = request.Headers.Authorization;

			if (authorization == null || authorization.Scheme != "Bearer")
				return;

			if (string.IsNullOrEmpty(authorization.Parameter))
			{
				context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
				return;
			}
		   
			var token = authorization.Parameter;
			var authenticateJwtTokenResult = await AuthenticateJwtToken(token);

			if (authenticateJwtTokenResult.Exception != null)
			{
				if (authenticateJwtTokenResult.Exception.GetType() == typeof(SecurityTokenExpiredException))
				{
					context.ErrorResult = new AuthenticationFailureResult("Token-Expired", request);
				}
			}
			else
			{
				if (authenticateJwtTokenResult.Principal == null)
					context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
				else
					context.Principal = authenticateJwtTokenResult.Principal;
			}
		}

		protected Task<AuthenticateJwtTokenResult> AuthenticateJwtToken(string token)
		{
			var tokenValidationResult = ValidateToken(token, out var username);

			if (tokenValidationResult.IsValid)
			{
				// based on username to get more information from database in order to build local identity
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, username)
					// Add more claims if needed: Roles, ...
				};

				var identity = new ClaimsIdentity(claims, "Jwt");
				IPrincipal user = new ClaimsPrincipal(identity);
				return Task.FromResult(new AuthenticateJwtTokenResult(user, null));
			}

			return Task.FromResult(new AuthenticateJwtTokenResult(null, tokenValidationResult.Exception));
		}

		private static TokenValidationResult ValidateToken(string token, out string username)
		{
			username = null;

			var tokenValidationResult = JwtManager.GetPrincipal(token);
			var identity = tokenValidationResult.ClaimsIdentity;
			if (tokenValidationResult.Exception != null || identity == null || !identity.IsAuthenticated)
			{
				tokenValidationResult.IsValid = false;
				return tokenValidationResult;
			}

			var usernameClaim = identity.FindFirst(ClaimTypes.Name);
			username = usernameClaim?.Value;

			if (string.IsNullOrEmpty(username))
			{
				tokenValidationResult.IsValid = false;
				return tokenValidationResult;
			}

			// More validate to check whether username exists in system
			tokenValidationResult.IsValid = true;
			return tokenValidationResult;
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			Challenge(context);
			return Task.FromResult(0);
		}

		private void Challenge(HttpAuthenticationChallengeContext context)
		{
			string parameter = null;

			if (!string.IsNullOrEmpty(Realm))
				parameter = "realm=\"" + Realm + "\"";

			context.ChallengeWith("Bearer", parameter);
		}
	}
}