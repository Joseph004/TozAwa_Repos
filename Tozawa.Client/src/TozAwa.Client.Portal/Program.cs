using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tozawa.Client.Portal.Configurations;
using Tozawa.Client.Portal.HttpClients;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Services;
using Tozawa.Client.Portal.Shared;
using Tozawa.Client.Portal.StateHandler;
using TozAwa.Client.Portal.Services;

namespace TozAwa.Client.Portal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                return config.GetSection("AppSettings").Get<AppSettings>();
            });

            var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            // Register the Message Handler
            builder.Services.AddScoped(_ => new DefaultBrowserOptionsMessageHandler
            {
                /*  DefaultBrowserRequestCache = BrowserRequestCache.NoStore,
                 DefaultBrowserRequestCredentials = BrowserRequestCredentials.Include, */
                DefaultBrowserRequestMode = BrowserRequestMode.Cors
            });

            builder.Services.AddHttpClient("TzDefault", client =>
            {
                client.BaseAddress = new Uri($"{appSettings.AADClient.Authority}");
                client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "https://localhost:44331");
                client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            })
                .AddHttpMessageHandler<DefaultBrowserOptionsMessageHandler>();

            builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("TzDefault"));

            builder.Services.AddApiAuthorization();
            builder.Services.AddScoped<IDataProtectionProviderService, DataProtectionProviderService>();
            builder.Services.AddTransient<IUserInfos, UserInfos>();
            builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        }
);
            builder.Services.AddMudBlazorDialog();

            // ******
            // BLAZOR COOKIE Auth Code (begin)
            // From: https://github.com/aspnet/Blazor/issues/1554
            // HttpContextAccessor
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddBlazoredLocalStorage(config =>
        {
            config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            config.JsonSerializerOptions.WriteIndented = false;
        });
            builder.Services.AddBlazoredSessionStorage(config =>
            {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
            });

            // use this to add MudServices and the MudBlazor.Extensions
            builder.Services.AddMudServicesWithExtensions();

            // or this to add only the MudBlazor.Extensions
            builder.Services.AddMudExtensions();

            builder.Services.RegisterHttpClients(appSettings.TozAwaBffApiSettings.ApiUrl);
            builder.Services.AddHttpClient<AuthHttpClient>();
            
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<CookieStorageAccessor>();
            builder.Services.AddScoped<ICookie, Cookie>();
            builder.Services.AddScoped<UserState>();
            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<ITranslationService, TranslationService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ISnackBarService, SnackBarService>();
            builder.Services.AddScoped<ObjectTextService>();
            builder.Services.AddScoped<MemberService>();
            builder.Services.AddScoped<AttachmentService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddSingleton<PageHistoryState>();
            builder.Services.AddScoped<LoadingState>();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("root-user", policy => policy.RequireClaim("root-user", "UserIsRoot"));
            });
            await builder.Build().RunAsync();
        }
    }
}