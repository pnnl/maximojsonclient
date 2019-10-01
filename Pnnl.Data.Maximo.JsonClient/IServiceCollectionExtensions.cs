using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Helper functions for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add Maximo JSON connection pooling to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to configure.</param>
        /// <param name="configuration">The configuration being bound.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> collection is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddMaximoJsonConnectionPool(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddMemoryCache();
            services.AddOptions();

            services.TryAdd(ServiceDescriptor.Singleton<IMaximoJsonClientPool>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<MaximoJsonClientPoolOptions>>();
                var memoryCache = provider.GetRequiredService<IMemoryCache>();
                var logger = provider.GetRequiredService<ILogger<MaximoJsonClientPool>>();

                return new MaximoJsonClientPool(options, memoryCache, logger);
            }));

            services.Configure<MaximoJsonClientPoolOptions>(configuration);

            return services;
        }
    }
}
