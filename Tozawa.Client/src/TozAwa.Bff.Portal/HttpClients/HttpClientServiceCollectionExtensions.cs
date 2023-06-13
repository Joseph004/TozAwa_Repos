using System.Net;
using Tozawa.Bff.Portal.Configuration;

namespace Tozawa.Bff.Portal.HttpClients;

public static class HttpClientServiceCollectionExtensions
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddHttpClient<ITozAwaAuthHttpClient, TozAwaAuthHttpClient>((provider, clientConfig) =>
        {
            clientConfig.BaseAddress = new Uri(appSettings.AuthSettings.ApiUrl);
            clientConfig.DefaultRequestHeaders.Add("tozawa-resourceId", appSettings.AuthSettings.ResourceId);
            clientConfig.DefaultRequestHeaders.Add("Accept", "application/json");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        });

        services.AddHttpClient<IAttachmentHttpClient, AttachmentHttpClient>((provider, clientConfig) =>
        {
            clientConfig.BaseAddress = new Uri(appSettings.AttachmentSettings.ApiUrl);
            clientConfig.DefaultRequestHeaders.Add("tozawa-resourceId", appSettings.AttachmentSettings.ResourceId);
            clientConfig.DefaultRequestHeaders.Add("Accept", "application/json");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        });

        /*  services.AddHttpClient<IAttachmentHttpClient, AttachmentHttpClient>((provider, clientConfig) =>
         {
             clientConfig.BaseAddress = new Uri(appSettings.AttachmentSettings.ApiUrl);
             clientConfig.DefaultRequestHeaders.Add("X-Api-Key", appSettings.AttachmentSettings.ApiKey);
             clientConfig.DefaultRequestHeaders.Add("Accept", "application/json");
         });

         services.AddHttpClient<IAuthorizationHttpClient, AuthorizationHttpClient>((provider, clientConfig) =>
         {
             clientConfig.BaseAddress = new Uri(appSettings.AuthSettings.ApiUrl);
             clientConfig.DefaultRequestHeaders.Add("X-Api-Key", appSettings.AuthSettings.ApiKey);
             clientConfig.DefaultRequestHeaders.Add("Accept", "application/json");
         });

         services.AddHttpClient<IConfigurationHttpClient, ConfigurationHttpClient>((provider, clientConfig) =>
         {
             clientConfig.BaseAddress = new Uri(appSettings.ConfigurationSettings.ApiUrl);
             clientConfig.DefaultRequestHeaders.Add("X-Api-Key", appSettings.ConfigurationSettings.ApiKey);
             clientConfig.DefaultRequestHeaders.Add("Accept", "application/json");
         }); */

        return services;
    }
}

public static class ResponseHeaderExtensions
{
    public static IApplicationBuilder UseNoSniffHeaders(this IApplicationBuilder builder)
    {
        return builder.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            await next();
        });
    }

}