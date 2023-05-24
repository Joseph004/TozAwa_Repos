using System.Threading.Tasks;
using Tozawa.Client.Portal.HttpClients;
using Tozawa.Client.Portal.Models.FormModels;
using Tozawa.Client.Portal.Models.ResponseRequests;

namespace Tozawa.Client.Portal.Services
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
