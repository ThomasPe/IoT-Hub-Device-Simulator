using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IoTHubDeviceSimulator.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLogger(this ServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                var logger = new LoggerConfiguration()
                    .WriteTo.Sink(new ActionSink())
                    .WriteTo.Debug()
                    .CreateLogger();

                loggingBuilder.AddSerilog(logger);
            });
        }
    }
}
