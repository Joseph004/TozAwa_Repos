using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TozAwaHome.Configurations;
using TozAwaHome.Data;
using TozAwaHome.Extensions;
using TozAwaHome.Services;

namespace TozAwaHome;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
		
#if ANDROID || IOS
		var config = new ConfigurationBuilder()
	  .AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), "appsettings.json", false, false)
	  .AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), "appsettings.Development.json", true, false)
	  .Build();
		builder.Configuration.AddConfiguration(config);
#else
   var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", false)
	.AddJsonFile("appsettings.Development.json", true)
	.Build();
builder.Configuration.AddConfiguration(config);
#endif

		ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

        var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));
        builder.Services.AddSingleton(appSettings);


		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IDataProtectionProviderService, DataProtectionProviderService>();
        builder.Services.AddSingleton<IAppService, AppService>();
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddMudServices();

        // use this to add MudServices and the MudBlazor.Extensions
        //builder.Services.AddMudServicesWithExtensions();

        // or this to add only the MudBlazor.Extensions
        //builder.Services.AddMudExtensions();

		builder.Services.AddSingleton<HttpClient>();
		builder.Services.RegisterHttpClients();

        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<ITranslationService, TranslationService>();
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<ObjectTextService>();
        builder.Services.AddSingleton<MemberService>();
        builder.Services.AddSingleton<AttachmentService>();
		builder.Services.AddSingleton<LoadingState>();

        return builder.Build();
	}
}
