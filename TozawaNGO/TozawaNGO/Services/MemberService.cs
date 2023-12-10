using System;
using System.Threading.Tasks;
using MudBlazor;
using TozawaNGO.HttpClients;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Services
{
    public class MemberService
    {
        private readonly ITozAwaBffHttpClient _client;
        private const string _baseUriPath = $"member";
        public MemberService(ITozAwaBffHttpClient client) => _client = client;
        public async Task<GetResponse<TableData<MemberDto>>> GetItems(TableState state, bool includeDeleted, string searchString, string pageOfEmail, string email = "")
        {
            var uri = new GetItemsQueryParameters(state, includeDeleted, searchString, email, pageOfEmail).ToQueryString(_baseUriPath);
            return await _client.SendGet<TableData<MemberDto>>(uri);
        }
        public async Task<UpdateResponse> PatchMember(Guid id, PatchMemberRequest request)
        {
            return await _client.SendNoEntityPatch<PatchMemberRequest>($"{_baseUriPath}/{id}", request.ToPatchDocument());
        }

        public async Task<bool> EmailExists(string email)
        {
            email = email.Trim();
            var response = await GetItems(new TableState(), false, null, null, email);
            return response.Entity?.TotalItems > 0;
        }

        public async Task<AddResponse<MemberDto>> AddMember(AddMemberRequest request)
        {
            return await _client.SendPost<MemberDto>(_baseUriPath, request);
        }
    }
}
