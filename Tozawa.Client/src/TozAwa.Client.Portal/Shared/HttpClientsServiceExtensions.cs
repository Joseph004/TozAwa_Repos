using Microsoft.Extensions.DependencyInjection;
using Tozawa.Client.Portal.HttpClients;

namespace Tozawa.Client.Portal.Shared
{
    public static class HttpClientsServiceExtensions
    {
        public static IServiceCollection RegisterHttpClients(this IServiceCollection services)
        {
            services.AddScoped<IAuthHttpClient, AuthHttpClient>();
            services.AddScoped<ITozAwaBffHttpClient, TozAwaBffHttpClient>();
            services.AddScoped<IAuthHttpClient, AuthHttpClient>();

            return services;
        }
    }
}
