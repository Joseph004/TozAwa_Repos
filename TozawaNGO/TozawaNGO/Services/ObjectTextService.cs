using System.Threading.Tasks;
using TozawaNGO.HttpClients;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Services
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
