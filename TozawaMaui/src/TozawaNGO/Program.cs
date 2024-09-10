using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MudBlazor;
using MudBlazor.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans.Configuration;
using TozawaNGO;
using TozawaNGO.Configurations;
using TozawaNGO.Helpers;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using Fluxor;
using Shared.SignalR;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

builder.Services.AddRazorPages();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(20);
    opt.Cookie.IsEssential = true;
});

builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 100 * 1024 * 1024;
    options.DisableImplicitFromServicesParameters = true;
});
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = true);
}
else
{
    builder.Services.AddServerSideBlazor();
}

builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddDataProtection();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Host.UseOrleansClient(client =>
{
    client.UseSignalR(configure: null);
    client.UseLocalhostClustering();
    client.AddMemoryStreams("SMS");
    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "OrleansBasics";
    });
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}).AddOrleans();

builder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly));
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin-member", policy => policy.RequireClaim("admin-member", "MemberIsAdmin"));
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();


builder.Services.AddMvcCore(options =>
{
    options.Filters.Add(new RequireHttpsAttribute());
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMudServices();

// ******
// BLAZOR COOKIE Auth Code (begin)
// From: https://github.com/aspnet/Blazor/issues/1554
// HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();
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

builder.Services.RegisterHttpClients();

builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<TodoService>();
builder.Services.AddScoped<ICookie, Cookie>();
builder.Services.AddScoped<TozawaNGO.StateHandler.UserState>();
builder.Services.AddScoped<TozawaNGO.StateHandler.ScrollTopState>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<TozawaNGO.Services.ICurrentUserService, TozawaNGO.Services.CurrentUserService>();
builder.Services.AddScoped<ISnackBarService, SnackBarService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<BlazorServerAuthStateCache>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<LoadingState>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IEncryptDecrypt, EncryptDecrypt>();

builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("ALL");
app.UseHttpsRedirection();

app.UseStaticFiles();

RequestLocalizationOptions GetLocalizationOptions()
{
    var cultures = appSettings.Languages.ToDictionary(x => x.Culture, x => x.LongName);

    var supportedCultures = cultures.Keys.ToArray();

    var localizationOpotions = new RequestLocalizationOptions()
       .AddSupportedCultures(supportedCultures)
       .AddSupportedUICultures(supportedCultures);

    return localizationOpotions;
}

app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization(GetLocalizationOptions());

app.MapControllers();
app.MapHub<ClientHub>("/hubs/clienthub");
app.Use(async (context, next) =>
{
    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
    await next.Invoke();
});
app.MapBlazorHub(configureOptions: options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
    options.TransportMaxBufferSize = 131072;
    options.ApplicationMaxBufferSize = 131072;
});
app.MapFallbackToPage("/_Host");

app.Run();