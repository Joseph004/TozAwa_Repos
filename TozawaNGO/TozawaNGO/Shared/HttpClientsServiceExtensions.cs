using Microsoft.Extensions.DependencyInjection;
using TozawaNGO.HttpClients;
using System.Net.Http.Headers;
using TozawaNGO.Helpers;

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