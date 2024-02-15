using System.Threading.Tasks;
using TozawaNGO.HttpClients;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Services
{
    public class ObjectTextService(ITozAwaBffHttpClient client)
    {
        private readonly ITozAwaBffHttpClient _client = client;

        public async Task<UpdateResponse> UpdateObjectText(UpdateObjectTextCommand request)
        {
            return await _client.SendNoEntityPut<UpdateObjectTextCommand>($"/api/objecttext", request);
        }
    }
}
