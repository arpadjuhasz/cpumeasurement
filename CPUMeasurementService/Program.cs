using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CPUMeasurementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(configuration.GetSection("Logging"))
                    .AddSerilog(new LoggerConfiguration().WriteTo.File("cpumeasurementservice.log").CreateLogger());
                });
                services.AddHostedService<CPUMeasurementService>();
            });
        
    }
}
