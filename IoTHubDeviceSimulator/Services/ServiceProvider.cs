using Microsoft.Extensions.DependencyInjection;
using System;

namespace IoTHubDeviceSimulator.Services
{
    public static class ServiceProvider
    {
        private static Lazy<IServiceProvider> LazyContainer = new Lazy<IServiceProvider>(RegisterContainer);

        /// <summary>
        /// IoC container.
        /// </summary>
        public static IServiceProvider Container => LazyContainer.Value;

        /// <summary>
        /// Registers services for DI.
        /// </summary>
        private static IServiceProvider RegisterContainer()
        {
            var services = new ServiceCollection();
            services.AddLogger();
            return services.BuildServiceProvider();
        }
    }
}
