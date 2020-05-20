using CPUMeasurementBackend.Repositories;
using CPUMeasurementBackend.WebServices;
using CPUMeasurementBackend.WebServices.Accounts;
using CPUMeasurementBackend.WebServices.Authorizations;
using CPUMeasurementBackend.WebServices.Managements;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            RepositoryInitializer.InitializeDatabase(Configuration.GetValue<string>("InitializeConnectionString"));
            services.AddCors();
            services.AddControllers();
            services.AddScoped<CPUMeasurementRepository>();
            services.AddScoped<MeasurementService>();
            services.AddScoped<ManagementService>();
            services.AddScoped<AccountService>();
            services.AddScoped<AccountRepository>();
            services.AddJwtValidation(this.Configuration);
            
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

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
