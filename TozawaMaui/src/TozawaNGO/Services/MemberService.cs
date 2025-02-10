using MudBlazor;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.ResponseRequests;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Services
{
    public class MemberService(ITozAwaBffHttpClient client)
    {
        private readonly ITozAwaBffHttpClient _client = client;
        private const string _baseUriPath = $"member";

        public async Task<GetResponse<TableData<MemberDto>>> GetItems(string page, string pageSize, bool includeDeleted, string searchString, string pageOfEmail, string email = "")
        {
            var uri = new GetItemsQueryParameters(page, pageSize, includeDeleted, searchString, email, pageOfEmail).ToQueryString($"{_baseUriPath}/members");
            return await _client.SendGet<TableData<MemberDto>>(uri);
        }

        public async Task<AddResponse<LoginResponseDto>> SwitchOrganization(Guid organizationId)
        {
            return await _client.SendPost<LoginResponseDto>($"{_baseUriPath}/switch", new { Id = organizationId });
        }
    }
}
