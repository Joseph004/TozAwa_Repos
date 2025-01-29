using MudBlazor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Extensions;
using Fluxor;

namespace TozawaMauiHybrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

            var a = Assembly.GetExecutingAssembly();
            var settings = string.Empty;
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                settings = $"{a.GetName().Name}.appsettings.Android.json";
            }
            else
            {
                settings = $"{a.GetName().Name}.appsettings.Development.json";
            }
#else
            settings = $"{a.GetName().Name}.appsettings.json";
#endif
            using var stream = a.GetManifestResourceStream(settings);
            var config = new ConfigurationBuilder()
            .AddJsonStream(stream ?? new MemoryStream())
            .Build();

            configuration.AddConfiguration(config);
            var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServicesWithExtensions(c =>
            {
                c.WithoutAutomaticCssLoading();
                c.WithDefaultDialogOptions(ex =>
                 {
                     ex.Position = DialogPosition.BottomRight;
                 });
            });
            builder.Services.AddFluxor(options => options.ScanAssemblies(typeof(MauiProgram).Assembly));
            builder.Services.AddScoped<ScrollTopState>();
            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<LoadingState>();
            builder.Services.AddScoped<FirstloadState>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ICountryCityService, CountryCityService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.RegisterHttpClients(appSettings);
            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("admin-member",
                 policy => policy.RequireClaim("admin-member", "MemberIsAdmin"));
            });
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<MemberService>();
            builder.Services.AddScoped<ISnackBarService, SnackBarService>();
            builder.Services.AddScoped<AuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AuthStateProvider>());
            builder.Services.AddScoped<PreferencesStoreClone>();
            builder.Services.AddScoped<ITranslationService, TranslationService>();
            builder.Services.AddScoped<WeatherForecastService>();
            builder.Services.AddScoped<NavMenuTabState>();
            builder.Services.AddScoped<AttachmentService>();

            //builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            return builder.Build();
        }
    }
}
