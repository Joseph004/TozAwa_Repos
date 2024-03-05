
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrleansHost.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static TConfig ConfigureAppSettings<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services != null)
            {
                if (configuration != null)
                {
                }
                else
                {
                    throw new ArgumentNullException(nameof(configuration));
                }

                var config = new TConfig();
                configuration.Bind(config);
                services.AddSingleton(config);
                return config;
            }

            throw new ArgumentNullException(nameof(services));
        }
    }
}