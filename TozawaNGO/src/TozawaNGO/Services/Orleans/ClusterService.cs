/* using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Grains;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TozawaNGO.Services
{
    public class ClusterService(ILogger<ClusterService> logger) : IHostedService
    {
        private readonly ILogger<ClusterService> logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Client.Connect(async error =>
            {
                logger.LogError(error, error.Message);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                return true;
            });
        }

        public Task StopAsync(CancellationToken cancellationToken) => Client.Close();

        public IClusterClient Client { get; } = new ClientBuilder()
            .ConfigureApplicationParts(manager => manager.AddApplicationPart(typeof(IWeatherGrain).Assembly).WithReferences())
            .UseLocalhostClustering()
            .AddSimpleMessageStreamProvider("SMS")
            .Build();
    }

    public static class ClusterServiceBuilderExtensions
    {
        public static IServiceCollection AddClusterService(this IServiceCollection services)
        {
            services.AddSingleton<ClusterService>();
            services.AddSingleton<IHostedService>(_ => _.GetService<ClusterService>());
            services.AddTransient(_ => _.GetService<ClusterService>().Client);
            return services;
        }
    }
} */