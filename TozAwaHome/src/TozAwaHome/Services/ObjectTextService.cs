using System.Threading.Tasks;
using TozAwaHome.HttpClients;
using TozAwaHome.Models.FormModels;
using TozAwaHome.Models.ResponseRequests;

namespace TozAwaHome.Services
{
    public class ObjectTextService
    {
        private readonly ITozAwaBffHttpClient _client;
        public ObjectTextService(ITozAwaBffHttpClient client) => _client = client;
        public async Task<UpdateResponse> UpdateObjectText(UpdateObjectTextCommand request)
        {
            return await _client.SendNoEntityPut<UpdateObjectTextCommand>($"/api/objecttext", request);
        }
    }
}
