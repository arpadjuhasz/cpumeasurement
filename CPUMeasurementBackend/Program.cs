using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPUMeasurementBackend.Repositories;
using CPUMeasurementBackend.HostedService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CPUMeasurementBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://lcoalhost:5000");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddLogging(builder =>
                    {
                        builder.AddConfiguration(configuration.GetSection("Logging"))
                          .AddSerilog(new LoggerConfiguration().WriteTo.File(@"CPUMeasurementBackend.log").CreateLogger());
                        builder.AddSerilog(dispose: true);
                       
                    });
                    services.AddHostedService<MeasurementListener>();
                    services.AddSingleton<Management>();
                    services.AddHostedService<ManagementListener>();
                    services.AddScoped<CPUMeasurementRepository>();
            });

                
    }
}
