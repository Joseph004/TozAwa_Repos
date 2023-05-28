using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.Extensions
{
    public static class ScopedServicesExtensions
    {
        public static IServiceCollection RegisterScopeds(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ISnackBarService, SnackBarService>();

            services.AddScoped<HttpContextAccessor>();
            services.AddScoped<IDataProtectionProviderService, DataProtectionProviderService>();


            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            services.AddHttpClient<ILanguagesHttpClient, LanguagesHttpClient>();
            services.AddHttpClient<ISystemTypeHttpClient, SystemTypeHttpClient>();
            services.AddHttpClient<ITranslationHttpClient, TranslationHttpClient>();
            services.AddHttpClient<IXliffHttpClient, XliffHttpClient>();
            services.AddHttpClient<IAuthHttpClient, AuthHttpClient>();
            services.AddHttpClient<HttpClient>();

            return services;
        }
    }
}