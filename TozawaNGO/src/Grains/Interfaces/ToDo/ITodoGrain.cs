using Orleans;
using Grains;
using System.Threading.Tasks;

namespace Grains
{
    public interface ITodoGrain : IGrainWithGuidKey
    {
        Task SetAsync(TodoItem item);
        Task ActivateAsync(TodoItem item);
        Task ClearAsync();
        Task<TodoItem> GetAsync();
    }
}