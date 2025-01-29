using MudBlazor;
using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services;

public class MemberService(ITozAwaBffHttpClient client)
{
    private readonly ITozAwaBffHttpClient _client = client;
    private const string _baseUriPath = $"member";

    public async Task<GetResponse<TableData<MemberDto>>> GetItems(string page, string pageSize, bool includeDeleted, string searchString, string pageOfEmail, string email = "")
    {
        var uri = new GetItemsQueryParameters(page, pageSize, includeDeleted, searchString, email, pageOfEmail).ToQueryString(_baseUriPath);
        return await _client.SendGet<TableData<MemberDto>>(uri);
    }
    public async Task<GetResponse<MemberDto>> GetItem(Guid id)
    {
        return await _client.SendGet<MemberDto>($"{_baseUriPath}/{id}");
    }
    public async Task<UpdateResponse> PatchMember(Guid id, PatchMemberRequest request)
    {
        return await _client.SendNoEntityPatch<PatchMemberRequest>($"{_baseUriPath}/{id}", request.ToPatchDocument());
    }
    public async Task<bool> EmailExists(string email)
    {
        email = email.Trim();
        var response = await GetItems("0", "1000", false, null, null, email);
        return response.Entity?.TotalItems > 0;
    }
    public async Task<AddResponse<MemberDto>> AddMember(AddMemberRequest request)
    {
        return await _client.SendPost<MemberDto>(_baseUriPath, request);
    }
    public async Task<AddResponse<LoginResponseDto>> SwitchOrganization(Guid organizationId)
    {
        return await _client.SendPost<LoginResponseDto>($"{_baseUriPath}/switch", new { Id = organizationId });
    }
}
