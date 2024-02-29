using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shared.SignalR;

namespace OrleansHost.Api
{
    public class ApiService : IHostedService
    {
        private readonly IWebHost host;

        public ApiService(IGrainFactory factory)
        {
            host = WebHost
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(factory);

                    services.AddSignalR();

                    services.AddControllers()
                        .SetCompatibilityVersion(CompatibilityVersion.Latest)
                        .AddApplicationPart(typeof(WeatherController).Assembly)
                        .AddControllersAsServices();

                    //services.AddSignalR().AddOrleans();

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
                    app.UseRouting();
                    app.UseCors(nameof(ApiService));
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