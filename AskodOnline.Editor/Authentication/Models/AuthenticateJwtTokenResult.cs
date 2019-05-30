using System;
using System.Security.Principal;

namespace AskodOnline.Editor.Authentication.Models
{
	public class AuthenticateJwtTokenResult
	{
		public AuthenticateJwtTokenResult(IPrincipal principal, Exception exception)
		{
			Principal = principal;
			Exception = exception;
		}

		public IPrincipal Principal { get; }

		public Exception Exception { get;}
	}
}