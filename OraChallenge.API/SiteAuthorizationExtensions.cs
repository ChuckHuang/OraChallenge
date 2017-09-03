using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace OraChallenge.API
{
    public static class SiteAuthorizationExtensions
    {
        private static readonly string signingKey= "very_long_very_secret_secret";
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
        static private string GetIssuer()
        {
            return "issuer";
        }

        static private string GetAudience()
        {
            return "audience";
        }
        public static string CreateJwtToken(string userId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = new JwtSecurityToken(issuer: userId,
                signingCredentials: new SigningCredentials(GetSignInKey(), SecurityAlgorithms.HmacSha256));
                var tokenString = tokenHandler.WriteToken(token);
                return tokenString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public static string ReadUserIdFromJwtToken(string tokenStr)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(tokenStr);                  
                return token.Issuer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
