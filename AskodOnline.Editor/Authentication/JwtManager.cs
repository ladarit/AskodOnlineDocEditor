using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AskodOnline.Editor.Authentication
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static string GenerateToken(string username, int expireMinutes = 10)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
				NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static TokenValidationResult GetPrincipal(string token)
        {
	        try
	        {
		        var tokenHandler = new JwtSecurityTokenHandler();
		        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

		        if (jwtToken == null)
			        return null;

		        var symmetricKey = Convert.FromBase64String(Secret);

		        var validationParameters = new TokenValidationParameters
		        {
			        RequireExpirationTime = true,
			        ValidateIssuer = false,
			        ValidateAudience = false,
			        ValidateLifetime = true,
			        ClockSkew = TimeSpan.Zero,
			        IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
		        };

		        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
		        return new TokenValidationResult
		        {
			        ClaimsIdentity = claimsPrincipal.Identity as ClaimsIdentity,
					Exception = null
		        };
	        }
			catch (Exception e)
	        {
				return new TokenValidationResult
				{
					ClaimsIdentity = null,
					Exception = e
				};
			}
		}

	    public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var symmetricKey = Convert.FromBase64String(Secret);

			var tokenValidationParameters = new TokenValidationParameters
		    {
			    ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
			    ValidateIssuer = false,
			    //ValidateIssuerSigningKey = true,
			    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
			    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
		    };

		    var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
		    var jwtSecurityToken = securityToken as JwtSecurityToken;
		    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			    throw new SecurityTokenException("Invalid token");

		    return principal;
	    }

	    public static string GenerateRefreshToken()
	    {
		    var randomNumber = new byte[32];
		    using (var rng = RandomNumberGenerator.Create())
		    {
			    rng.GetBytes(randomNumber);
			    return Convert.ToBase64String(randomNumber);
		    }
	    }
	}
}