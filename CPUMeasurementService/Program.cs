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
                    .AddSerilog(new LoggerConfiguration().WriteTo.File("CPUMeasurementService.log").CreateLogger());
                    builder.AddSerilog(dispose: true);
                });

                services.AddSingleton<CancelService>();
                services.AddSingleton<CycleStorageService>();
                services.AddSingleton<ComputerDiagnostic>();
                services.AddSingleton<ClientConfigurationReader>();

                services.AddHostedService<MeasurementService>();
                services.AddHostedService<ManagementService>();
                
                
                
            });
        
    }
}
