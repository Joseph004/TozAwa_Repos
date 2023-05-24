using TozAwaHome.HttpClients;

namespace TozAwaHome.Extensions
{
    public static class HttpClientsServiceExtensions
    {
        public static IServiceCollection RegisterHttpClients(this IServiceCollection services)
        {
            services.AddSingleton<IAuthHttpClient, AuthHttpClient>();
            services.AddSingleton<ITozAwaBffHttpClient, TozAwaBffHttpClient>();
            services.AddSingleton<IAuthHttpClient, AuthHttpClient>();

            return services;
        }
    }
}
