using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TozawaMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            AppSettings? appSettings = new();

            ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
            
            // Load appsettings.json
            var config= new ConfigurationBuilder()
                .AddJsonFile("wwwroot/appsettings.json", false)
                .AddJsonFile("wwwroot/appsettings.Development.json", true)
                .Build();

            configuration.AddConfiguration(config);
            appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

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

            builder.Services.AddScoped<ITranslationService, TranslationService>();
            builder.Services.AddScoped<AuthenticationStateProvider, ShareRazorClassLibrary.Helpers.AuthStateProvider>();
            builder.Services.AddScoped<AuthStateProvider>();
            builder.Services.AddScoped(sp => new HttpClient { });
            builder.Services.AddScoped<IAuthHttpClient, AuthHttpClient>();
            builder.Services.AddScoped<ITozAwaBffHttpClient, TozAwaBffHttpClient>();
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<WeatherForecastService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
