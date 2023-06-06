using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tozawa.Language.Svc.Configuration;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Converters;
using Tozawa.Language.Svc.Files;
using Tozawa.Language.Svc.Services;
using Tozawa.Language.Svc.Validation;
using Tozawa.Language.Svc.XliffConverter;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("CustomerSettings/appsettings.json", false, true).AddJsonFile("CustomerSettings/appsettings.Development.json", true, true).Build();

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));
var jwtSettings = configuration.GetSection("AppSettings:JWTSettings");

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

builder.Services.AddControllers();

// For Entity Framework  
builder.Services.AddDbContext<LanguageContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.Sql));
builder.Services.AddSingleton(appSettings);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAzureFileTasks, AzureFileTasks>();
builder.Services.AddScoped<IXliffExportTransaction, XliffExportTransaction>();
builder.Services.AddScoped<IXliffConverter, XliffConverter>();
builder.Services.AddScoped<IExporter, Exporter>();
builder.Services.AddScoped<IXliffImporter, XliffImporter>();
builder.Services.AddScoped<IImportResultService, ImportResultService>();
builder.Services.AddScoped<ITranslatedTextConverter, TranslatedTextConverter>();

builder.Services.AddScoped<IXliffImportTransaction, XliffImportTransaction>();
builder.Services.AddScoped<IXliffImportTransaction, XliffImportTransaction>();
builder.Services.AddScoped<IXDocumentToXliff, DocumentToXliff>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.RegisterValidationService();

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
                });

var app = builder.Build();

app.UseCors("ALL");

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
