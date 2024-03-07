using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using OrleansHost.Api;
using Grains.Auth.Models.Authentication;
using Grains.Configurations;
using Grains.Context;
using Shared.Settings;
using Shared.SignalR;
using OrleansHost.Service;

namespace OrleansHost
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("OrleansHost.settings.Development.json", true)
                .AddJsonFile("OrleansHost.settings.json", false)
                .Build();

            return new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddCommandLine(args);
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddFilter("Orleans.Runtime.Management.ManagementGrain", LogLevel.Warning);
                    builder.AddFilter("Orleans.Runtime.SiloControl", LogLevel.Warning);
                })
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

                    services.AddScoped<Grains.Auth.Services.ICurrentUserService, Grains.Auth.Services.CurrentUserService>();
                    services.AddSingleton(typeof(HubLifetimeManager<>), typeof(DefaultHubLifetimeManager<>));
                    services.Configure<SiloSettings>(configuration.GetSection(nameof(SiloSettings)));
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });

                    services.AddHostedService<ApiService>();
                    services.AddSignalR().AddOrleans();
                })
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering();
                    builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "OrleansBasics";
                    });
                    builder.AddMemoryGrainStorageAsDefault();
                    builder.AddMemoryStreams("SMS");
                    builder.AddMemoryGrainStorage("PubSubStore");
                    builder.AddMemoryGrainStorageAsDefault();
                    builder.UseSignalR();

                    builder.RegisterHub<ClientHub>();
                    builder.UseDashboard(options =>
                    {
                        options.HideTrace = true;
                    });
                    builder.ConfigureServices(services =>
                    {
                        services.AddHostedService<StartupService>();
                    });
                })
                .RunConsoleAsync();
        }
    }
}