using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Grains;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace TozawaNGO.Services
{
    public class TodoService(ILogger<TodoService> logger, IClusterClient client)
    {
        private readonly ILogger<TodoService> logger = logger;
        private readonly IClusterClient _client = client;

        public async Task<ImmutableArray<TodoItem>> GetAllAsync(Guid ownerKey)
        {
            // get all the todo item keys for this owner
            var itemKeys = await _client.GetGrain<ITodoManagerGrain>(ownerKey)
                .GetAllAsync();

            // fan out to get the individual items from the cluster in parallel
            var tasks = ArrayPool<Task<TodoItem>>.Shared.Rent(itemKeys.Length);
            try
            {
                // issue all individual requests at the same time
                for (var i = 0; i < itemKeys.Length; ++i)
                {
                    tasks[i] = _client.GetGrain<ITodoGrain>(itemKeys[i]).GetAsync();
                }

                // build the result as requests complete
                var result = ImmutableArray.CreateBuilder<TodoItem>(itemKeys.Length);
                for (var i = 0; i < itemKeys.Length; ++i)
                {
                    var item = await tasks[i];

                    // we can get a null result if the individual grain failed to unregister
                    // in this case we can finish the job here
                    if (item == null)
                    {
                        await _client.GetGrain<ITodoManagerGrain>(ownerKey).UnregisterAsync(itemKeys[i]);
                    }

                    result.Add(item);
                }
                return result.ToImmutable();
            }
            finally
            {
                ArrayPool<Task<TodoItem>>.Shared.Return(tasks);
            }
        }

        public Task<TodoItem> GetAsync(Guid id) =>
             _client.GetGrain<ITodoGrain>(id).GetAsync();
        public async Task SetAsync(TodoItem item)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new("OwnerKey", item.OwnerKey.ToString()),
                new("Key", item.Key.ToString()),
                new("Title", item.Title),
                new("IsDome", item.IsDone.ToString())
            };

            var content = new FormUrlEncodedContent(pairs);

            var client = new HttpClient { BaseAddress = new Uri($"https://localhost:8081") };

            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpRequestMessage request = new(HttpMethod.Post, $"/api/todo/{item.OwnerKey}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(item),
                                                Encoding.UTF8,
                                                "application/json")//CONTENT-TYPE header
            };

            await client.SendAsync(request)
                   .ContinueWith(responseTask =>
                   {
                       Console.WriteLine("Response: {0}", responseTask.Result);
                   });
        }

        public Task DeleteAsync(Guid itemKey) =>
            _client.GetGrain<ITodoGrain>(itemKey).ClearAsync();

        public Task<StreamSubscriptionHandle<TodoNotification>> SubscribeAsync(Guid ownerKey, Func<TodoNotification, Task> action) =>
            _client.GetStreamProvider("SMS")
                .GetStream<TodoNotification>(ownerKey)
                .SubscribeAsync(new TodoItemObserver(logger, action));

        private class TodoItemObserver(ILogger<TodoService> logger, Func<TodoNotification, Task> action) : IAsyncObserver<TodoNotification>
        {
            private readonly ILogger<TodoService> _logger = logger;
            private readonly Func<TodoNotification, Task> _action = action;

            public Task OnCompletedAsync() => Task.CompletedTask;

            public Task OnErrorAsync(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Task.CompletedTask;
            }
            public Task OnNextAsync(TodoNotification item, StreamSequenceToken token = null) => _action(item);
        }
    }
}