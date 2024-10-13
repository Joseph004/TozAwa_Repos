using MudBlazor;
using MudBlazor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Extensions;

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
           });

            builder.Services.RegisterHttpClients();

            builder.Services.AddScoped<AuthStateProvider>();
            builder.Services.AddSingleton<PreferencesStoreClone>();
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var handler = HttpHelper.GetInsecureHandler();
                builder.Services.AddSingleton(sp => new HttpClient(handler) { });
            }
            else
            {
                builder.Services.AddSingleton(sp => new HttpClient { });
            }
#else
           builder.Services.AddSingleton(sp => new HttpClient { });
#endif
            builder.Services.AddScoped<ITranslationService, TranslationService>();
            builder.Services.AddScoped<WeatherForecastService>();
            return builder.Build();
        }
    }
}
