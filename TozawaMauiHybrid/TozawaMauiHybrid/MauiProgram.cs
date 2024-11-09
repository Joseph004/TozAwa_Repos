using MudBlazor;
using MudBlazor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Extensions;

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

            builder.Services.AddSingleton<ScrollTopState>();
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<LoadingState>();
            builder.Services.AddSingleton<FirstloadState>();
            builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
            builder.Services.RegisterHttpClients(appSettings);
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<AuthStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(s => s.GetRequiredService<AuthStateProvider>());
            builder.Services.AddSingleton<PreferencesStoreClone>();
            builder.Services.AddSingleton<ITranslationService, TranslationService>();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<NavMenuTabState>();

            //builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            return builder.Build();
        }
    }
}
