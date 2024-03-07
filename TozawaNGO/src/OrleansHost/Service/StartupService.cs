using Grains;
using Grains.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrleansHost.Service;

public class StartupService(IServiceProvider services) : IHostedService
{
    private IServiceProvider _services = services;

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var scope = _services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IGrainFactory>();
        var items = new List<TodoItem>{
                new(Guid.NewGuid(), "Test one", false, SystemTextId.ToDoOwnerId),
                new(Guid.NewGuid(), "Test two", false, SystemTextId.ToDoOwnerId),
                new(Guid.NewGuid(), "Test three", false, SystemTextId.ToDoOwnerId)
            };

        foreach (var item in items)
        {
            await factory.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
        }

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        // The code in here will run when the application stops
        // In your case, nothing to do
        return Task.CompletedTask;
    }
}