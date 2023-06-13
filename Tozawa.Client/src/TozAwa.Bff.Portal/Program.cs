using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Controllers;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Services;
using Tozawa.Bff.Portal.Validation;
using Tozawa.Client.Portal.HttpClients;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

builder.Services.AddAuthorization(options =>
           {
               options.DefaultPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                   .Build();
           });
builder.Services.AddDataProtection();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMemberConverter, MemberConverter>();
builder.Services.AddScoped<ILanguageText, LanguageText>();
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.RegisterHttpClients(appSettings);
builder.Services.AddHttpClient<ILanguageHttpClient, LanguageHttpClient>();
builder.Services.RegisterValidationService();
builder.Services.AddScoped<IUserCountryByIp, UserCountryByIp>();

builder.Services.AddCors(options => options.AddPolicy("TozAwaCorsPolicyBff", builder =>
    {
        var origins = appSettings.CorsOrigins.Split(",");
        builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
    }));
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
    options.Filters.Add(new RequireHttpsAttribute());
}).AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>())
 .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                }); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseCors("TozAwaCorsPolicyBff");
app.UseAuthentication();

app.UseAuthorization();
app.UseNoSniffHeaders();
app.UseMvc();
app.Run();
