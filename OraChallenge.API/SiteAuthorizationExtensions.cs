using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OraChallenge.API
{
    public static class SiteAuthorizationExtensions
    {
        public static IServiceCollection AddSiteAuthorization(this IServiceCollection services)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSignInKey(),
                ValidateIssuer = false,
                ValidIssuer = GetIssuer(),
                ValidateAudience = false,
                ValidAudience = GetAudience(),
                ValidateLifetime = false
            };

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


                })

                .AddJwtBearer(o =>
                {
                    o.IncludeErrorDetails = true;
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();

                            c.Response.StatusCode = 401;
                            c.Response.ContentType = "text/plain";

                            return c.Response.WriteAsync(c.Exception.ToString());
                        }

                    };
                });

            return services;
        }

        private static SecurityKey GetSignInKey()
        {
            const string secretKey = "very_long_very_secret_secret";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            return signingKey;
        }
        private static string GetIssuer()
        {
            return "issuer";
        }

        private static string GetAudience()
        {
            return "audience";
        }
        public static string CreateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>();
            claims.Add(new Claim("userId", userId));
            claims.Add(new Claim("createdAt", DateTime.Now.ToString()));
            var token = new JwtSecurityToken(claims: claims,
            signingCredentials: new SigningCredentials(GetSignInKey(), SecurityAlgorithms.HmacSha256));
            
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public static string DecryptUserId(string authorization)
        {
            var tokenStr = authorization.Substring("Bearer ".Length).Trim();
            return ReadUserIdFromJwtToken(tokenStr);
        }

        private static string ReadUserIdFromJwtToken(string tokenStr)
        {
                var tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken token = (JwtSecurityToken)tokenHandler.ReadToken(tokenStr);
                return token.Payload["userId"].ToString();
        }

    }
}
