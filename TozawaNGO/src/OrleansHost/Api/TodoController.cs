using Microsoft.AspNetCore.Mvc;
using Grains;
using System.Buffers;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;

namespace OrleansHost.Api
{
    [ApiController]
    [Route("api/todo")]
    public class TodoController(IGrainFactory factory, IHubContext<ClientHub> hub) : ControllerBase
    {
        private readonly IGrainFactory _factory = factory;
        private readonly IHubContext<ClientHub> _hub = hub;

        [HttpGet("{itemKey}")]
        public Task<TodoItem> GetAsync([Required] Guid itemKey) =>
            _factory.GetGrain<ITodoGrain>(itemKey).GetAsync();

        [HttpDelete("{itemKey}")]
        public async Task DeleteAsync([Required] Guid itemKey)
        {
            await _factory.GetGrain<ITodoGrain>(itemKey).ClearAsync();
        }

        [HttpGet("list/{ownerKey}")]
        public async Task<ImmutableArray<TodoItem>> ListAsync([Required] Guid ownerKey)
        {
            // get all item keys for this owner
            var keys = await _factory.GetGrain<ITodoManagerGrain>(ownerKey).GetAllAsync();

            // fast path for empty owner
            if (keys.Length == 0) return [];

            // fan out and get all individual items in parallel
            var tasks = ArrayPool<Task<TodoItem>>.Shared.Rent(keys.Length);
            try
            {
                // issue all requests at the same time
                for (var i = 0; i < keys.Length; ++i)
                {
                    tasks[i] = _factory.GetGrain<ITodoGrain>(keys[i]).GetAsync();
                }

                // compose the result as requests complete
                var result = ImmutableArray.CreateBuilder<TodoItem>(tasks.Length);
                for (var i = 0; i < keys.Length; ++i)
                {
                    result.Add(await tasks[i]);
                }
                return result.ToImmutable();
            }
            finally
            {
                ArrayPool<Task<TodoItem>>.Shared.Return(tasks);
            }
        }

        public class TodoItemModel
        {
            [Required]
            public Guid Key { get; set; }

            [Required]
            public string Title { get; set; }

            [Required]
            public bool IsDone { get; set; }

            [Required]
            public Guid OwnerKey { get; set; }
        }

        [HttpPost, Route("{ownerKey}")]
        public async Task<ActionResult> PostAsync([Required] Guid ownerKey, [FromBody] TodoItemModel model)
        {
            model.OwnerKey = ownerKey;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = new TodoItem(model.Key, model.Title, model.IsDone, model.OwnerKey);
            await _factory.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
            await _hub.Clients.All.SendAsync("ToDoAdded", item.Key);
            return Ok();
        }
    }
}