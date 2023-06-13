using Microsoft.Extensions.DependencyInjection;
using Tozawa.Client.Portal.HttpClients;
using System.Net.Http.Headers;
using TozAwa.Client.Portal.Helpers;

namespace Tozawa.Client.Portal.Shared;

public static class HttpClientsServiceCollectionExtension
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient<IAuthHttpClient, AuthHttpClient>("BlazorServerHttpClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<CustomMessageHandler>();
        services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>("BlazorServerHttpClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<CustomMessageHandler>();
        services.AddHttpClient<IAuthHttpClient, AuthHttpClient>("BlazorServerHttpClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<CustomMessageHandler>();

        return services;
    }
}