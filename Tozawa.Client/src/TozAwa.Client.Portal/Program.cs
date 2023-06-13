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
using Tozawa.Client.Portal.Services;
using Tozawa.Client.Portal.Shared;
using Tozawa.Client.Portal.StateHandler;
using TozAwa.Client.Portal.Helpers;
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
            builder.Services.AddTransient(_ => new CustomMessageHandler()
            {
                DefaultBrowserRequestMode = BrowserRequestMode.Cors,
                DefaultBrowserRequestCache = BrowserRequestCache.NoStore
            });

            // Optional: Register the HttpClient service using the named client "Default"
            // This will use this client when using @inject HttpClient
            builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorServerHttpClient"));
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
                options.AddPolicy("tozawaroot", policy => policy.RequireClaim("root-user", "true"));
            });
            await builder.Build().RunAsync();
        }
    }
}