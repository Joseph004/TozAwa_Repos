using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TozawaNGO;
using TozawaNGO.Auth.Models;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Converters;
using TozawaNGO.Auth.Services;
using TozawaNGO.Configurations;
using TozawaNGO.Context;
using TozawaNGO.Data;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;
using TozawaNGO.Shared;
using TozawaNGO.StateHandler;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = true);
}
else
{
    builder.Services.AddServerSideBlazor();
}

builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddDataProtection();

// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TozawangoDbContext>()
    .AddDefaultTokenProviders();

// For Entity Framework  
builder.Services.AddDbContext<TozawangoDbContext>(options =>
{
    options.UseModel(TozawaNGO.MyCompiledModels.TozawangoDbContextModel.Instance);
    options.UseSqlServer(appSettings.ConnectionStrings.Sql);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<TozawaNGO.Auth.Services.ICurrentUserService, TozawaNGO.Auth.Services.CurrentUserService>();
builder.Services.AddScoped<ICurrentUserConverter, CurrentUserConverter>();
builder.Services.AddScoped<TozawaNGO.Auth.Services.IDataProtectionProviderService, TozawaNGO.Auth.Services.DataProtectionProviderService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.AddScoped<ICurrentCountry, CurrentCountry>();
builder.Services.AddScoped<TokenProvider>();

// Optional: Register the HttpClient service using the named client "Default"
// This will use this client when using @inject HttpClient
//builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorServerHttpClient"));
//builder.Services.AddApiAuthorization();

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddJwtBearer("tzuserauthentication", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = appSettings.JWTSettings.ValidIssuer,
        ValidAudience = appSettings.JWTSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTSettings.SecurityKey))
    };
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization(config =>
   {
       config.AddPolicy("admin-member", policy => policy.RequireClaim("admin-member", "MemberIsAdmin"));
   });


builder.Services.AddMvcCore(options =>
{
    options.Filters.Add(new RequireHttpsAttribute());
}).AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>())
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
}); ;

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

// use this to add MudServices and the MudBlazor.Extensions
//builder.Services.AddMudServicesWithExtensions();

// or this to add only the MudBlazor.Extensions
builder.Services.AddMudExtensions();
builder.Services.RegisterHttpClients();

builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddScoped<ICookie, Cookie>();
builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<TozawaNGO.Services.ICurrentUserService, TozawaNGO.Services.CurrentUserService>();
builder.Services.AddScoped<ISnackBarService, SnackBarService>();
builder.Services.AddScoped<ObjectTextService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<BlazorServerAuthStateCache>();
builder.Services.AddScoped<FileService>();

builder.Services.AddScoped<LoadingState>();
builder.Services.AddScoped<AfterRenderState>();

builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<TozawangoDbContext>();
await Task.Run(async () =>
{
    var model = context.Model;
    var warmup = context.Users.Count();
    var warmup2 = await context.Users.FirstOrDefaultAsync(x => x.Email == "josephluhandu@yahoo.com");
});

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
//app.MapControllers();

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

//app.UseMvc();
app.MapControllers();
//app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
//


app.Run();