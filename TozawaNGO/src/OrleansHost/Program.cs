using Grains;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansHost.Api;
using Shared.Settings;
using Shared.SignalR;
//using Orleans.Providers.Streams.AzureQueue;

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
                    services.AddSingleton(typeof(HubLifetimeManager<>), typeof(DefaultHubLifetimeManager<>));
                    services.Configure<SiloSettings>(configuration.GetSection(nameof(SiloSettings)));
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });

                    services.AddHostedService<ApiService>();
                    services.AddSignalR();
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
                    builder.UseSignalR(signalRConfig =>
                    {
                        signalRConfig.UseFireAndForgetDelivery = true;

                        signalRConfig.Configure(sb =>
                        {
                            sb.AddMemoryGrainStorage("SignalRStorage");
                        });
                    });

                    builder.RegisterHub<ClientHub>();
                    builder.UseDashboard(options =>
                    {
                        options.HideTrace = true;
                    });
                })
                .RunConsoleAsync();
        }
    }
}