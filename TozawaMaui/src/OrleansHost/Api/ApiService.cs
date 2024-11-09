using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrleansHost.Attachment.Converters;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Services;
using Grains.Configurations;
using Grains.Context;
using Grains.Services;
using Shared.SignalR;
using Grains.Helpers;
using OrleansHost.Helpers;
using OrleansHost.Validation;
using OrleansHost.Auth.Controllers;
using System.Text.Json.Serialization;

namespace OrleansHost.Api
{
    public class ApiService : IHostedService
    {
        private readonly IWebHost host;

        public ApiService(IGrainFactory factory)
        {
            var configuration = new ConfigurationBuilder()
              .AddEnvironmentVariables()
              .AddJsonFile("OrleansHost.settings.Development.json", true)
              .AddJsonFile("OrleansHost.settings.json", false)
              .Build();

            host = WebHost
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var appSettings = services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

                    // For Identity  
                    services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<TozawangoDbContext>()
                        .AddDefaultTokenProviders();

                    // For Entity Framework  
                    services.AddDbContext<TozawangoDbContext>(options =>
                    {
                        options.UseSqlServer(appSettings.ConnectionStrings.Sql, b => b.MigrationsAssembly("OrleansHost"));
                    });

                    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
                    services.RegisterValidationService();
                    services.AddScoped<Grains.Auth.Services.ICurrentUserService, Grains.Auth.Services.CurrentUserService>();
                    services.AddScoped<ICurrentUserConverter, CurrentUserConverter>();
                    services.AddScoped<Grains.Auth.Services.IDataProtectionProviderService, Grains.Auth.Services.DataProtectionProviderService>();
                    services.AddScoped<IUserTokenService, UserTokenService>();
                    services.AddScoped<ICurrentCountry, CurrentCountry>();
                    services.AddScoped<IFileAttachmentConverter, FileAttachmentConverter>();
                    services.AddScoped<IFileAttachmentCreator, FileAttachmentCreator>();
                    services.AddScoped<IGoogleService, GoogleService>();
                    services.AddScoped<IPasswordHashService, PasswordHashService>();
                    services.AddScoped<IEncryptDecrypt, EncryptDecrypt>();

                    services.AddAuthentication(options =>
                    {
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
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
                    });

                    services.AddMvcCore(options =>
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

                    services.AddFluentValidationAutoValidation();
                    services.AddValidatorsFromAssemblyContaining<Program>();

                    services.AddHttpContextAccessor();
                    services.AddScoped<HttpContextAccessor>();
                    services.AddHttpClient();
                    services.AddScoped<HttpClient>();

                    services.AddSingleton(factory);

                    services.AddSignalR();

                    services.AddControllers()
                        .AddNewtonsoftJson()
                        .AddJsonOptions(o =>
                        {
                            o.JsonSerializerOptions.Converters.Add(new SystemTextJsonExceptionConverter());
                            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                        })
                        .AddApplicationPart(typeof(WeatherController).Assembly)
                        .AddControllersAsServices();

                    services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v0", new OpenApiInfo
                        {
                            Title = nameof(Grains),
                            Version = "v0"
                        });
                    });

                    services.AddCors(options =>
                    {
                        options.AddPolicy(nameof(ApiService),
                            builder =>
                            {
                                builder
                                    .WithOrigins(
                                        "https://localhost:7122")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials();
                            });
                    });
                })
                .Configure(app =>
                {
                    app.UseForwardedHeaders();
                    app.UseRouting();
                    app.UseCors(nameof(ApiService));
                    app.UseHttpsRedirection();

                    app.UseStaticFiles();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseMiddleware<ErrorWrappingMiddleware>();
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v0/swagger.json", nameof(Grains));
                    });
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapHub<ClientHub>("/hubs/clienthub");
                    });
                })
                .UseUrls("https://localhost:8081")
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken) =>
            host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) =>
            host.StopAsync(cancellationToken);
    }
}