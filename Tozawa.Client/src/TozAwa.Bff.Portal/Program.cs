using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
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
var jwtSettings = configuration.GetSection("JWTSettings");

/* builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
    }); */

builder.Services.AddAuthentication()
               .AddJwtBearer("tzuserauthentication", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["validIssuer"],
        ValidAudience = jwtSettings["validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
    };
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
               {
                   opt.Audience = appSettings.AAD.ResourceId;
                   opt.Authority = $"{appSettings.AAD.Instance}{appSettings.AAD.TenantId}";
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

builder.Services.AddCors();
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

app.UseCors("ALL");
app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.UseCors(
       builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()
   );
app.UseMvc();
app.Run();
