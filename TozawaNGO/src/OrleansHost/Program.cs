using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansHost.Api;
//using Orleans.Providers.Streams.AzureQueue;

namespace OrleansHost
{
    public class Program
    {
        public static Task Main(string[] args)
        {
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
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });

                    services.AddHostedService<ApiService>();
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
                    builder.UseDashboard(options =>
                    {
                        options.HideTrace = true;
                    });
                })
                .RunConsoleAsync();
        }
    }
}