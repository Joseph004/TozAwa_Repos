using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.HttpClients;

namespace TozawaMauiHybrid.Extensions;

public static class HttpClientsServiceCollectionExtension
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services, AppSettings appSettings)
    {
#if DEBUG
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            services.AddHttpClient<IAuthHttpClient, AuthHttpClient>(c =>
            {
                c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(c => HttpHelper.GetInsecureHandler());
            services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>(c =>
                {
                    c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                }).ConfigurePrimaryHttpMessageHandler(c => HttpHelper.GetInsecureHandler());
        }
        else
        {
            services.AddHttpClient<IAuthHttpClient, AuthHttpClient>(c =>
            {
                c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>(c =>
                {
                    c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                });
        }
#else
           services.AddHttpClient<IAuthHttpClient, AuthHttpClient>(c => 
            {
                c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        services.AddHttpClient<ITozAwaBffHttpClient, TozAwaBffHttpClient>(c => 
            {
                c.BaseAddress = new Uri(appSettings.TozAwaNGOApiSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
#endif
        return services;
    }
}