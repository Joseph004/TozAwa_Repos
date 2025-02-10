using ShareRazorClassLibrary.HttpClients;

namespace TozawaNGO.Shared;

public static class HttpClientsServiceCollectionExtension
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IAuthHttpClient, AuthHttpClient>();
        services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>();

        return services;
    }
}