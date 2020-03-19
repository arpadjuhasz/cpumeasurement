using CPUMeasurementBackend.WebService.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService
{
    public  static class AuthorizationService
    {
        public static void AddJwtValidation(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("Salt"));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(x =>
                    {
                        x.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                var accountService = context.HttpContext.RequestServices.GetRequiredService<AccountService>();
                                var accountId = int.Parse(context.Principal.Identity.Name);
                                AccountId = accountId;
                                var account = accountService.GetAccountById(accountId);
                                StringValues authorization = string.Empty;
                                context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorization);
                                var storedTokens = accountService.GetTokensByUserId(accountId);
                                Token = authorization.ToString().Substring(7);
                                if (account == null || !storedTokens.Contains(Token))
                                {
                                    accountService.DeleteAccessToken(accountId);
                                    // return unauthorized if user no longer exists
                                    context.Fail("Unauthorized");
                                }
                                return Task.CompletedTask;
                            }
                        };
                        x.RequireHttpsMetadata = false;
                        x.SaveToken = true;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
        }

        public static int AccountId = 0;
        public static string Token = null;

        
    }
}
