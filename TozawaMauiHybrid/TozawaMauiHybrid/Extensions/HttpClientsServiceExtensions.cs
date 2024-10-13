using TozawaMauiHybrid.HttpClients;

namespace TozawaMauiHybrid.Extensions;

public static class HttpClientsServiceCollectionExtension
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IAuthHttpClient, AuthHttpClient>();
        services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>();

        return services;
    }
}