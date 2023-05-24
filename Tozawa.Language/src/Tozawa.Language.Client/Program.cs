using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using Tozawa.Language.Client.Configuration;
using Tozawa.Language.Client.Extensions;
using Tozawa.Language.Client.HttpClients;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    //only add details when debugging
    o.DetailedErrors = environment.IsDevelopment();
});

builder.Services.RegisterScopeds();

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

builder.Services.AddMudBlazorDialog();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddHttpClient();


builder.Services.Configure<CookiePolicyOptions>(options =>
           {
               options.CheckConsentNeeded = context => true;
               options.MinimumSameSitePolicy = SameSiteMode.Lax;
           });
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LogoutPath = new PathString("/logout?returnUrl=/");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthorization(config =>
   {
       config.AddPolicy("root-user", policy => policy.RequireClaim("root-user", appSettings.RootKey.Key));
   });

var app = builder.Build();

app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization();

app.UseEndpoints(endpoinds =>
{
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
});

app.Run();
