using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPUMeasurementBackend.Repository;
using CPUMeasurementBackend.HostedService;
using CPUMeasurementBackend.WebService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using CPUMeasurementBackend.WebService.Account;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Primitives;

namespace CPUMeasurementBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddCors();
            services.AddControllers();
            services.AddScoped<CPUDataRepository>();
            services.AddScoped<MeasurementService>();
            services.AddScoped<ManagementService>();
            services.AddScoped<AccountService>();
            services.AddScoped<AccountRepository>();
            services.AddJwtValidation(this.Configuration);
            //var key = Encoding.ASCII.GetBytes(this.Configuration.GetValue<string>("Salt"));
            //services.AddAuthentication(x =>
            //    {
            //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(x =>
            //    {
            //        x.Events = new JwtBearerEvents
            //        {
            //            OnTokenValidated = context =>
            //            {
            //                var accountService = context.HttpContext.RequestServices.GetRequiredService<AccountService>();
            //                var accountId = int.Parse(context.Principal.Identity.Name);
            //                var account = accountService.GetAccountById(accountId);
            //                StringValues authorization = string.Empty;
            //                context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorization);
            //                var storedToken = accountService.GetTokenByUserId(accountId);
            //                if (account == null || $"Bearer {storedToken}" != authorization.ToString())
            //                {
            //                    accountService.DeleteAccessToken(accountId);
            //                // return unauthorized if user no longer exists
            //                context.Fail("Unauthorized");
            //                }
            //                return Task.CompletedTask;
            //            }
            //        };
            //        x.RequireHttpsMetadata = false;
            //        x.SaveToken = true;
            //        x.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(key),
            //            ValidateIssuer = false,
            //            ValidateAudience = false
            //        };
            //    });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
